//Sample provided by Fabio Galuppo  
//April 2017

#ifndef SOCKET_OPS_HPP
#define SOCKET_OPS_HPP

#include <zmq.hpp>
#include <vector>
#include <string>
#include <tuple>
#include <utility>
#include <thread>
#include <chrono>

namespace socket_ops {
	namespace internal {
		size_t send(zmq::socket_t& sender_socket, zmq::message_t& msg, int flags) {
			const size_t MAX_RETRIES = 20; //TODO: Config
			size_t retry_counter = 0;
			bool retry;
			do {
				retry = false;
				size_t msg_size = msg.size();
				if (sender_socket.send(msg, flags))
					return msg_size;
				if (retry_counter++ < MAX_RETRIES) {
					retry = true;
					std::this_thread::sleep_for(std::chrono::microseconds(100)); //TODO: Config
				}
			} while (retry);
			return 0;
		}

		size_t recv(zmq::socket_t& receiver_socket, zmq::message_t& msg, int flags) {
			if (receiver_socket.recv(&msg, flags))
				return msg.size();
			return 0;
		}
	}

	inline size_t send(zmq::socket_t& sender_socket, zmq::message_t msg) {
		return internal::send(sender_socket, msg, ZMQ_DONTWAIT);
	}

	inline size_t send_more(zmq::socket_t& sender_socket, zmq::message_t msg) {
		return internal::send(sender_socket, msg, ZMQ_DONTWAIT | ZMQ_SNDMORE);
	}

	inline bool try_send(zmq::socket_t& sender_socket, zmq::message_t msg) {
		bool sent = internal::send(sender_socket, msg, ZMQ_DONTWAIT) > 0;
		return sent;
	}

	inline bool try_send_more(zmq::socket_t& sender_socket, zmq::message_t msg) {
		bool sent = internal::send(sender_socket, msg, ZMQ_DONTWAIT | ZMQ_SNDMORE) > 0;
		return sent;
	}

	namespace internal {
		template <typename T, typename... Ts>
		struct bulk_sender {
			static void send(zmq::socket_t& sender_socket, size_t& total_bytes_sent, bool& success, T& data, Ts&... args) {
				size_t n = success ? socket_ops::send_more(sender_socket, std::move(data)) : 0;
				total_bytes_sent += n;
				success = success && n > 0;
				bulk_sender<Ts...>::send(sender_socket, total_bytes_sent, success, args...);
			}
		};

		template <typename T>
		struct bulk_sender<T> {
			static void send(zmq::socket_t& sender_socket, size_t& total_bytes_sent, bool& success, T& data) {
				size_t n = success ? socket_ops::send(sender_socket, std::move(data)) : 0;
				total_bytes_sent += n;
				success = success && n > 0;
			}
		};
	}

	using result_type = std::tuple<bool, size_t>;

	namespace result {
		inline bool failed(result_type result) {
			return !std::get<0>(result);
		}

		inline bool succeeded(result_type result) {
			return std::get<0>(result);
		}

		inline size_t value(result_type result) {
			return std::get<1>(result);
		}
	}

	template <typename... Ts>
	inline result_type bulk_send_with_result(zmq::socket_t& sender_socket, Ts&... args) {
		size_t total_bytes_sent = 0;
		bool success = true;
		internal::bulk_sender<Ts...>::send(sender_socket, total_bytes_sent, success, args...);
		return std::make_tuple(success, total_bytes_sent);
	}

	template <typename... Ts>
	inline size_t bulk_send(zmq::socket_t& sender_socket, Ts&... args) {
		//this method ignores partial failure
		result_type result = bulk_send_with_result(sender_socket, args...);
		return result::value(result);
	}

	template <typename... Ts>
	inline bool try_bulk_send(zmq::socket_t& sender_socket, Ts&... args) {
		result_type result = bulk_send_with_result(sender_socket, args...);
		bool sent = result::succeeded(result);
		return sent;
	}

	inline size_t receive(zmq::socket_t& receiver_socket, zmq::message_t& msg) {
		int64_t more;
		size_t more_size = sizeof(more);
		receiver_socket.getsockopt(ZMQ_RCVMORE, &more, &more_size);
		return internal::recv(receiver_socket, msg, ZMQ_DONTWAIT);
	}

	inline bool try_receive(zmq::socket_t& receiver_socket, zmq::message_t& msg) {
		bool received = receive(receiver_socket, msg) > 0;
		return received;
	}
	
	namespace internal {
		template <typename T, typename... Ts>
		struct bulk_receiver {
			static void receive(zmq::socket_t& receiver_socket, size_t& total_bytes_received, bool& success, T& data, Ts&... args) {
				size_t n = success ? socket_ops::receive(receiver_socket, data) : 0;
				total_bytes_received += n;
				success = success && n > 0;
				bulk_receiver<Ts...>::receive(receiver_socket, total_bytes_received, success, args...);
			}
		};

		template <typename T>
		struct bulk_receiver<T> {
			static void receive(zmq::socket_t& receiver_socket, size_t& total_bytes_received, bool& success, T& data) {
				size_t n = success ? socket_ops::receive(receiver_socket, data) : 0;
				total_bytes_received += n;
				success = success && n > 0;
			}
		};
	}

	template <typename... Ts>
	inline result_type bulk_receive_with_result(zmq::socket_t& receiver_socket, Ts&... args) {
		size_t total_bytes_received = 0;
		bool success = true;
		internal::bulk_receiver<Ts...>::receive(receiver_socket, total_bytes_received, success, args...);
		return std::make_tuple(success, total_bytes_received);
	}

	template <typename... Ts>
	inline size_t bulk_receive(zmq::socket_t& receiver_socket, Ts&... args) {
		//this method ignores partial failure
		result_type result = bulk_receive_with_result(receiver_socket, args...);
		return result::value(result);
	}

	template <typename... Ts>
	inline bool try_bulk_receive(zmq::socket_t& receiver_socket, Ts&... args) {
		result_type result = bulk_receive_with_result(receiver_socket, args...);
		bool sent = result::succeeded(result);
		return sent;
	}
}

#endif //SOCKET_OPS_HPP