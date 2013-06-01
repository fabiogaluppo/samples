//Sample provided by Fabio Galuppo
//May 2013

#pragma once
#ifndef _tsp_class_
#define _tsp_class_

#include <array>
#include <cmath>
#include <functional>
#include <fstream>
#include <cassert>
#include <utility>

template<int N>
struct tsp_class 
{
	typedef std::pair<int /* x */, int /* y */> city_location;
	typedef std::function<int()> rand_function;

	std::array<city_location, N> cities;

	tsp_class()
	{
	}

	tsp_class(const tsp_class& that)
	{
		cities = that.cities;
	}

	tsp_class(std::ifstream& istr)
	{
		assert (istr.good());
		assert (istr.is_open());

		int n;		
		istr.read(reinterpret_cast<char*>(&n), sizeof(int));

		assert (n == N);

		for(int i = 0; i < N; ++i)
		{
			int x, y;
			istr.read(reinterpret_cast<char*>(&x), sizeof(int))
				.read(reinterpret_cast<char*>(&y), sizeof(int));
			cities[i] = std::make_pair(x, y);
		}		
	}

	auto operator=(const tsp_class& that) -> tsp_class&
	{
		if (this != &that)
			cities = that.cities;
		return *this;
	}
  
	auto do_cycle_lenght() const -> double
	{
		double temp = euclidean_distance(cities[N - 1], cities[0]);

		for (int i = 0, l = N - 1; i < l; ++i)
			temp += euclidean_distance(cities[i], cities[i + 1]);

		return temp;
	}

	//perturb_first: false means don't perturb starting/ending city
	auto do_pertubation(rand_function rnd_city, bool perturb_first) -> void
	{
		int c1, c2;
		do
		{
			c1 = rnd_city();
			do c2 = rnd_city(); while(c1 == c2);
		} while(!perturb_first && (0 == c1 || 0 == c2));

		std::swap(cities[c1], cities[c2]);
	}

	auto save(std::ofstream& ostr) -> void
	{
		assert (ostr.good());
		assert (ostr.is_open());

		int n = N;
		ostr.write(reinterpret_cast<char*>(&n), sizeof(int));
		
		for(auto& city : cities)
			ostr.write(reinterpret_cast<char*>(&city.first), sizeof(int))
			    .write(reinterpret_cast<char*>(&city.second), sizeof(int));

		ostr.flush();
	}

private:
	static auto euclidean_distance(city_location c1, city_location c2) -> double
	{
		auto x1 = c1.first;
		auto y1 = c1.second;
		auto x2 = c2.first;
		auto y2 = c2.second;
		auto dx = std::abs(x1 - x2);
		auto dy = std::abs(y1 - y2);
		return std::sqrt(dx * dx + dy * dy);
	}
};

#include <iostream>

template<int N>
auto display(tsp_class<N>& tsp_instance, bool emit_mathematica_graph_plot = false) -> void
{
	for(auto city: tsp_instance.cities)
		std::cout << "(" << city.first << ", " << city.second << ") : ";
	std::cout << tsp_instance.do_cycle_lenght() << std::endl;
	
	if(emit_mathematica_graph_plot)
		mathematica_graph_plot(tsp_instance);
}

template<int N>
auto mathematica_graph_plot(tsp_class<N>& tsp_instance) -> void
{
	std::cout << "GraphPlot[{";
	for (int i = 0, l = N - 1; i < l; ++i)
		std::cout << i << " -> " << i + 1 << ", ";
	std::cout << N - 1 <<  " -> " << 0 << "}, ";
	std::cout << "VertexCoordinateRules -> {";
	for (int i = 0, l = N - 1; i < l; ++i)
	{
		auto& city = tsp_instance.cities[i];
		std::cout << i << " -> {" << city.first << ", " << city.second << "}, ";
	}
	auto& city = tsp_instance.cities[N - 1];
	std::cout << N - 1 << " -> {" << city.first << ", " << city.second << "}}]" << std::endl;
}

#endif /* _tsp_class_ */