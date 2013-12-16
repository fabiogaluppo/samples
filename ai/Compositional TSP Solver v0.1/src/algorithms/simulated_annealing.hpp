//Sample provided by Fabio Galuppo
//Compositional TSP Solver version 0.1
//October 2013

#pragma once
#ifndef _simulated_annealing_
#define _simulated_annealing_

#include "..\tsp.hpp"

#include <cmath>

void simulated_annealing(const double initial_temperature,
						 const double stopping_criteria_temperature,
						 const double decreasing_factor, 
						 const int monte_carlo_steps,
						 tsp_class& tsp_instance, 
						 tsp_class::rand_function rnd_tsp,
						 std::function<double()> rnd_probability)
{
	double temperature = initial_temperature;
	
	while(temperature > stopping_criteria_temperature) 
	{
		double cycle_length = tsp_instance.do_cycle_length();		
		tsp_class temp_tsp_instance = tsp_instance;
    
		int i = monte_carlo_steps;
		while(i-- > 0)
		{
			temp_tsp_instance.do_pertubation(rnd_tsp, false);	
			double temp_cycle_length = temp_tsp_instance.do_cycle_length();

			double dE = temp_cycle_length - cycle_length;

			bool update = false;
			if(dE < 0.0)
			{
				update = true;
			} 
			else
			{
				//Inferior solution can be allowed to move from local optimal solution
				//using probability of acceptance based on Boltzmann's function
				auto boltzmannFunction = std::exp(-dE / temperature);
				auto acceptanceProbability = rnd_probability();
				update = boltzmannFunction > acceptanceProbability;				
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