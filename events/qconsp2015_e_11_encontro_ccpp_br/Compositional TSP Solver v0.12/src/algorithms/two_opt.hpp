//Sample provided by Fabio Galuppo
//Compositional TSP Solver version 0.12
//March 2015

#ifndef _two_opt_
#define _two_opt_

#include "../tsp.hpp"

#include <algorithm>
#include <cmath>

tsp_class two_opt_swap(const tsp_class& tsp_instance, 
					   tsp_class::cities_type::size_type i, 
					   tsp_class::cities_type::size_type k)
{
	tsp_class temp_tsp_instance = tsp_instance;
	std::reverse(temp_tsp_instance.cities.begin() + i, temp_tsp_instance.cities.begin() + k + 1);
	return temp_tsp_instance;
}

#define DISTANCE_COMPARISON(v) static_cast<long>(std::floor(10000 * (v)))

void two_opt_all(tsp_class& tsp_instance)
{
	bool can_continue;
	do
	{
		can_continue = false;
		auto best_distance = DISTANCE_COMPARISON(tsp_instance.do_cycle_length());
	
		for(tsp_class::cities_type::size_type i = 1; i < tsp_instance.cities.size() - 1; ++i)
		{
			for(tsp_class::cities_type::size_type k = i + 1; k < tsp_instance.cities.size(); ++k)
			{
				auto new_route = two_opt_swap(tsp_instance, i, k);
				auto new_distance = DISTANCE_COMPARISON(new_route.do_cycle_length());
				if (new_distance < best_distance)
				{
					tsp_instance = new_route;
					can_continue = true;
					break;
				}
			}

			if (can_continue) break;
		}
	} while(can_continue);
}

#endif /* _two_opt_ */