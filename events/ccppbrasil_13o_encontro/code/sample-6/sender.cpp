//Sample provided by Fabio Galuppo  
//August 2017

//compile:
//clang++ -I../include -I./random_util/include -I./cppzmq/include -L/usr/local/Cellar/zeromq/4.2.2/lib -std=c++14 -lzmq sender.cpp -o ./bin/sender.exe

//run:
//./bin/sender.exe "ID1"

#include <zmq.h>
#include <zmq.hpp>
#include "zmq_utils.hpp"

#include <utility>
#include <thread>
#include <atomic>
#include <iostream>

struct socket_monitor : private zmq::monitor_t {
    socket_monitor(zmq::socket_t& socket) {
        monitor_thread = std::move(std::thread([&](){ 
            monitor(socket, "inproc://socket-monitor", ZMQ_EVENT_ALL); 
        }));
        monitor_thread.detach();
    }

    ~socket_monitor() {
        zmq::monitor_t::abort();
    }

    const bool is_connected() const { return connected; }

    private:
        virtual void on_event_connected(const zmq_event_t &event_, const char* addr_) {
            connected = true;
            std::cout << "connected..." << std::endl;
        }

        virtual void on_event_disconnected(const zmq_event_t &event_, const char* addr_) {
            connected = false;
            std::cout << "disconnected..." << std::endl;
        }

        //http://api.zeromq.org/4-2:zmq-socket-monitor
        //supported events:
        //virtual void on_monitor_started() {}
        //virtual void on_event_connected(const zmq_event_t &event_, const char* addr_) { (void)event_; (void)addr_; }
        //virtual void on_event_connect_delayed(const zmq_event_t &event_, const char* addr_) { (void)event_; (void)addr_; }
        //virtual void on_event_connect_retried(const zmq_event_t &event_, const char* addr_) { (void)event_; (void)addr_; }
        //virtual void on_event_listening(const zmq_event_t &event_, const char* addr_) { (void)event_; (void)addr_; }
        //virtual void on_event_bind_failed(const zmq_event_t &event_, const char* addr_) { (void)event_; (void)addr_; }
        //virtual void on_event_accepted(const zmq_event_t &event_, const char* addr_) { (void)event_; (void)addr_; }
        //virtual void on_event_accept_failed(const zmq_event_t &event_, const char* addr_) { (void)event_; (void)addr_; }
        //virtual void on_event_closed(const zmq_event_t &event_, const char* addr_) { (void)event_; (void)addr_; }
        //virtual void on_event_close_failed(const zmq_event_t &event_, const char* addr_) { (void)event_; (void)addr_; }
        //virtual void on_event_disconnected(const zmq_event_t &event_, const char* addr_) { (void)event_; (void)addr_; }
        //virtual void on_event_handshake_failed(const zmq_event_t &event_, const char* addr_) { (void) event_; (void) addr_; }
        //virtual void on_event_handshake_succeed(const zmq_event_t &event_, const char* addr_) { (void) event_; (void) addr_; }
        //virtual void on_event_unknown(const zmq_event_t &event_, const char* addr_) { (void)event_; (void)addr_; }

    private:
        std::atomic<bool> connected;
        std::thread monitor_thread;
};

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
    
    socket_monitor m(dealer);
    while (true) {
        try {
            if (m.is_connected()) {
                zmq_utils::send_string(dealer, "SOME DATA");
                zmq::message_t msg;
                size_t size = 0;
                while (m.is_connected() && size == 0)
                    size = dealer.recv(&msg, ZMQ_DONTWAIT);
                if (size > 0)
                    std::cout << zmq_utils::to_string(msg) << "\n";
            }
        } catch (const zmq::error_t& err) {
            std::cout << "[ERROR] " << err.what() << "\n";
        }
    }
    
    //dealer.close();
    //context.close(); 
    
    return 0;
}