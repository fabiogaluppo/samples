//Sample provided by Fabio Galuppo  
//April 2017

//compile:
//clang++ -std=c++14 -I../include -I./cppzmq/include -I./msgpack/include -I./random_util/include -lzmq source.cpp -o ./bin/source.exe

//run:
//./bin/source.exe 2 30

#include <zmq.hpp>

#include <iostream>
#include <string>
#include <cctype>
#include <algorithm>
#include <cstdio>
#include <cstdlib>
#include <random_int_util.hpp>

#include "zmq_utils.hpp"
#include "common.hpp"

int main(int argc, const char* argv[]) {
    long N = 0L, M = 0L;
    char* end;
    if (argc < 3 || 
        (N = std::strtol(argv[1], &end, 10)) == 0 ||
        (M = std::strtol(argv[2], &end, 10)) == 0) {
		std::cout << "usage: " << argv[0] << " <N workers> <M messages>\n";
        std::cout << "example: " << argv[0] << " 2 30\n";
		return 0;
	}

    std::cout << "ZMQ version = " << zmq_utils::get_zmq_version() << "\n";
    
    zmq::context_t zmq_context(2);
    zmq::socket_t source_socket(zmq_context, ZMQ_PUSH);
    zmq::socket_t sink_socket(zmq_context, ZMQ_PUSH);
    
    zmq_utils::set_affinity(source_socket, 1);
    zmq_utils::set_affinity(sink_socket, 2);

    source_socket.bind("tcp://*:60002");
    sink_socket.connect("tcp://localhost:60003");

    //notify # of messages to sink
    std::string SM = std::to_string(M);
    sink_socket.send(zmq::message_t(SM.begin(), SM.end()), 0);

    std::cout << "Waiting for workers...\n";
    std::cout << "Press [ENTER] when workers are ready...\n";
    std::getchar();
    std::cout << "Publishing...\n";

    seed_rand();
    for (long i = 0; i < M; ++i) {
        int coin = rand_int(1, 2);
        if (coin == 1) {
            source_socket.send(command_msg(COMMAND_1), ZMQ_SNDMORE);
            source_socket.send(command_1_args(i + 1, i + 1), 0);
        }
        else {
            source_socket.send(command_msg(COMMAND_2), ZMQ_SNDMORE);
            char a = rand_int<short>(97, 122);
            bool b = rand_int(1, 2) == 1;
            char c = rand_int<short>(97, 122);
            source_socket.send(command_2_args(a, b, c), 0);
        }
    }

    for (long i = 0; i < N; ++i) {
        source_socket.send(command_msg(POISON_PILL), 0);
    }

    std::cout << "Scatter completed...\n";

    sink_socket.close();
	source_socket.close();
    zmq_context.close();

    return 0;
}