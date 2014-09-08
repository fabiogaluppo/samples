//Sample provided by Fabio Galuppo
//Compositional TSP Solver version 0.11
//September 2014

#pragma once
#ifndef _nearest_neighbour_
#define _nearest_neighbour_

#include "..\tsp.hpp"

#include <algorithm>

void nearest_neighbour(tsp_class& tsp_instance)
{
	auto xs = tsp_instance.cities;
	tsp_instance.cities.clear();
		
	auto x = *xs.begin();
	tsp_instance.cities.push_back(x);
	xs.erase(xs.begin());
	
	while(xs.size() > 0)
	{
		auto nearest = std::min_element(xs.begin(), xs.end(), [&x](const tsp_class::city_info& i, const tsp_class::city_info& j) {
			return tsp_class::euclidean_distance(x.location, i.location) < tsp_class::euclidean_distance(x.location, j.location);
		});

		x = *nearest;
		tsp_instance.cities.push_back(x);
		xs.erase(nearest);
	}
}

#endif /* _nearest_neighbour_ */