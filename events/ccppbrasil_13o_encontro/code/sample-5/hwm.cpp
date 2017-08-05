//Sample provided by Fabio Galuppo  
//August 2017

//compile:
//clang++ -I../include -I./cppzmq/include -L/usr/local/Cellar/zeromq/4.2.2/lib -std=c++11 -lzmq hwm.cpp -o ./bin/hwm.exe

//run:
//./bin/hwm.exe

#include <zmq.h>
#include <zmq.hpp>
#include "zmq_utils.hpp"

#include <iostream>

const size_t N = 1000;

inline size_t send_all_until_block(zmq::socket_t& push) {
    size_t sent = 0;
    while (sent < N && zmq_send(push, nullptr, 0, ZMQ_DONTWAIT) == 0)
        ++sent;
    return sent;
}

inline size_t receive_all(zmq::socket_t& pull) {
    int received = 0;
    while (zmq_recv(pull, nullptr, 0, ZMQ_DONTWAIT) == 0)
        ++received;
    return received;
}

inline void bind(zmq::socket_t& pull, const char* address, int rcv_hwm) {
    pull.setsockopt(ZMQ_RCVHWM, rcv_hwm);
    pull.bind(address);
}

inline void connect(zmq::socket_t& push, const char* address, int snd_hwm) {
    push.setsockopt(ZMQ_SNDHWM, snd_hwm);
    push.connect(address);
}

int main(int argc, char* argv[]) {
    std::cout << "ZMQ version = " << zmq_utils::get_zmq_version() << "\n";

    zmq::context_t context;
    zmq::socket_t pull(context, ZMQ_PULL), push(context, ZMQ_PUSH);
    const char* address = "inproc://hwm";

    bind(pull, address, 0);
    //bind(pull, address, 1);
    //bind(pull, address, 2);

    connect(push, address, 0);
    //connect(push, address, 1);
    //connect(push, address, 2);

    size_t sent = send_all_until_block(push);
    receive_all(pull);

    //0 0 = N; 0 1 = N; 0 2 = N
    //1 0 = N; 1 1 = 2; 1 2 = 3
    //2 0 = N; 2 1 = 3; 2 2 = 4
    std::cout << "ZMQ_RCVHWM: " << pull.getsockopt<int>(ZMQ_RCVHWM) << " ";
    std::cout << "ZMQ_SNDHWM: " << push.getsockopt<int>(ZMQ_SNDHWM) << " ";
    std::cout << "'# of sent messages': " << sent << "\n";

    push.close();
    pull.close();
    context.close();

    return 0;
}