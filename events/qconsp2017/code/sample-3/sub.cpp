//Sample provided by Fabio Galuppo  
//April 2017

//compile:
//clang++ -std=c++11 -I../include -I./cppzmq/include -lzmq sub.cpp -o ./bin/sub.exe

//run:
//./bin/sub.exe 5 "GOOG " "MSFT "

#include <zmq.hpp>

#include <iostream>
#include <string>
#include <cstring>
#include <cstdlib>

#include "zmq_utils.hpp" 

static inline std::string to_string(zmq::message_t& msg) {
    char* ptr = static_cast<char*>(msg.data());
    return std::move(std::string(ptr, ptr + msg.size()));
}

int main(int argc, const char* argv[]) {
    long N = 0L;
    char* end;
    if (argc < 3 || (N = std::strtol(argv[1], &end, 10)) == 0) {
		std::cout << "usage: " << argv[0] << " <N messages> <topic list>\n";
        std::cout << "example: " << argv[0] << " 10 \"APPL \" \"GOOG \" \"MSFT \"\n";
		return 0;
	}
    
    std::cout << "ZMQ version = " << zmq_utils::get_zmq_version() << "\n";
    
    zmq::context_t zmq_context(1);
    zmq::socket_t subscriber_socket(zmq_context, ZMQ_SUB);

    subscriber_socket.connect("tcp://localhost:60001");

    std::cout << "Listening...\n";
    std::cout << "[CTRL + C] to finish...\n";

    for (int i = 2; i < argc; ++i) {
        const char* filter = argv[i];
        subscriber_socket.setsockopt(ZMQ_SUBSCRIBE, filter, std::strlen(filter));
    }

    for (long i = 0; i < N; ++i) {
        zmq::message_t msg; 
        subscriber_socket.recv(&msg, 0);

        std::cout << to_string(msg) << "\n";
    }

    subscriber_socket.close();
    zmq_context.close();

    return 0;
}