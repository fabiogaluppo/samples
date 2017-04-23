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
}

#endif /* ZMQ_UTILS_HPP */