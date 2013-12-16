//Sample provided by Fabio Galuppo
//Compositional TSP Solver version 0.1
//October 2013

#pragma once
#ifndef __tsp_support_
#define __tsp_support_

struct GeneralArgs
{
	GeneralArgs(const unsigned int number_of_iterations_or_generations, const unsigned int number_of_tasks_in_parallel) :
		number_of_iterations_or_generations(number_of_iterations_or_generations),
		number_of_tasks_in_parallel(number_of_tasks_in_parallel)
	{
	}

	const unsigned int number_of_iterations_or_generations; 
	const unsigned int number_of_tasks_in_parallel;
};

struct SAArgs
{
	SAArgs(const double initial_temperature,
		   const double stopping_criteria_temperature,
		   const double decreasing_factor, 
		   const int monte_carlo_steps) :
		   initial_temperature(initial_temperature),
		   stopping_criteria_temperature(stopping_criteria_temperature),
		   decreasing_factor(decreasing_factor),
           monte_carlo_steps(monte_carlo_steps)
	{
	}

	const double initial_temperature;
	const double stopping_criteria_temperature;
	const double decreasing_factor;
	const int monte_carlo_steps;
};

struct ACOArgs
{
	ACOArgs(const unsigned int aco_iterations, 
		const ants_type::size_type number_of_ants, 
		const double base_pheromone,
		const double favor_pheromone_level_over_distance,
		const double favor_distance_over_pheromone_level,
		const double value_for_intensification_and_evaporation,
		const double pheronome_distribution) :
		aco_iterations(aco_iterations), 
		number_of_ants(number_of_ants), 
		base_pheromone(base_pheromone),
		favor_pheromone_level_over_distance(favor_pheromone_level_over_distance),
		favor_distance_over_pheromone_level(favor_distance_over_pheromone_level),
		value_for_intensification_and_evaporation(value_for_intensification_and_evaporation),
		pheronome_distribution(pheronome_distribution)
	{
	}

	const unsigned int aco_iterations; 
	const ants_type::size_type number_of_ants;
	const double base_pheromone;
	const double favor_pheromone_level_over_distance;
	const double favor_distance_over_pheromone_level;
	const double value_for_intensification_and_evaporation;
	const double pheronome_distribution;
};

struct GAArgs
{
	GAArgs(const unsigned int population_size,
	   const unsigned int mutation_percentage,
	   const unsigned int group_size,
	   const unsigned int number_of_generations,
	   const unsigned int nearby_cities,
	   const double nearby_cities_percentage) :
	population_size(population_size),
	mutation_percentage(mutation_percentage),
	group_size(group_size),
	number_of_generations(number_of_generations),
	nearby_cities(nearby_cities),
	nearby_cities_percentage(nearby_cities_percentage)
	{
	}

	const unsigned int population_size;
	const unsigned int mutation_percentage;
	const unsigned int group_size;
	const unsigned int number_of_generations;
	const unsigned int nearby_cities;
	const double nearby_cities_percentage;
};

typedef GeneralArgs General_args_type;
typedef SAArgs SA_args_type;
typedef ACOArgs ACO_args_type;
typedef GAArgs GA_args_type;

General_args_type
make_General_args(const unsigned int number_of_iterations_or_generations, const unsigned int number_of_tasks_in_parallel)
{
	return General_args_type(number_of_iterations_or_generations, number_of_tasks_in_parallel);
}

SA_args_type 
make_SA_args(const double initial_temperature,
			 const double stopping_criteria_temperature,
			 const double decreasing_factor, 
			 const int monte_carlo_steps)
{
	return SA_args_type(initial_temperature, stopping_criteria_temperature, decreasing_factor, monte_carlo_steps);
}

ACO_args_type
make_ACO_args(const unsigned int aco_iterations, 
		const ants_type::size_type number_of_ants, 
		const double base_pheromone,
		const double favor_pheromone_level_over_distance,
		const double favor_distance_over_pheromone_level,
		const double value_for_intensification_and_evaporation,
		const double pheronome_distribution)
{
	return ACO_args_type(aco_iterations, number_of_ants, base_pheromone, 
						 favor_pheromone_level_over_distance, favor_distance_over_pheromone_level,
						 value_for_intensification_and_evaporation, pheronome_distribution);
}

GA_args_type
make_GA_args(const unsigned int population_size,
	   const unsigned int mutation_percentage,
	   const unsigned int group_size,
	   const unsigned int number_of_generations,
	   const unsigned int nearby_cities,
	   const double nearby_cities_percentage)
{
	return GA_args_type(population_size, mutation_percentage, group_size, 
		number_of_generations, nearby_cities, nearby_cities_percentage);
}

#include <vector>
typedef std::vector<General_args_type> General_args_type_collection;
typedef std::vector<SA_args_type> SA_args_type_collection;
typedef std::vector<ACO_args_type> ACO_args_type_collection;
typedef std::vector<GA_args_type> GA_args_type_collection;

