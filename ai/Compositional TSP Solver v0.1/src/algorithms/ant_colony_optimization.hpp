//Sample provided by Fabio Galuppo
//Compositional TSP Solver version 0.1
//October 2013

#pragma once
#ifndef _ant_colony_optimization_
#define _ant_colony_optimization_

#include "..\tsp.hpp"
#include "..\randomizers.hpp"

#include <cmath>
#include <memory>
#include <algorithm>
#include <utility>
#include <limits>

typedef std::vector<std::vector<double>> pheromones_type;

struct ant_class;

typedef std::vector<std::shared_ptr<ant_class>> ants_type;

struct ant_class
{
	typedef std::vector<bool> tabu_type;
	typedef std::vector<unsigned short> tour_type;
	
	ant_class(int size, int city, 
		const pheromones_type& pheromones, 
		const tsp_class& tsp_instance, 
		std::unique_ptr<double_random_functor> random_func = std::unique_ptr<double_random_functor>(new double_random_functor())) :
		referential_city(city),
		pheromones(pheromones),
		tsp_instance(tsp_instance),
		random_func(std::move(random_func))
	{
		tabu.resize(size);
		tour.resize(size);
		reset();
	}
	
	void reset()
	{
		current_city = referential_city;
		
		tabu_type::size_type N = tabu.size();
		for (tabu_type::size_type i = 0; i < N; ++i)
		{
			tabu[i] = false;
			tour[i] = 0;
		}

		tabu[current_city] = true;
		tour[0] = current_city;
		tour_index = 1;
		tour_length = 0.0;
	}

	tour_type tour;

private:
	tabu_type tabu;

	int current_city;
	int next_city;
	tour_type::size_type tour_index;
    double tour_length;
	const pheromones_type& pheromones;
	const tsp_class& tsp_instance;
	std::unique_ptr<double_random_functor> random_func;	

	bool move(const double alpha /* favor pheromone level over distance */, const double beta /* favor distance over pheromone level */)
	{
		tour_type::size_type N = tour.size();
		if (tour_index < N)
		{
			take_next_city(alpha, beta);
			return true;
		}

		return false;
	}

	void take_next_city(const double alpha /* favor pheromone level over distance */, const double beta /* favor distance over pheromone level */)
	{
		double total_for_non_visited_cities = 0.0;
		tsp_class::cities_type::size_type from = current_city, to = 0;
		for(auto i = tabu.cbegin(); i != tabu.cend(); ++i, ++to)
			if (*i == 0)
				total_for_non_visited_cities += product_of_pheronome_level_and_distance(tsp_instance, pheromones, from, to, alpha, beta);		
		
		to = 0;
		tabu_type::size_type N = tabu.size();
		unsigned short max_iteration_guard = std::numeric_limits<unsigned short>::max();
		while (max_iteration_guard > 0)
		{
			if (!tabu[to])
			{
				const double current_product_of_pheronome_level_and_distance = product_of_pheronome_level_and_distance(tsp_instance, pheromones, from, to, alpha, beta);
				const double visitProbability = current_product_of_pheronome_level_and_distance / total_for_non_visited_cities;
				
				if ((*random_func)() < visitProbability)
					break;
				else
					max_iteration_guard--;
			}

			to = (to + 1) % N;
		}
		
		next_city = to;
		tabu[next_city] = true;
		tour[tour_index++] = next_city;
		
		tour_length += tsp_class::euclidean_distance(tsp_instance.cities[current_city].location, tsp_instance.cities[next_city].location);
		if (tour_index == N)
			tour_length += tsp_class::euclidean_distance(tsp_instance.cities[N-1].location, tsp_instance.cities[0].location);
		
		current_city = next_city;
	}

	static double product_of_pheronome_level_and_distance(const tsp_class& tsp_instance, const pheromones_type& pheromones, pheromones_type::size_type from, pheromones_type::size_type to, double alpha, double beta)
	{
		const auto level_of_pheronome_between_current_and_candidate_location = std::pow(pheromones[from][to], alpha);
		const auto distance_between_current_and_candidate_location = std::pow((1.0 / tsp_class::euclidean_distance(tsp_instance.cities[from].location, tsp_instance.cities[to].location)), beta);
		return  level_of_pheronome_between_current_and_candidate_location * distance_between_current_and_candidate_location;
	}

	friend static void update_pheromone_trails(const tsp_class&, pheromones_type&, const ants_type&, const double, const double, const double);
	friend static void take_min_cycle(tsp_class&, const ants_type&);
	friend static void ant_colony_optimization(tsp_class&, double, unsigned int, const ants_type::size_type, const double, const double, const double, const double, const unsigned int);

	private:
		unsigned short referential_city;
};

