//Sample provided by Fabio Galuppo  
//August 2017

//compile:
//clang++ -O3 -std=c++14 -I../include -I./cppzmq/include -L/usr/local/Cellar/zeromq/4.2.2/lib -lzmq req.cpp -o ./bin/req.exe

//run:
//./bin/req.exe

#include <zmq.hpp>

#include <string>
#include <iostream>
#include <cstring>

#include "zmq_utils.hpp" 

int main(int argc, char* argv[]) {
    std::cout << "ZMQ version = " << zmq_utils::get_zmq_version() << "\n";
    
    zmq::context_t zmq_context(1);
    zmq::socket_t request_socket(zmq_context, ZMQ_REQ);

    request_socket.connect("tcp://localhost:60000");
    
    const char* msg = "Hello, World!";
    if (argc > 1)
        msg = argv[1];
 
    zmq::message_t zmsg{msg, msg + strlen(msg)};
    request_socket.send(zmsg, 0);

    request_socket.recv(&zmsg, 0);
    size_t size = zmsg.size();
    if (size > 0) {
        const char* begin = static_cast<const char*>(zmsg.data());
        const char* end = begin + size;
        std::cout << std::string(begin, end) << "\n";
    } else {
        std::cout << "no data returned from recv\n";
    }
    
    request_socket.close();
    zmq_context.close();

    return 0;
}