void display_args(const char* description,
	              const General_args_type_collection& gs,
				  const SA_args_type_collection& sas,
				  const ACO_args_type_collection& acos,
				  const GA_args_type_collection& gas)
{
	std::cout << "SOLUTION ARGUMENTS:" << std::endl;

    std::cout << description << std::endl;
	
	if (gs.size() > 0)
	{
		std::cout << "GENERAL ARGUMENTS:" << std::endl;		
		int count = 0;
		for(auto i = gs.cbegin(); i != gs.cend(); ++i) 
		{
			std::cout << " #" << ++count << ":"<< std::endl;
			std::cout << " Number of Iterations or Generations = " << i->number_of_iterations_or_generations << std::endl;
			std::cout << " Number of Tasks in Parallel = " << i->number_of_tasks_in_parallel << std::endl;
			//std::cout << "Propagate Inferior Results Across Generations = " << (p ? "true" : "false") << std::endl;
		}
	}

	if (sas.size() > 0)
	{
		std::cout << "SIMULATED ANNEALING ARGUMENTS:" << std::endl;
		int count = 0;
		for(auto i = sas.cbegin(); i != sas.cend(); ++i) 
		{
			std::cout << " #" << ++count << ":"<< std::endl;
			std::cout << "   Initial Temperature = " << i->initial_temperature << std::endl;
			std::cout << "   Stopping Criteria Temperature = " << i->stopping_criteria_temperature << std::endl;
			std::cout << "   Decreasing Factor = " << i->decreasing_factor << std::endl;
			std::cout << "   Monte Carlo Steps = " << i->monte_carlo_steps << std::endl;
		}
	}

	if (acos.size() > 0)
	{
		std::cout << "ANT COLONY OPTIMIZATION ARGUMENTS:" << std::endl;
		int count = 0;
		for(auto i = acos.cbegin(); i != acos.cend(); ++i) 
		{
			std::cout << " #" << ++count << ":"<< std::endl;
			std::cout << "   ACO Iterations = " << i->aco_iterations << std::endl;
			std::cout << "   Number of Ants = " << i->number_of_ants << std::endl;
			std::cout << "   Base Pheromone = " << i->base_pheromone << std::endl;
			std::cout << "   Favor Pheromone Level over Distance = " << i->favor_pheromone_level_over_distance << std::endl;
			std::cout << "   Favor Distance over Pheromone Level = " << i->favor_distance_over_pheromone_level << std::endl;
			std::cout << "   Intensification and Evaporation Value = " << i->value_for_intensification_and_evaporation << std::endl;
			std::cout << "   Pheromone Distribution = " << i->pheronome_distribution << std::endl;
		}
	}

	if (gas.size() > 0)
	{
		std::cout << "GENETIC ALGORITHM ARGUMENTS:" << std::endl;
		int count = 0;
		for(auto i = gas.cbegin(); i != gas.cend(); ++i) 
		{
			std::cout << " #" << ++count << ":"<< std::endl;
			std::cout << "   Population Size = " << i->population_size << std::endl;
			std::cout << "   Mutation Percentage = " << i->mutation_percentage << std::endl;
			std::cout << "   Group Size = " << i->group_size << std::endl;
			std::cout << "   Number of Generations = " << i->number_of_generations << std::endl;
			std::cout << "   Nearby Cities = " << i->nearby_cities << std::endl;
			std::cout << "   Nearby Cities Percentage = " << i->nearby_cities_percentage << std::endl;
		}
	}
	
	std::cout << "--------------------------------------------------" << std::endl;
}

template <typename A>
struct Args
{
	typedef std::vector<A> collection_type;

	operator collection_type() const 
	{ 
		return collection;
	}

	Args()
	{
	}

	Args(A a0)
	{
		collection.push_back(a0);
	}

	Args(A a0, A a1)
	{
		collection.push_back(a0);
		collection.push_back(a1);
	}

	Args(A a0, A a1, A a2)
	{
		collection.push_back(a0);
		collection.push_back(a1);
		collection.push_back(a2);
	}

	Args(A a0, A a1, A a2, A a3)
	{
		collection.push_back(a0);
		collection.push_back(a1);
		collection.push_back(a2);
		collection.push_back(a3);
	}

	Args(A a0, A a1, A a2, A a3, A a4)
	{
		collection.push_back(a0);
		collection.push_back(a1);
		collection.push_back(a2);
		collection.push_back(a3);
		collection.push_back(a4);
	}

	Args(A a0, A a1, A a2, A a3, A a4, A a5)
	{
		collection.push_back(a0);
		collection.push_back(a1);
		collection.push_back(a2);
		collection.push_back(a3);
		collection.push_back(a4);
		collection.push_back(a5);
	}

	//...

	typename collection_type::const_reference operator[](typename collection_type::size_type index) const
	{	
		return collection[index];
	}

	typename collection_type::reference operator[](typename collection_type::size_type index)
	{	
		return collection[index];
	}

	collection_type collection;
};

#endif /* __tsp_support_ */