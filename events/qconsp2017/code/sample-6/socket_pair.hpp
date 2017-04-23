//Sample provided by Fabio Galuppo  
//April 2017

#ifndef SOCKET_PAIR_HPP
#define SOCKET_PAIR_HPP

#include <zmq.hpp>
#include <functional>
#include <utility>

namespace socket_pair {
	struct socket_pair final {
		socket_pair(zmq::socket_t sender_socket, zmq::socket_t receiver_socket, std::function<void(socket_pair&)> config_sockets = nullptr) :
			sender_socket(std::move(sender_socket)),
			receiver_socket(std::move(receiver_socket)) {
			if (config_sockets)
				config_sockets(*this);
		}

		socket_pair() = delete;
		socket_pair(const socket_pair&) = delete;
		socket_pair& operator=(const socket_pair&) = delete;

		socket_pair(socket_pair&& that) :
			sender_socket(std::move(that.sender_socket)),
			receiver_socket(std::move(that.receiver_socket)) {
		}

		~socket_pair() {
			close();
		}

		void close() {
			sender_socket.close();
			receiver_socket.close();
		}

		zmq::socket_t& get_sender_socket() {
			return sender_socket;
		}

		zmq::socket_t& get_receiver_socket() {
			return receiver_socket;
		}

		std::string get_id() {
			char id[256]{ '\0' };
			size_t size;
			sender_socket.getsockopt(ZMQ_IDENTITY, id, &size);
			return id;
		}

	private:
		zmq::socket_t sender_socket;
		zmq::socket_t receiver_socket;
	};

	socket_pair make_dealer(zmq::context_t& zmq_context, const char* id, const char* router_to_send_address,
		const char* router_to_receive_address, std::function<void(socket_pair&)> config_sockets) {
		zmq::socket_t dealer_sender_socket(zmq_context, ZMQ_DEALER);
		zmq::socket_t dealer_receiver_socket(zmq_context, ZMQ_DEALER);
		dealer_sender_socket.setsockopt(ZMQ_IDENTITY, id, strlen(id));
		dealer_receiver_socket.setsockopt(ZMQ_IDENTITY, id, strlen(id));
		socket_pair dealer(std::move(dealer_sender_socket), std::move(dealer_receiver_socket), config_sockets);
		dealer.get_sender_socket().connect(router_to_send_address);
		dealer.get_receiver_socket().connect(router_to_receive_address);
		return std::move(dealer);
	}

	socket_pair make_router(zmq::context_t& zmq_context, const char* router_sender_bind_address,
		const char* router_receiver_bind_address, std::function<void(socket_pair&)> config_sockets) {
		zmq::socket_t router_sender_socket(zmq_context, ZMQ_ROUTER);
		zmq::socket_t router_receiver_socket(zmq_context, ZMQ_ROUTER);
		socket_pair router(std::move(router_sender_socket), std::move(router_receiver_socket), config_sockets);
		router.get_sender_socket().bind(router_sender_bind_address);
		router.get_receiver_socket().bind(router_receiver_bind_address);
		return std::move(router);
	}
}

#endif //SOCKET_PAIR_HPP