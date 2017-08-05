//Sample provided by Fabio Galuppo  
//August 2017

//compile:
//clang++ -I../include -I./random_util/include -I./cppzmq/include -L/usr/local/Cellar/zeromq/4.2.2/lib -std=c++1z -lzmq sender.cpp -o ./bin/sender.exe

//run:
//./bin/sender.exe "ID1"

#include <zmq.h>
#include <zmq.hpp>
#include "zmq_utils.hpp"

#include <string>
#include <iostream>
#include <tuple>

namespace internal {
    static inline size_t msg_send(zmq::socket_t& sender_socket, zmq::message_t& msg, int flags) {
        size_t msg_size = msg.size();
        if (sender_socket.send(msg, flags))
            return msg_size;
        return 0;
    }

    static inline size_t send(zmq::socket_t& sender_socket, zmq::message_t msg) {
        return msg_send(sender_socket, msg, ZMQ_DONTWAIT);
    }

    static inline size_t send_more(zmq::socket_t& sender_socket, zmq::message_t msg) {
        return msg_send(sender_socket, msg, ZMQ_DONTWAIT | ZMQ_SNDMORE);
    }
};

namespace internal::variadic {
    template <typename T, typename... Ts>
    struct bulk_sender {
        static void send(zmq::socket_t& sender_socket, size_t& total_bytes_sent, bool& success, T& data, Ts&... args) {
            size_t n = success ? internal::send_more(sender_socket, std::move(data)) : 0;
            total_bytes_sent += n;
            success = success && n > 0;
            bulk_sender<Ts...>::send(sender_socket, total_bytes_sent, success, args...);
        }
    };

    template <typename T>
    struct bulk_sender<T> {
        static void send(zmq::socket_t& sender_socket, size_t& total_bytes_sent, bool& success, T& data) {
            size_t n = success ? internal::send(sender_socket, std::move(data)) : 0;
            total_bytes_sent += n;
            success = success && n > 0;
        }
    };
};

using result_type = std::tuple<bool, size_t>;

namespace result {
    inline bool failed(result_type result) {
        return !std::get<0>(result);
    }

    inline bool succeeded(result_type result) {
        return std::get<0>(result);
    }

    inline size_t value(result_type result) {
        return std::get<1>(result);
    }
}

template <typename... Ts>
inline result_type bulk_send(zmq::socket_t& sender_socket, Ts&... args) {
    size_t total_bytes_sent = 0;
    bool success = true;
    internal::variadic::bulk_sender<Ts...>::send(sender_socket, total_bytes_sent, success, args...);
    return std::make_tuple(success, total_bytes_sent);
}

int main(int argc, const char* argv[]) {
    std::cout << "ZMQ version = " << zmq_utils::get_zmq_version() << "\n";
    if (argc < 2) {
        std::cout << "missing dealer_id\n";
        return 1;
    }
    const char* dealer_id = argv[1]; //dealer id
    std::cout << "dealer_id = " << dealer_id << "\n";
    
    zmq::context_t context;
    zmq::socket_t dealer(context, ZMQ_DEALER);
    dealer.setsockopt(ZMQ_IDENTITY, dealer_id, strlen(dealer_id));
    dealer.connect("tcp://127.0.0.1:60000");
    
    std::string data1 = "Hello", data2 = "World";

    zmq::message_t data_1_msg { data1.begin(), data1.end() };
    zmq::message_t data_2_msg { data2.begin(), data2.end() };

    bulk_send(dealer, data_1_msg, data_2_msg);

    dealer.close();
    context.close(); 
    
    return 0;
}