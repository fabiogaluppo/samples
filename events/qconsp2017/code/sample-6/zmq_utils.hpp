//Sample provided by Fabio Galuppo  
//April 2017

#ifndef ZMQ_UTILS_HPP
#define ZMQ_UTILS_HPP

#include <zmq.hpp>
#include <tuple>
#include <string>
#include <sstream>
#include <memory>
#include <array>

namespace zmq_utils {
	std::string get_zmq_version() {
		auto v = zmq::version();
		auto major = std::get<0>(v);
		auto minor = std::get<1>(v);
		auto patch = std::get<2>(v);
		std::stringstream ss;
		ss << major << "." << minor << "." << patch;
		return ss.str();
	}

	void pre_config_socket(zmq::socket_t& socket, int64_t affinity, int linger = 0, int tcp_keep_alive = -1, int snd_hwm = 1000, int rcv_hwm = 1000) {
		socket.setsockopt(ZMQ_AFFINITY, &affinity, sizeof(affinity));
		socket.setsockopt(ZMQ_LINGER, linger);
		socket.setsockopt(ZMQ_TCP_KEEPALIVE, tcp_keep_alive);
		socket.setsockopt(ZMQ_SNDHWM, snd_hwm);
		socket.setsockopt(ZMQ_RCVHWM, rcv_hwm);
	}

	#if defined _WIN32
		using fd_type = SOCKET;
	#else
		using fd_type = int;
	#endif

	inline zmq::pollitem_t make_pollitem(zmq::socket_t& socket, fd_type fd = 0, short events = ZMQ_POLLIN, short revents = 0) {
		return zmq::pollitem_t { static_cast<void*>(socket), fd, events, revents };
	}

	template <size_t N>
	inline int poll(std::array<zmq::pollitem_t, N>& items, long timeout = -1L) {
		return zmq::poll(items.data(), items.size(), timeout);
	}

	template <size_t N>
	inline bool pollin(std::array<zmq::pollitem_t, N>& items, size_t index) {
		return items[index].revents & ZMQ_POLLIN;
	}
}

#endif /* ZMQ_UTILS_HPP */