//Sample provided by Fabio Galuppo
//Compositional TSP Solver version 0.1
//October 2013

#pragma once
#ifndef _tsp_class_
#define _tsp_class_

#include <vector>
#include <string>

#include <cmath>
#include <functional>

struct tsp_class
{
	typedef std::pair<int /* x */, int /* y */> city_location;
	typedef std::function<int()> rand_function;
	
	struct city_info
	{
		int id;
		city_location location;
	};
	
	typedef std::vector<city_info> cities_type;
	
	typedef std::wstring description_type;

	cities_type cities;
	description_type description;

	//Semiregular:
	tsp_class()
	{
	}

	tsp_class(const tsp_class& that)
	{
		cities = that.cities;
	}

	tsp_class& operator=(const tsp_class& that)
	{
		if (this != &that)
			cities = that.cities;
		return *this;
	}

	~tsp_class()
	{
	}

	//Regular:
	friend bool operator==(const tsp_class& lhs, const tsp_class& rhs) 
	{
		if (lhs.cities.size() == rhs.cities.size())
		{
			auto x = lhs.cities.cbegin();
			auto y = rhs.cities.cbegin();

			while(x != lhs.cities.cend() || y != rhs.cities.cend())
			{
				bool a = x->id != y->id,
					 b = x->location.first != y->location.first,
					 c = x->location.second != y->location.second;

				if (a || b || c) 
					return false;
				
				++x; 
				++y;
			}
		
			return true;
		}

		return false;
	}
	
	friend bool operator!=(const tsp_class& lhs, const tsp_class& rhs)
	{
		 return !(lhs == rhs);
	}

	//
	double do_cycle_length() const
	{
		cities_type::size_type N = cities.size();

		double temp = euclidean_distance(cities[N - 1].location, cities[0].location);		

		for (int i = 0, l = N - 1; i < l; ++i)
			temp += euclidean_distance(cities[i].location, cities[i + 1].location);

		return temp;
	}

	//perturb_first: false means don't perturb starting/ending city
	void do_pertubation(rand_function rnd_city, bool perturb_first)
	{
		int c1, c2;
		do
		{
			c1 = rnd_city();
			do c2 = rnd_city(); while(c1 == c2);
		} while(!perturb_first && (0 == c1 || 0 == c2));

		std::swap(cities[c1], cities[c2]);
	}

	static double euclidean_distance(city_location c1, city_location c2)
	{
		auto x1 = c1.first;
		auto y1 = c1.second;
		auto x2 = c2.first;
		auto y2 = c2.second;
		auto dx = std::abs(x1 - x2);
		auto dy = std::abs(y1 - y2);
		return std::sqrt(static_cast<double>(dx * dx + dy * dy));
	}
};

#include <iostream>

void mathematica_graph_plot(tsp_class& tsp_instance, bool vertexLabeling = false, bool plotLabel = true)
{
	auto LEN = tsp_instance.cities.size() - 1;

	std::cout << "GraphPlot[{";
	for (int i = 0, l = LEN; i < l; ++i)
		std::cout << i << " -> " << i + 1 << ", ";
	std::cout << LEN <<  " -> " << 0 << "}, ";

	if (plotLabel) 
		std::cout << "PlotLabel -> " << "\"" << tsp_instance.do_cycle_length() <<  "\", ";

	std::cout << "Frame -> True, ";

	std::cout << "VertexLabeling -> " <<  (vertexLabeling ? "True" : "False") <<  ", ";

	std::cout << "VertexCoordinateRules -> {";
	for (int i = 0, l = LEN; i < l; ++i)
	{
		auto& city = tsp_instance.cities[i];
		std::cout << i << " -> {" << city.location.first << ", " << city.location.second << "}, ";
	}
	auto& city = tsp_instance.cities[LEN];
	std::cout << LEN << " -> {" << city.location.first << ", " << city.location.second << "}}]" << std::endl;
}

void display(tsp_class& tsp_instance, bool emit_mathematica_graph_plot = false)
{
	for(auto city = tsp_instance.cities.cbegin(); city != tsp_instance.cities.cend(); ++city)
		std::cout << "[" << city->id << "]" << "(" << city->location.first << ", " << city->location.second << ") : ";
	std::cout << tsp_instance.do_cycle_length() << std::endl;
	
	if(emit_mathematica_graph_plot)
		mathematica_graph_plot(tsp_instance);
}

#endif /* _tsp_class_ */