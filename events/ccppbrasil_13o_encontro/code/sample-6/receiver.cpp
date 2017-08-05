//Sample provided by Fabio Galuppo  
//August 2017

//compile:
//clang++ -I../include -I./random_util/include -I./cppzmq/include -L/usr/local/Cellar/zeromq/4.2.2/lib -std=c++14 -lzmq receiver.cpp -o ./bin/receiver.exe

//run:
//./bin/receiver.exe

#include <zmq.h>
#include <zmq.hpp>

#include "zmq_utils.hpp"
#include "random_int_util.hpp"

#include <vector>
#include <array>
#include <string>
#include <iostream>
#include <mutex>
#include <future>
#include <thread>
#include <chrono>
#include <utility>
#include <signal.h>

volatile bool running = true;

std::mutex sync;

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

    std::vector<std::future<bool>> tasks;
    zmq::message_t id_msg, data_msg;
    while (running) {
        const long timeout_ms = 5 * 1000;
        try {
            if (zmq::poll(items.data(), items.size(), timeout_ms)) {
                {
                    std::lock_guard<std::mutex> _lock(sync);
                    router.recv(&id_msg, 0);
                    router.recv(&data_msg, 0);    
                }
                std::cout << ".";
                std::cout.flush();
                std::string id = zmq_utils::to_string(id_msg);
                std::string data = zmq_utils::to_string(data_msg);
                tasks.push_back(
                    std::async([&, id{std::move(id)}, data{std::move(data)}]() {
                        zmq::message_t id_msg_{id.begin(), id.end()};
                        zmq::message_t data_msg_{data.rbegin(), data.rend()};
                        std::this_thread::sleep_for(std::chrono::seconds(rand_int(1, 5)));
                        {
                            std::lock_guard<std::mutex> _lock(sync);
                            try {
                                router.send(id_msg_, ZMQ_SNDMORE);
                                router.send(data_msg_, 0);
                            } catch (const zmq::error_t&) {
                                return false;
                            }
                        }
                        return true;
                    })
                );
            }
        } catch (const zmq::error_t& err) {
            std::cout << "[ERROR] " << err.what() << "\n";
        } 
    }

    for (auto& f : tasks) f.wait();
    router.close();
    context.close();

    return 0;
}