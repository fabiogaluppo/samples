//Sample provided by Fabio Galuppo 
//August 2015

//compile: cl.exe /Fo.\bin\obj\ /I.\zmq /I.\zmq\zeromq-4.1.2\include /EHsc /Ox libzmq.lib zmq_pull.cpp /link /LIBPATH:.\zmq\libs /out:bin\zmq_pull.exe
//run: .\bin\zmq_pull.exe

#include <zmq.hpp>

#include "randomized_byte_stream.hpp"
#include "stop_watch.hpp"

#include <iostream>
#include <iomanip>
#include <queue>
#include <tuple>
#include <utility>

stop_watch sw;

std::queue<std::tuple<size_t, size_t, long long>> summary;

void pull(const char* address, size_t S, size_t N)
{
    zmq::context_t context(1);
	zmq::socket_t  receiver(context, ZMQ_PULL);
	receiver.connect(address);
    
    randomized_byte_stream rbs(S);
    _queue q;    
    for (size_t i = 0; i <= N; ++i)
	{
		receiver.recv(rbs.raw_pointer(), rbs.size());
		if (rbs.zeroed())
		{
			sw.restart();
			continue;
		}

		q.enqueue(rbs);
	}
    
	auto us = sw.elapsed_us().count();
    std::cout << N << " received messages in " << us << " us.\n";
	std::cout << "top message is " << to_string(q.dequeue()) << "\n";
	std::cout << "-----------------------------------------------------------------------\n";
	std::cout.flush();
    
    summary.push(std::make_tuple(S, N, us));
}

#include <array>

int main()
{
    const char* client = "tcp://localhost:5557"; 
    
   std::array<size_t, 1> Ns { 10240 };
   std::array<size_t, 7> Ss { 32, 64, 128, 256, 512, 1024, 2048 };
    
    for (auto N : Ns)
        for (auto S : Ss)
            pull(client, S, N);
    
    while (summary.size() > 0)
    {
        auto x = summary.front();
        summary.pop();
        double N = std::get<0>(x);
        double S = std::get<1>(x);        
        double T = std::get<2>(x);
        double msgs_per_ms = (S / T) * 1000.0;  
        
        std::cout << std::setprecision(0) << std::setw(5) << N << std::setw(8) 
                  << S << std::setw(10) << T << " us" 
                  << std::setw(10) << std::fixed << std::setprecision(4) << msgs_per_ms << " msgs/ms\n";
    }
}