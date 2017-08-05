//Sample provided by Fabio Galuppo  
//August 2017

//compile:
//clang++ -I../include -I./random_util/include -I./cppzmq/include -L/usr/local/Cellar/zeromq/4.2.2/lib -std=c++14 -lzmq receiver.cpp -o ./bin/receiver.exe

//run:
//./bin/receiver.exe

#include <zmq.h>
#include <zmq.hpp>

#include "zmq_utils.hpp"

#include <array>
#include <string>
#include <iostream>
#include <signal.h>

volatile bool running = true;

void sigint_handler(int) {
    std::cout << "\nwait, we're trying to finish this process properly...\n";
    running = false;
}

int main(int argc, const char* argv[]) {
    std::cout << "ZMQ version = " << zmq_utils::get_zmq_version() << "\n";

    signal(SIGINT, sigint_handler);

    zmq::context_t context;
    zmq::socket_t router(context, ZMQ_ROUTER);
    router.setsockopt(ZMQ_ROUTER_HANDOVER, 1);
    router.bind("tcp://127.0.0.1:60000");

    std::array<zmq::pollitem_t, 1> items {
        zmq::pollitem_t {
            static_cast<void*>(router),
            0,
            ZMQ_POLLIN,
            0
        }
    };

    zmq::message_t id_msg, data_1_msg, data_2_msg;
    while (running) {
        const long timeout_ms = 5 * 1000;
        try {
            if (zmq::poll(items.data(), items.size(), timeout_ms)) {
                router.recv(&id_msg, 0);
                router.recv(&data_1_msg, 0);
                router.recv(&data_2_msg, 0);      
                
                std::string id = zmq_utils::to_string(id_msg);
                std::string data1 = zmq_utils::to_string(data_1_msg);
                std::string data2 = zmq_utils::to_string(data_2_msg);
                std::cout << id << " ";
                std::cout << data1 << " ";
                std::cout << data2 << " ";
                std::cout << "\n-----------------" << std::endl;
            }
        } catch (const zmq::error_t& err) {
            std::cout << "[ERROR] " << err.what() << "\n";
        } 
    }

    router.close();
    context.close();

    return 0;
}