//Sample provided by Fabio Galuppo
//Compositional TSP Solver version 0.1
//October 2013

#pragma once
#ifndef __tsp_
#define __tsp_

#include "tsp.hpp"
#include "tsp_algo.hpp"

enum struct DisplayFlags 
{
	None = 0,
	EmitMathematicaGraphPlot = 1,
	DisplaySeparator = 2,
	All = EmitMathematicaGraphPlot | DisplaySeparator
};

struct Display
{
	Display(const char* text, DisplayFlags flags = DisplayFlags::None) 
		: Text_(text), DisplayFlags_(flags)
	{
	}
	
	TSP operator()(TSP::T t)
	{
		std::cout << Text_ << ":" << std::endl;
		display(ref(t), has_flag(DisplayFlags::EmitMathematicaGraphPlot));
		if (has_flag(DisplayFlags::DisplaySeparator))
			std::cout << "--------------------------------------------------" << std::endl;
		return TSP(t);
	}

private:
	bool has_flag(DisplayFlags flag)
	{
		return static_cast<unsigned int>(flag) == (static_cast<unsigned int>(DisplayFlags_) & static_cast<unsigned int>(flag));
	}

	const char* Text_;
	DisplayFlags DisplayFlags_;
};

#include <chrono>

struct stop_watch
{
	stop_watch() :
		Start_(now())
	{
	}

	std::chrono::milliseconds elapsed_ms() const
	{
		return std::chrono::duration_cast<std::chrono::milliseconds>(now() - Start_);
	}

	void restart() { Start_ = now(); }

private:
	static std::chrono::high_resolution_clock::time_point now()
	{ 
		return std::chrono::high_resolution_clock::now(); 
	}

	std::chrono::high_resolution_clock::time_point Start_;
};

struct Measure
{
	typedef TSP::transformer_type F;
	
	Measure(F f, F p = Identity()) : AlgoFunc_(f), PrintFunc_(p)
	{
	}

	TSP operator()(TSP::T t)
	{
		stop_watch sw;
		auto result = AlgoFunc_(t);
		auto elapsed_ms = sw.elapsed_ms().count();
		bnd(result, PrintFunc_);
		std::cout << elapsed_ms << " ms" << std::endl;
		std::cout << "--------------------------------------------------" << std::endl;
		return result;
	}

private:
	F AlgoFunc_, PrintFunc_;
};

#include <cstdlib>

tsp_class read_tsp_instance()
{
	tsp_class tsp_instance;

	//read an instance of tsp
	int N;
	std::cin >> N;
	if (N > 1)
	{
		int city_number = 0;
		for(int i = 0; i < N; ++i)
		{
			int x, y;
			std::cin >> x >> y;
			tsp_class::city_info info = { ++city_number, std::make_pair(x, y) };
			tsp_instance.cities.push_back(info);
		}
	}
	else
	{
		std::cerr << "N must be greater than 1" << std::endl;
		std::exit(1);
	}

	return tsp_instance;
}

#include "_tsp_support.hpp"

#endif /* __tsp_ */