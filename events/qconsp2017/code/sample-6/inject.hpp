//Sample provided by Fabio Galuppo  
//April 2017

#ifndef INJECT_HPP
#define INJECT_HPP

#include <zmq.hpp>
#include "zmq_utils.hpp"
#include "socket_pair.hpp"
#include "socket_pair_ops.hpp"

#include <iostream>
#include <iomanip>
#include <string>
#include <vector>
#include <cstdlib>
#include <cstring>
#include <array>
#include <memory>
#include <utility>
#include <thread>
#include <mutex>

namespace inject {
	static std::mutex sync;
	
	void display_bytes(zmq::message_t& msg, size_t size, const char* label = nullptr) {
		std::lock_guard<std::mutex> _lock(sync);

		std::cout << "[" << std::setw(5) << std::dec << std::uppercase << std::setfill('0') << std::this_thread::get_id() << "] ";
		if (label) std::cout << label << ": ";
		
		unsigned char* ptr = static_cast<unsigned char*>(msg.data());
		for (int i = 0; i < size; ++i) {
			unsigned char ch = *(ptr + i);
			std::cout << std::setw(2) << std::hex << std::uppercase << std::setfill('0') << (0xff & ch) << " ";
		}
		std::cout << "\n";
	}

	void pause() {
		while (std::cin.get() != 10);
	}
	
	zmq::message_t make_payload(int i) {
		std::array<char, 512> xs;
		char* ptr = xs.data();
		*reinterpret_cast<unsigned int*>(ptr + 0) = i;
		*reinterpret_cast<unsigned int*>(ptr + 4) = 0xFFFFFFFF;
		*reinterpret_cast<unsigned int*>(ptr + 8) = 0xDEADC0DE;
		*reinterpret_cast<unsigned int*>(ptr + 12) = 0xFFFFFFFF;
		return std::move(zmq::message_t(xs.begin(), xs.end()));
	}

	void run(int argc, const char* argv[]) {
		if (argc != 5 || std::strlen(argv[4]) != 3) {
			std::cout << "usage: " << argv[0] << " <connect to send> <connect to receive> <number of messages> <dealer id (length must be 3)>\n";
			return;
		}

		const char* router_to_send_address = argv[1]; //connect to send
		const char* router_to_receive_address = argv[2]; //connect to receive
		int N = std::atoi(argv[3]); //number of messages
		const char* dealer_id = argv[4]; //dealer id

		std::cout << "ZMQ version = " << zmq_utils::get_zmq_version() << "\n";
		std::cout << "dealer_id = " << dealer_id << "\n";
		std::cout << "Enter to finish...\n";

		zmq::context_t zmq_context(2);

		socket_pair::socket_pair dealer = socket_pair::make_dealer(zmq_context, dealer_id, router_to_send_address, router_to_receive_address, [](socket_pair::socket_pair& sp) {
			auto& sender_socket = sp.get_sender_socket();
			auto& receiver_socket = sp.get_receiver_socket();
			zmq_utils::pre_config_socket(sender_socket, 1);
			zmq_utils::pre_config_socket(receiver_socket, 2);
			//additional config here...
		});

		std::thread receiver_thread([&] {
			std::array<zmq::pollitem_t, 1> items { zmq_utils::make_pollitem(dealer.get_receiver_socket()) };
			int counter = 0;
			while (counter < N) {
				int retval = zmq_utils::poll(items, 0);
				if (zmq_utils::pollin(items, 0)) {
					zmq::message_t decoder, payload;
					while (!socket_pair_ops::try_bulk_receive(dealer, decoder, payload)); 
					std::string label = "payload back " + std::to_string(counter + 1);
					display_bytes(payload, 16, label.c_str());
					++counter;
				}
			}
		});

		std::array<char, 1> xs = { '\0' };		
		for (int i = 0; i < N; ++i) {
			bool status;
            do {
                zmq::message_t decoder(xs.begin(), xs.end());
			    zmq::message_t payload = make_payload(i + 1);
				display_bytes(payload, 16, "payload");
                status = socket_pair_ops::try_bulk_send(dealer, decoder, payload);
            } while (!status);
		}

		receiver_thread.join();

		pause();

		dealer.close();
		zmq_context.close();
	}
}

#endif /* INJECT_HPP */