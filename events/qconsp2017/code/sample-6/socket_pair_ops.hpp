//Sample provided by Fabio Galuppo  
//April 2017

#ifndef SOCKET_PAIR_OPS_HPP
#define SOCKET_PAIR_OPS_HPP

#include "socket_pair.hpp"
#include "socket_ops.hpp"
#include <utility>

namespace socket_pair_ops {
	inline size_t send(socket_pair::socket_pair& sp, zmq::message_t&& msg) {
		return socket_ops::send(sp.get_sender_socket(), std::move(msg));
	}

	inline size_t send_more(socket_pair::socket_pair& sp, zmq::message_t&& msg) {
		return socket_ops::send_more(sp.get_sender_socket(), std::move(msg));
	}

	inline bool try_send(socket_pair::socket_pair& sp, zmq::message_t&& msg) {
		return socket_ops::try_send(sp.get_sender_socket(), std::move(msg));
	}

	inline bool try_send_more(socket_pair::socket_pair& sp, zmq::message_t&& msg) {
		return socket_ops::try_send_more(sp.get_sender_socket(), std::move(msg));
	}

	template <typename... Ts>
	inline socket_ops::result_type bulk_send_with_result(socket_pair::socket_pair& sp, Ts&... args) {
		return socket_ops::bulk_send_with_result(sp.get_sender_socket(), args...);
	}

	template <typename... Ts>
	inline size_t bulk_send(socket_pair::socket_pair& sp, Ts&... args) {
		return socket_ops::bulk_send(sp.get_sender_socket(), args...);
	}

	template <typename... Ts>
	inline bool try_bulk_send(socket_pair::socket_pair& sp, Ts&... args) {
		return socket_ops::try_bulk_send(sp.get_sender_socket(), args...);
	}

	inline size_t receive(socket_pair::socket_pair& sp, zmq::message_t& msg) {
		return socket_ops::receive(sp.get_receiver_socket(), msg);
	}

	inline bool try_receive(socket_pair::socket_pair& sp, zmq::message_t& msg) {
		return socket_ops::try_receive(sp.get_receiver_socket(), msg);
	}

	template <typename... Ts>
	inline socket_ops::result_type bulk_receive_with_result(socket_pair::socket_pair& sp, Ts&... args) {
		return socket_ops::bulk_receive_with_result(sp.get_receiver_socket(), args...);
	}

	template <typename... Ts>
	inline size_t bulk_receive(socket_pair::socket_pair& sp, Ts&... args) {
		return socket_ops::bulk_receive(sp.get_receiver_socket(), args...);
	}

	template <typename... Ts>
	inline bool try_bulk_receive(socket_pair::socket_pair& sp, Ts&... args) {
		return socket_ops::try_bulk_receive(sp.get_receiver_socket(), args...);
	}
}

#endif //SOCKET_PAIR_OPS_HPP