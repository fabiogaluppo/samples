//Sample provided by Fabio Galuppo  
//April 2017

//compile:
//clang++ -std=c++11 -I../include -I./cppzmq/include -lzmq pub.cpp -o ./bin/pub.exe

//run:
//./bin/pub.exe

#include <zmq.hpp>

#include <iostream>
#include <string>
#include <cctype>
#include <algorithm>

#include "zmq_utils.hpp" 

int main(int argc, const char* argv[]) {
    std::cout << "ZMQ version = " << zmq_utils::get_zmq_version() << "\n";
    
    zmq::context_t zmq_context(1);
    zmq::socket_t publisher_socket(zmq_context, ZMQ_PUB);

    publisher_socket.bind("tcp://*:60001");
    publisher_socket.bind("ipc://pub.ipc");

    std::cout << "Publishing...\n";
    std::cout << "[CTRL + C] to finish...\n";

    std::string data;
    while (std::getline(std::cin, data)) {
        std::transform(data.begin(), data.end(), data.begin(), 
            [](int ch){ return std::toupper(ch); });
        publisher_socket.send(zmq::message_t(data.begin(), data.end()), 0);
    }

    publisher_socket.close();
	zmq_context.close();

    return 0;
}