void update_pheromone_trails(const tsp_class& tsp_instance, pheromones_type& pheromones, const ants_type& ants, const double base_pheromone, const double rho, const double qval)
{
	const tsp_class::cities_type::size_type N = tsp_instance.cities.size();

	//evaporate
	for (tsp_class::cities_type::size_type i = 0; i < N; ++i)
	{
		for (tsp_class::cities_type::size_type j = 0; j < N; ++j)
		{
			pheromones[i][j] *= (1.0 - rho);
			if (pheromones[i][j] < 0.0) 
				pheromones[i][j] = base_pheromone;
		}
	}
	
	const ants_type::size_type A = ants.size();
	
	//intensify	
	for(auto ant = ants.cbegin(); ant != ants.end(); ++ant)
	{
		for (tsp_class::cities_type::size_type i = 0; i < N; ++i) 
		{
			int from = (*ant)->tour[i];
			int to   = (*ant)->tour[((i+1) % N)];
		  
		  pheromones[from][to] += ((qval / (*ant)->tour_length) * rho);
		  pheromones[to][from] =  pheromones[from][to];
		}
	}
}

void take_min_cycle(tsp_class& tsp_instance, const ants_type& ants)
{
	std::vector<std::pair<int, double>> tours_lenght;
	tours_lenght.resize(ants.size());
	
	int i = 0;
	std::transform(ants.cbegin(), ants.cend(), tours_lenght.begin(), [&i](const std::shared_ptr<ant_class>& a)
	{
		return std::make_pair(i++, (*a).tour_length);
	});
	
	std::sort(tours_lenght.begin(), tours_lenght.end(), [](const std::pair<int, double>& lhs, const std::pair<int, double>& rhs)
	{
		return lhs.second < rhs.second;
	});

	auto best_tour = *tours_lenght.begin();	
	ant_class& best_ant = *(ants[best_tour.first].get());
	
	tsp_class temp = tsp_instance;
	i = 0;
	for(auto iter = best_ant.tour.cbegin(); iter != best_ant.tour.cend(); ++iter)
		temp.cities[i++] = tsp_instance.cities[*iter];

	if (temp.do_cycle_length() <= tsp_instance.do_cycle_length())
		tsp_instance = temp;
}

void ant_colony_optimization(tsp_class& tsp_instance,
							 const double base_pheromone, 
							 unsigned int iterations, 
							 const ants_type::size_type number_of_ants, 
							 const double favor_pheromone_level_over_distance, 
							 const double favor_distance_over_pheromone_level, 
							 const double value_for_intensification_and_evaporation, 
							 const double pheronome_distribution,
							 const unsigned int rnd_seed)
{
	const auto N = tsp_instance.cities.size();
	random_functor rnd_Q(static_cast<int>(pheronome_distribution), 100, rnd_seed);
	double Q = rnd_Q();

	pheromones_type pheromones;
	ants_type ants;
	{
		random_functor delay(1, number_of_ants * 2);		
		std::vector<std::unique_ptr<double_random_functor>> double_random_functors;
		for (ants_type::size_type i = 0; i < number_of_ants; ++i)				
			double_random_functors.push_back(std::unique_ptr<double_random_functor>(new double_random_functor(rnd_seed + i * delay())));

		for (std::vector<std::shared_ptr<ant_class>>::size_type a = 0; a < number_of_ants; ++a)
			ants.push_back(std::shared_ptr<ant_class>(new ant_class(N, a % N, pheromones, tsp_instance, std::move(double_random_functors[a]))));
	}

	pheromones.reserve(N * N);
	for (tsp_class::cities_type::size_type i = 0; i < N; ++i)
	{
		std::vector<double> v;
		for (tsp_class::cities_type::size_type j = 0; j < N; ++j)
			v.push_back(base_pheromone);
		pheromones.push_back(v);
	}
	
	while (iterations-- > 0)
	{
		 int moved = 0;
		 for (auto a = ants.begin(); a != ants.end(); ++a) 
			 moved += (*a)->move(favor_pheromone_level_over_distance, favor_distance_over_pheromone_level) ? 1 : 0;
		 if (0 == moved)
		 {
			 //update_pheromone_trails(tsp_instance, pheromones, ants, base_pheromone, value_for_intensification_and_evaporation, pheronome_distribution);
			 update_pheromone_trails(tsp_instance, pheromones, ants, base_pheromone, value_for_intensification_and_evaporation, Q);

			 if (iterations > 0)
			 {
				 take_min_cycle(tsp_instance, ants);
				 for (auto a = ants.begin(); a != ants.end(); ++a) 
					 (*a)->reset();
			 }
		 }
	}
}

#endif /* _ant_colony_optimization_ */