//Sample provided by Fabio Galuppo  
//April 2017

//compile:
//clang++ -std=c++14 -I../include -I./cppzmq/include -I./msgpack/include -L/usr/local/Cellar/zeromq/4.2.2/lib -lzmq sink.cpp -o ./bin/sink.exe

//run:
//./bin/sink.exe

#include <zmq.hpp>

#include <iostream>
#include <string>
#include <cctype>
#include <algorithm>

#include "zmq_utils.hpp"
#include "common.hpp"

int main(int argc, const char* argv[]) {
    std::cout << "ZMQ version = " << zmq_utils::get_zmq_version() << "\n";
    
    zmq::context_t zmq_context(1);
    zmq::socket_t sink_socket(zmq_context, ZMQ_PULL);
    
    sink_socket.bind("tcp://*:60003");

    std::cout << "Waiting for start...\n";

    zmq::message_t msg;
    sink_socket.recv(&msg, 0);
    int M = std::stoi(zmq_utils::to_string(msg));
    std::cout << "Waiting " << M << " messages...\n";
    for (int i = 0; i < M; ++i) {
        sink_socket.recv(&msg, 0);
        std::cout << "Result #" << (i + 1) << ": " << zmq_utils::to_string(msg) << "\n"; 
    }

    std::cout << "Gather completed...\n";

    sink_socket.close();
	zmq_context.close();

    return 0;
}