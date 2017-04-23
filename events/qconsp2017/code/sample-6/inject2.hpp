//Sample provided by Fabio Galuppo  
//April 2017

#ifndef INJECT2_HPP
#define INJECT2_HPP

#include <zmq.hpp>
#include "zmq_utils.hpp"
#include "socket_pair.hpp"
#include "socket_pair_ops.hpp"

#include <chrono>
#include <thread>
#include <atomic>
#include <vector>
#include <array>
#include <iostream>
#include <iomanip>
#include <utility>
#include <algorithm>

namespace inject2 {
	static inline std::chrono::high_resolution_clock::time_point now() {
        return std::chrono::high_resolution_clock::now();
    }

    static inline std::chrono::microseconds to_us(const std::chrono::duration<long long, std::nano>& t) {
        return std::chrono::duration_cast<std::chrono::microseconds>(t);
    }

    static inline std::chrono::milliseconds to_ms(const std::chrono::duration<long long, std::nano>& t) {
        return std::chrono::duration_cast<std::chrono::milliseconds>(t);
    }

    zmq::message_t make_payload(int i, std::chrono::high_resolution_clock::time_point timestamp) {
		std::array<char, 512> xs;
        char* ptr = xs.data();
		*reinterpret_cast<unsigned int*>(ptr + 0) = i;
		*reinterpret_cast<std::chrono::high_resolution_clock::time_point*>(ptr + 4) = timestamp;
		return std::move(zmq::message_t(xs.begin(), xs.end()));
	}

	void run(int argc, const char* argv[]) {
		if (argc < 5 || std::strlen(argv[4]) != 3) {
			std::cout << "usage: " << argv[0] << " <connect to send> <connect to receive> <number of messages> <dealer id (length must be 3)> <transactions per second (optional)>\n";
			return;
		}

		const char* router_to_send_address = argv[1]; //connect to send
		const char* router_to_receive_address = argv[2]; //connect to receive
		int N = std::atoi(argv[3]); //number of messages
		const char* dealer_id = argv[4]; //dealer id
        size_t TPS = argc > 5 ? std::atoi(argv[5]) : 0; //transactions per second

        std::atomic<int> M; //N
        std::string rtt_info;

		std::cout << "ZMQ version = " << zmq_utils::get_zmq_version() << "\n";
		std::cout << "dealer_id = " << dealer_id << "\n";
        std::string info_TPS = "TPS = " + (TPS > 0 ? std::to_string(TPS) : std::string("BURST..."));
        std::cout << info_TPS << "\n";

        std::chrono::high_resolution_clock::time_point start, finish;

		zmq::context_t zmq_context(2);

        std::vector<std::chrono::nanoseconds> time_xs;
        time_xs.resize(N);
        std::fill(time_xs.begin(), time_xs.end(), std::chrono::high_resolution_clock::duration::zero());

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
				long timeout_ms = 10 * 1000; //if 10 s timeout, messages were lost
                int retval = zmq_utils::poll(items, timeout_ms);
                if (retval == 0) {
                    finish = now() - std::chrono::milliseconds(timeout_ms);
                    M = counter;
                    rtt_info = "rtt NOT completed (received) = " + std::to_string(counter) + 
                                std::string(" (missing) = ") + std::to_string(N - counter);
                    std::cout << rtt_info << "\n";
                    return;
                }
				if (zmq_utils::pollin(items, 0)) {
					zmq::message_t decoder, payload;
					while (!socket_pair_ops::try_bulk_receive(dealer, decoder, payload)); 
					auto _now_ = now();
                    char* ptr = reinterpret_cast<char*>(payload.data());
                    int i = *reinterpret_cast<int*>(ptr + 0);
                    auto t0 = *reinterpret_cast<std::chrono::high_resolution_clock::time_point*>(ptr + 4);
                    time_xs[i] = _now_ - t0;
					++counter;
				}
			}
            finish = now();
            M = N;
            rtt_info = "rtt completed = " + std::to_string(counter);
            std::cout << rtt_info << "\n"; 
		});

		std::array<char, 1> xs = { '\0' };
        start = now();		
		for (int i = 0; i < N; ++i) {
			bool status;
            auto _start_ = now();
			do {
                zmq::message_t decoder(xs.begin(), xs.end());
			    zmq::message_t payload = make_payload(i, _start_);
                status = socket_pair_ops::try_bulk_send(dealer, decoder, payload);
            } while (!status);
            while (TPS > 0) {
                auto _finish_ = now();
                auto delta = to_us(_finish_ - _start_);
                if (!(delta < std::chrono::microseconds(1000000 / TPS) /* TPS in us */))
                    break;
            }
		}

		receiver_thread.join();

		auto total = finish - start;

        //report
        std::cout << "[" << to_us(time_xs[0]).count() << " us] ";
        for (size_t i = 1; i < N; ++i) {
            auto t = to_us(time_xs[i] - time_xs[i - 1]);
            std::cout << "[+" << std::max(t.count(), 0LL) << " us] ";
        }
        std::cout << "\n";
        std::cout << std::setw(10) << std::setfill(' ') << to_ms(total).count() << " ms total elapsed [roundtrip in milliseconds]\n";
        std::cout << std::setw(10) << std::setfill(' ') << to_us(total).count() << " us total elapsed [roundtrip in microseconds]\n";
        if (M > 0) {
            auto acc = time_xs[0];
            for (size_t i = 1; i < time_xs.size(); ++i) {
                auto t = time_xs[i] - time_xs[i - 1];
                acc += (t.count() > 0LL ? t : std::chrono::high_resolution_clock::duration::zero()); 
            }
            auto avg_per_msg = std::chrono::duration<long long, std::nano>(acc.count() / M);
            auto sep = std::string(75, '-');
            std::cout << sep << "\n"; 
            std::cout << std::setw(10) << std::setfill(' ') << to_us(avg_per_msg).count() << " us mean  latency [roundtrip in microseconds]\n";
            std::cout << sep << "\n";
            std::cout << info_TPS << "\n";
            std::cout << rtt_info << "\n";
        }

		dealer.close();
		zmq_context.close();
	}
}

#endif /* INJECT2_HPP */