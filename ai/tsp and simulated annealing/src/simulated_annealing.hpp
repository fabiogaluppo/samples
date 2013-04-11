//Sample provided by Fabio Galuppo
//March 2013

#pragma once
#ifndef _simulated_annealing_
#define _simulated_annealing_

#include "tsp.hpp"

#include <cmath>

template<int N>
auto simulated_annealing(const double initial_temperature,
						 const double stopping_criteria_temperature,
						 const double decreasing_factor, 
						 const int monte_carlo_steps,
						 bool perturb_first,
						 tsp_class<N>& tsp_instance, 
						 typename tsp_class<N>::rand_function rnd_tsp,
						 std::function<double()> rnd_probability) -> void
{
	double temperature = initial_temperature;
	
	while(temperature > stopping_criteria_temperature) 
	{
		double cycle_length = tsp_instance.do_cycle_lenght();
		tsp_class<N> temp_tsp_instance = tsp_instance;
    
		int i = monte_carlo_steps;
		while(i-- > 0)
		{
			temp_tsp_instance.do_pertubation(rnd_tsp, perturb_first);	
			double temp_cycle_length = temp_tsp_instance.do_cycle_lenght();

			double dE = temp_cycle_length - cycle_length;

			bool update = false;
			if(dE < 0.0)
			{
				update = true;
			} 
			else
			{
				//Inferior solution can be allowed to move from local optimal solution
				//using probability of acceptance based on Boltzman's function
				auto boltzmanFunction = std::exp(-dE / temperature);
				auto acceptanceProbability = rnd_probability();
				if (boltzmanFunction > acceptanceProbability)
				{
					update = true;
				}
			}

			if(update)
			{
				tsp_instance = temp_tsp_instance;
				cycle_length = temp_cycle_length;
			}
		}

		temperature *= decreasing_factor;
	}
}

#endif /* _simulated_annealing_ */