//Sample provided by Fabio Galuppo  
//April 2017

#ifndef common_hpp
#define common_hpp

#include <zmq.hpp>
#include <msgpack.hpp>
#include <string>
#include <cstring>
#include <sstream>
#include <memory>

#include "zmq_utils.hpp"

enum COMMAND : int {
    POISON_PILL = 0,
    COMMAND_1 = 1,
    COMMAND_2 = 2,
};

inline zmq::message_t encode_command_args(std::stringstream& buffer) {
    buffer.seekg(0);
    std::string s = buffer.str();
    return std::move(zmq::message_t(s.begin(), s.end()));
}

inline msgpack::object decode_command_args(zmq::message_t& msg) {
    std::string s = zmq_utils::to_string(msg);
    return msgpack::unpack(s.data(), s.size()).get();
}

inline zmq::message_t command_1_args(int a, int b) {
    std::stringstream buffer;
    msgpack::pack(buffer, msgpack::type::tuple<int, int>(a, b));
    return encode_command_args(buffer);
}

inline msgpack::type::tuple<int, int> command_1_args(zmq::message_t& msg) {
    msgpack::type::tuple<int, int> data;
    decode_command_args(msg).convert(data);
    return data;
}

inline zmq::message_t command_2_args(char a, bool b, char c) {
    std::stringstream buffer;
    msgpack::pack(buffer, msgpack::type::tuple<char, bool, char>(a, b, c));
    return encode_command_args(buffer);
}

inline msgpack::type::tuple<char, bool, char> command_2_args(zmq::message_t& msg) {
    msgpack::type::tuple<char, bool, char> data;
    decode_command_args(msg).convert(data);
    return data;
}

inline zmq::message_t command_msg(COMMAND cmd) {
    auto scmd = std::to_string(static_cast<int>(cmd));
    return std::move(zmq::message_t(scmd.begin(), scmd.end()));
}

inline bool is_command(std::string& scmd, COMMAND cmd) {
    return static_cast<COMMAND>(std::stoi(scmd)) == cmd;
}

#endif /* common_hpp */