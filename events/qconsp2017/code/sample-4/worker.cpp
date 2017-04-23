//Sample provided by Fabio Galuppo  
//April 2017

//compile:
//clang++ -std=c++14 -I../include -I./cppzmq/include -I./msgpack/include -lzmq worker.cpp -o ./bin/worker.exe

//run:
//./bin/worker.exe

#include <zmq.hpp>

#include <iostream>
#include <iomanip>
#include <string>
#include <cctype>
#include <algorithm>
#include <memory>
#include <tuple>
#include <thread>
#include <chrono>

#include "zmq_utils.hpp" 
#include "common.hpp"

zmq::message_t process(std::string& scmd, zmq::message_t& msg) {
    std::string result = "FAILED";
    
    if (is_command(scmd, COMMAND_1)) {
        auto x = command_1_args(msg);
        int a = std::get<0>(x), b = std::get<1>(x);
        std::cout << a << " " << b << "\n";
        std::this_thread::sleep_for(std::chrono::seconds(2)); //simulate workload
        result = std::to_string(a + b);
    } else if (is_command(scmd, COMMAND_2)) {
        auto x = command_2_args(msg);
        char a = std::get<0>(x), c = std::get<2>(x);
        bool b = std::get<1>(x);
        std::cout << a << " " << std::boolalpha << b << " " << c << "\n";
        std::this_thread::sleep_for(std::chrono::seconds(1)); //simulate workload
        char ch = (b ? a : c) - 32;
        result = std::string(10, ch);
    }

    return std::move(zmq::message_t(result.begin(), result.end()));
}

int main(int argc, const char* argv[]) {
    std::cout << "ZMQ version = " << zmq_utils::get_zmq_version() << "\n";
    
    zmq::context_t zmq_context(2);
    zmq::socket_t worker_socket(zmq_context, ZMQ_PULL);
    zmq::socket_t sink_socket(zmq_context, ZMQ_PUSH);
    
    zmq_utils::set_affinity(worker_socket, 1);
    zmq_utils::set_affinity(sink_socket, 2);

    worker_socket.connect("tcp://localhost:60002");
    sink_socket.connect("tcp://localhost:60003");

    while (true) {
        std::cout << "Waiting message...\n";

        zmq::message_t msg; 
        worker_socket.recv(&msg, 0);
        auto scmd = zmq_utils::to_string(msg);
        if (is_command(scmd, POISON_PILL))
            break;

        std::cout << "Processing message...\n";
        
        worker_socket.recv(&msg, 0);
        auto result = process(scmd, msg);

        std::cout << "Sending result...\n";
        sink_socket.send(result, 0);

        std::cout << "-----------------\n";
    }
    std::cout << "Stopping worker...\n";

    sink_socket.close();
	worker_socket.close();
    zmq_context.close();

    return 0;
}