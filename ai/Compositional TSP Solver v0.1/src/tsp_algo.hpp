//Sample provided by Fabio Galuppo
//Compositional TSP Solver version 0.1
//October 2013

#pragma once
#ifndef _tsp_algo_
#define _tsp_algo_

#include "tsp.hpp"
#include "tsp_monad.hpp"
#include "tsp_functors.hpp"
#include "algorithms\ant_colony_optimization.hpp"
#include "algorithms\simulated_annealing.hpp"
#include "algorithms\nearest_neighbour.hpp"
#include "algorithms\two_opt.hpp"
#include "algorithms\genetic_algorithm.hpp"
#include "randomizers.hpp"

void get_base_args(unsigned int& i, unsigned int& n, base_args* args)
{
	if (nullptr != args)
	{
		if (args->get_id() == base_args_id::FORK_JOIN_ARGS_ID)
		{
			auto ptr = dynamic_cast<ForkJoin_args*>(args);
			i = ptr->i;
			n = ptr->number_of_tasks_in_parallel;
		}
	}
}

struct SA
{
	SA(const double initial_temperature,
	   const double stopping_criteria_temperature,
	   const double decreasing_factor, 
	   const int monte_carlo_steps) :
		InitialTemperature_(initial_temperature),
		StoppingCriteriaTemperature_(stopping_criteria_temperature),
		DecreasingFactor_(decreasing_factor),
		MonteCarloSteps_(monte_carlo_steps)
	{	
	}
	
	TSP operator()(TSP::T t)
	{
		unsigned int i = 0, n = 1;
		get_base_args(i, n, t.second);
		
		const auto N = ref(t).cities.size();		
		random_functor delay(1, static_cast<int>(n * 2));
		auto seed = static_cast<unsigned>(std::time(nullptr));
		auto rnd_tsp(random_functor(N, seed + i * delay()));

		simulated_annealing(InitialTemperature_, StoppingCriteriaTemperature_, 
							DecreasingFactor_, MonteCarloSteps_, ref(t),
							rnd_tsp, double_random_functor());
		return TSP(t);
	}

private:
	const double InitialTemperature_;
	const double StoppingCriteriaTemperature_;
	const double DecreasingFactor_;
	const int MonteCarloSteps_;
};

#include <vector>

struct ACO
{
	ACO(const unsigned int aco_iterations, 
		const ants_type::size_type number_of_ants, 
		const double base_pheromone,
		const double favor_pheromone_level_over_distance, 
		const double favor_distance_over_pheromone_level, 
		const double value_for_intensification_and_evaporation, 
		const double pheronome_distribution) : 
		ACOIterations_(aco_iterations),
		NumberOfAnts_(number_of_ants),
		BasePheromone_(base_pheromone),
		FavorPheromoneLevelOverDistance_(favor_pheromone_level_over_distance), 
		FavorDistanceOverPheromoneLevel_(favor_distance_over_pheromone_level), 
		ValueForIntensificationAndEvaporation_(value_for_intensification_and_evaporation),
		PheronomeDistribution_(pheronome_distribution)
	{
	}

	TSP operator()(TSP::T t)
	{
		unsigned int i = 0, n = 1;
		get_base_args(i, n, t.second);

		auto seed = static_cast<unsigned>(std::time(nullptr));
		random_functor delay(1, static_cast<int>(n * 10));
		
		ant_colony_optimization(ref(t), BasePheromone_, ACOIterations_, NumberOfAnts_, 
								FavorPheromoneLevelOverDistance_, FavorDistanceOverPheromoneLevel_,
								ValueForIntensificationAndEvaporation_, PheronomeDistribution_, seed + i * delay());
		return TSP(t);
	};

private:	
	const double BasePheromone_;
	const unsigned int ACOIterations_;
	const ants_type::size_type NumberOfAnts_; 
	const double FavorPheromoneLevelOverDistance_; 
	const double FavorDistanceOverPheromoneLevel_; 
	const double ValueForIntensificationAndEvaporation_; 
	const double PheronomeDistribution_;
};

struct _2OPT
{
	TSP operator()(TSP::T t)
	{
		two_opt_all(ref(t));
		return TSP(t);
	}
};

struct NN
{
	TSP operator()(TSP::T t)
	{
		nearest_neighbour(ref(t));
		return TSP(t);
	};
};

struct GA
{
	GA(const unsigned int population_size,
	   const unsigned int mutation_percentage,
	   const unsigned int group_size,
	   const unsigned int number_of_generations,
	   const unsigned int nearby_cities,
	   const double nearby_cities_percentage) :
	   PopulationSize_(population_size),
	   MutationPercentage_(mutation_percentage),
	   GroupSize_(group_size),
	   NumberOfGenerations_(number_of_generations),
	   NearbyCities_(nearby_cities),
	   NearbyCitiesPercentage_(nearby_cities_percentage)
	{
	}

	TSP operator()(TSP::T t)
	{
		unsigned int i = 0, n = 1;
		get_base_args(i, n, t.second);

		auto seed = static_cast<unsigned>(std::time(nullptr));
		random_functor delay(1, static_cast<int>(n * 10));

		genetic_algorithm(ref(t), PopulationSize_, MutationPercentage_, 
			GroupSize_, NumberOfGenerations_, NearbyCities_, NearbyCitiesPercentage_, seed + i * delay());
		return TSP(t);
	}

private:
	const unsigned int PopulationSize_;
	const unsigned int MutationPercentage_;
	const unsigned int GroupSize_;
	const unsigned int NumberOfGenerations_;
	const unsigned int NearbyCities_;
	const double NearbyCitiesPercentage_;
};

#endif /* _tsp_algo_ */