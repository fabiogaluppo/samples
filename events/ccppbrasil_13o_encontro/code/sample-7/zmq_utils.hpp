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

	std::string to_string(const zmq::message_t& msg) {
	    size_t size = msg.size();
	    if (size > 0) {
	        const char* begin = static_cast<const char*>(msg.data());
	        const char* end = begin + size;
	        return std::string(begin, end);
	    }
	    return std::string();
	}

	inline void send_string(zmq::socket_t& socket, const char* msg, int flags = 0) {
	    size_t n = strlen(msg);
	    if (n > 0) {
	        socket.send(zmq::message_t(msg + 0, msg + n), flags);
	    }
	}
}

#endif /* ZMQ_UTILS_HPP */