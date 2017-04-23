//Sample provided by Fabio Galuppo  
//April 2017

//compile:
//clang++ -std=c++14 -I../include -I./cppzmq/include -lzmq sender.cpp -o ./bin/sender.exe

//run:
//./bin/sender.exe "tcp://localhost:60000" "tcp://localhost:60001" "DL1"

#include <zmq.hpp>
#include "zmq_utils.hpp"
#include "socket_ops.hpp"

#include <array>
#include <cstring>
#include <string> 
#include <functional>
#include <utility>
#include <thread>
#include <mutex>
#include <atomic>
#include <iostream>
#include <iomanip>
#include <chrono>

namespace sender {
    static std::mutex sync;

    void println(const char* text, const char* label = nullptr) {
        std::lock_guard<std::mutex> _lock(sync);

        std::cout << "[" << std::setw(5) << std::dec << std::uppercase << std::setfill('0') << std::this_thread::get_id() << "] ";
        if (label) std::cout << label << ": ";
        
        std::cout << text << std::endl;
    }

    void println(zmq::message_t& msg, size_t size, const char* label = nullptr) {
		std::lock_guard<std::mutex> _lock(sync);

		std::cout << "[" << std::setw(5) << std::dec << std::uppercase << std::setfill('0') << std::this_thread::get_id() << "] ";
		if (label) std::cout << label << ": ";
		
		unsigned char* ptr = static_cast<unsigned char*>(msg.data());
		for (int i = 0; i < size; ++i) {
			unsigned char ch = *(ptr + i);
			std::cout << std::setw(2) << std::hex << std::uppercase << std::setfill('0') << (0xff & ch) << " ";
		}
		std::cout << std::endl;
	}

    void pause() {
        while (std::cin.get() != 10);
    }

    struct sender : private zmq::monitor_t {
        sender(zmq::context_t& zmq_context, const char* id, const char* send_to_address, 
            std::function<void(zmq::socket_t&)> config_socket = nullptr) 
                : socket(zmq_context, ZMQ_DEALER), connected(false) {
            socket.setsockopt(ZMQ_IDENTITY, id, strlen(id));
            if (config_socket)
                config_socket(socket);
            socket.connect(send_to_address);
            println(send_to_address, "sender address");
            monitor_thread = std::move(std::thread([&](){ 
                monitor(socket, "inproc://socket-monitor", ZMQ_EVENT_ALL); 
            }));
        }

        ~sender() {
            zmq::monitor_t::abort();
            if (monitor_thread.joinable()) monitor_thread.join();
            socket.close();
        }

        const bool is_connected() const { return connected; }

        zmq::socket_t& get_socket() { return socket; }

        private:
            virtual void on_event_connected(const zmq_event_t &event_, const char* addr_) {
                connected = true;
                println("connected...");
            }

            virtual void on_event_disconnected(const zmq_event_t &event_, const char* addr_) {
                connected = false;
                println("disconnected...");
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
            zmq::socket_t socket;
            std::thread monitor_thread;
    };

    zmq::message_t make_payload(int i) {
		std::array<char, 512> xs;
		char* ptr = xs.data();
		*reinterpret_cast<unsigned int*>(ptr + 0) = i;
		return std::move(zmq::message_t(xs.begin(), xs.end()));
	}

    inline std::string to_string(zmq::message_t& msg) {
        char* ptr = static_cast<char*>(msg.data());
        return std::move(std::string(ptr, ptr + msg.size()));
    }

    inline void send_string(zmq::socket_t& socket, const char* msg, int flags = 0) {
        size_t n = strlen(msg);
        if (n > 0) {
            socket.send(zmq::message_t(msg + 0, msg + n), flags);
        }
    }

    void run(int argc, const char* argv[]) {
        if (argc < 4) {
			std::cout << "usage: " << argv[0] << " <connect to send> <connect to receive> <dealer id (length must be 3)>\n";
			return;
		}

        const char* send_to_address = argv[1]; //connect to send
        const char* receive_from_address = argv[2]; //connect to receive
        const char* dealer_id = argv[3]; //dealer id

        std::cout << "ZMQ version = " << zmq_utils::get_zmq_version() << "\n";
        std::cout << "dealer_id = " << dealer_id << "\n";
        std::cout << "[CTRL + C] to finish...\n";

        zmq::context_t zmq_context(2);

        const char* summary_address = "inproc://summary-thread"; 
        std::thread summary_thread([&] {
            size_t sent = 0, received = 0;
            zmq::socket_t socket(zmq_context, ZMQ_PULL);
            socket.bind(summary_address);
            std::array<zmq::pollitem_t, 1> items { zmq_utils::make_pollitem(socket) };
			while (true) {
                int retval = zmq_utils::poll(items, 10 * 1000); //10 s
                if (retval == 0) {
                    std::string summary_text = "sent = " + std::to_string(sent) 
                        + " received = " + std::to_string(received);
                    println(summary_text.c_str());
                }
				else if (zmq_utils::pollin(items, 0)) {
					zmq::message_t msg;
					socket.recv(&msg, 0);
                    std::string smsg = to_string(msg);
                    if (smsg == "sent") {
                        ++sent;
                    } else if (smsg == "received") {
                        ++received;
                    }
				}
			}
            socket.close();
        });
        summary_thread.detach();

        std::thread receiver_thread([&] {
			zmq::socket_t socket(zmq_context, ZMQ_DEALER);
            socket.setsockopt(ZMQ_IDENTITY, dealer_id, strlen(dealer_id));
            zmq_utils::pre_config_socket(socket, 2);
            socket.connect(receive_from_address);
            println(receive_from_address, "receiver address");

            zmq::socket_t summary_socket(zmq_context, ZMQ_PUSH);
            summary_socket.connect(summary_address);
            
            std::array<zmq::pollitem_t, 1> items { zmq_utils::make_pollitem(socket) };
			while (true) {
                int retval = zmq_utils::poll(items, 0);
				if (zmq_utils::pollin(items, 0)) {
					zmq::message_t decoder, payload;
					while (!socket_ops::try_bulk_receive(socket, decoder, payload)){} 
					//println(payload, 8, "payload back");
                    send_string(summary_socket, "received");
				}
			}
            summary_socket.close();
            socket.close();
		});
        receiver_thread.detach();

        //sender scope
        {
            sender sndr(zmq_context, dealer_id, send_to_address, [](zmq::socket_t& socket) {
                zmq_utils::pre_config_socket(socket, 1);
                //additional config here...
            });
            zmq::socket_t summary_socket(zmq_context, ZMQ_PUSH);
            summary_socket.connect(summary_address);

            std::array<char, 1> xs = { '\0' };
            size_t count = 0;
            while (true) {
                if (sndr.is_connected()) {
                    bool status;
                    do {
                        zmq::message_t decoder(xs.begin(), xs.end());
                        zmq::message_t payload = make_payload(++count);
                        println(payload, 8);
                        status = socket_ops::try_bulk_send(sndr.get_socket(), decoder, payload);
                    } while (!status);
                    send_string(summary_socket, "sent");
                    std::this_thread::sleep_for(std::chrono::seconds(1));
                }
            }
            summary_socket.close();
        } //sender destructor called here 

        zmq_context.close();
    }
}

int main(int argc, const char* argv[]) {
    sender::run(argc, argv);

    return 0;
}
