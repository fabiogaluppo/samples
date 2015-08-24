//Sample provided by Fabio Galuppo 
//August 2015

//compile: cl.exe /Fo.\bin\obj\ /I.\zmq /I.\zmq\zeromq-4.1.2\include /EHsc /Ox libzmq.lib zmq_push.cpp /link /LIBPATH:.\zmq\libs /out:bin\zmq_push.exe
//run: .\bin\zmq_push.exe

#include <zmq.hpp>

#include "randomized_byte_stream.hpp"

#include <iostream>

void push(const char* address, size_t S, size_t N)
{
    zmq::context_t context(1);
	zmq::socket_t  sender(context, ZMQ_PUSH);
	sender.bind(address);

	randomized_byte_stream rbs(S);
    if (N > 0)
    {
        sender.send(rbs.zero(), rbs.size());
		for (int i = 0; i < N; ++i)
			sender.send(rbs.next(), rbs.size());
        std::cout << (N + 1) << " sent to server with size = " << S << ".\n";
    }
}

#include <array>

int main()
{
    const char* server = "tcp://*:5557"; 
    
    std::array<size_t, 1> Ns { 10240 };
    std::array<size_t, 7> Ss { 32, 64, 128, 256, 512, 1024, 2048 };
    
    for (auto N : Ns)
        for (auto S : Ss)
            push(server, S, N);
}