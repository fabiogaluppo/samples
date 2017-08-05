//Sample provided by Fabio Galuppo  
//April 2017

#ifndef ZMQ_UTILS_HPP
#define ZMQ_UTILS_HPP

#include <zmq.hpp>
#include <tuple>
#include <string>
#include <sstream>

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

	inline void set_affinity(zmq::socket_t& socket, int64_t affinity) {
    	socket.setsockopt(ZMQ_AFFINITY, &affinity, sizeof(affinity));
	}

	inline std::string to_string(zmq::message_t& msg) {
		char* ptr = static_cast<char*>(msg.data());
		return std::move(std::string(ptr, ptr + msg.size()));
	}
}

#endif /* ZMQ_UTILS_HPP */