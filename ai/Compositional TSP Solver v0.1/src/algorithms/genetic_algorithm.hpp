//Sample provided by Fabio Galuppo
//Compositional TSP Solver version 0.1
//October 2013

#pragma once
#ifndef _genetic_algorithm_
#define _genetic_algorithm_

#include "..\tsp.hpp"
#include "..\randomizers.hpp"

#include <iostream>
#include <utility>
#include <vector>
#include <algorithm>
#include <functional>
#include <limits>

typedef std::pair<typename tsp_class::cities_type::size_type /* id */, double /* distance */> distance_value_type;
typedef std::vector<distance_value_type> distance_collection_type;

struct tour
{
	typedef std::pair<unsigned int, unsigned int> connection;
	typedef std::vector<connection> connections_type;
	
	connections_type connections;

	tour(const tsp_class& tsp_instance) : 
		Fitness_(0.0), 
		TspInstance_(tsp_instance)
	{
	}

	tour(const tour& that) : 
		TspInstance_(that.TspInstance_)
	{
		connections = that.connections;
		Fitness_ = do_fitness();
	}

	tour& operator=(const tour& that)
	{
		if (this != &that)
		{
			connections = that.connections;
			const_cast<tsp_class&>(TspInstance_) = that.TspInstance_;
			Fitness_ = do_fitness();
		}
		return *this;
	}

	~tour()
	{
	}

	void resize_and_reset()
	{
		connections.resize(TspInstance_.cities.size());
		for (auto i = connections.begin(); i != connections.end(); ++i)
		{
			*i = std::make_pair(std::numeric_limits<unsigned int>::max(), 
				std::numeric_limits<unsigned int>::max());
		}
	}

	const std::pair<bool, bool> has_connection(connections_type::size_type index) const
	{
		const auto& i = connections[index];
		return std::make_pair(i.first != no_connection(), 
			i.second != no_connection());
	}

	double do_fitness()
	{
		double fitness = 0.0;
		unsigned int previous_city = 0, next_city = connections[0].first;

		for (auto i = connections.begin(); i != connections.end(); ++i)
		{
			fitness += tsp_class::euclidean_distance(TspInstance_.cities[previous_city].location, 
				TspInstance_.cities[next_city].location);
			
			bool x = previous_city != connections[next_city].first;
			previous_city = next_city;
			next_city =  x ? connections[next_city].first : connections[next_city].second;
		}

		Fitness_ = fitness;

		return fitness;
	}

	const double fitness() const { return Fitness_; }

	static unsigned int no_connection()
	{
		return std::numeric_limits<unsigned int>::max();
	}

private:
	double Fitness_;
	const tsp_class& TspInstance_;
};

std::vector<distance_collection_type> 
make_closest_cities_table(const tsp_class& tsp_instance, 
						  distance_collection_type::size_type closest_cities_maxcount)
{
	std::vector<distance_collection_type> distance_table;

	tsp_class::cities_type::size_type i_idx = 0;
	for (auto i = tsp_instance.cities.cbegin(); i != tsp_instance.cities.cend(); ++i, ++i_idx)
	{
		distance_collection_type distances;
		distances.reserve(tsp_instance.cities.size() - 1 /* (*) */);
		
		tsp_class::cities_type::size_type j_idx = 0;
		for (auto j = tsp_instance.cities.cbegin(); j != tsp_instance.cities.cend(); ++j, ++j_idx)
		{
			if (j_idx != i_idx) //excluding self (*)
			{
				auto distance = tsp_class::euclidean_distance(i->location, j->location);
				distances.push_back(std::make_pair(j_idx, distance));
			}
		}

		std::sort(distances.begin(), distances.end(), [] (const distance_value_type& lhs, const distance_value_type& rhs)
		{
			return lhs.second < rhs.second;
		});

		if (distances.size() > closest_cities_maxcount) distances.resize(closest_cities_maxcount);

		distance_table.push_back(std::move(distances));
	}

	return distance_table;
}

unsigned int& first_ref(const tour& t, tsp_class::cities_type::size_type index)
{
	return const_cast<unsigned int&>(t.connections[index].first);
}

unsigned int& second_ref(const tour& t, tsp_class::cities_type::size_type index)
{
	return const_cast<unsigned int&>(t.connections[index].second);
}

tsp_class tour_to_tsp_class(const tsp_class& tsp_instance, const tour& t)
{
	tsp_class result;
	result.cities.reserve(tsp_instance.cities.size());
	
	unsigned int previous_city = 0,	next_city = first_ref(t, 0);

	for (tour::connections_type::size_type i = 0, l = t.connections.size(); i < l; ++i)
	{
		result.cities.push_back(tsp_instance.cities[next_city]);		
		bool x = previous_city != first_ref(t, next_city);
		previous_city = next_city;
		next_city = x ? first_ref(t, next_city) : second_ref(t, next_city);
	}

	return result;
}

typedef std::vector<tour> population_type;

population_type make_population_via_selection(const tsp_class& tsp_instance, 
											  tour& best_tour, 
											  unsigned int population_size, 
											  std::vector<distance_collection_type> closest_cities_table, 
											  double closest_city_probability, 
											  tsp_class::rand_function rnd_tsp, 
											  std::function<double()> rnd_probability, 
											  tsp_class::rand_function rnd_closest_city)
{
	const auto N = tsp_instance.cities.size();

	population_type population;
	population.reserve(population_size);

	int begin_city, end_city;
	for (unsigned int i = 0; i < population_size; ++i)
	{
		tour new_tour(tsp_instance);
		new_tour.resize_and_reset();
		
		begin_city = rnd_tsp();
		end_city = begin_city;

		int next_city;
		for (unsigned int j = 0; j < N - 1; ++j)
		{
			do  next_city = (rnd_probability() < closest_city_probability) && (closest_cities_table[j].size() > 0) ? 
				closest_cities_table[j][rnd_closest_city()].first : rnd_tsp();
			while (new_tour.has_connection(next_city).second || (next_city == end_city));

			second_ref(new_tour, end_city) = next_city;
			first_ref(new_tour, next_city) = end_city;
			end_city = next_city;
			second_ref(new_tour, end_city) = begin_city;
			first_ref(new_tour, begin_city) = end_city;
		}

		//new_tour will execute do_fitness() in copy constructor when it's copied to population
		population.push_back(new_tour);
	}

	best_tour = *std::min_element(population.begin(), population.end(), [](const tour& lhs, const tour& rhs) {
		return lhs.fitness() < rhs.fitness();
	});

	return population;
}

bool has_valid_connection(const tsp_class& tsp_instance, 
						  tour& t, 
						  std::vector<tsp_class::cities_type::size_type>& city_usage, 
						  tsp_class::cities_type::size_type first_city, 
						  tsp_class::cities_type::size_type second_city)
{
	
    if ((first_city == second_city) || (2 == city_usage[first_city]) || (2 == city_usage[second_city]))
        return false;
    
	if ((0 == city_usage[first_city]) || (0 == city_usage[second_city]))
		return true;

	for (unsigned int i = 0; i < 2; ++i)
    {
        tsp_class::cities_type::size_type last_city = first_city;
        auto connection = i == 0 ? first_ref(t, first_city) : second_ref(t, first_city);
        
        tsp_class::cities_type::size_type tour_count = 0;
		while ((connection != tour::no_connection()) && (connection != second_city) && (tour_count < tsp_instance.cities.size() - 2))
        {
            ++tour_count;

            if (last_city != first_ref(t, connection))
            {
                last_city = connection;
                connection = first_ref(t, connection);
            }
            else
            {
                last_city = connection;
                connection = second_ref(t, connection);
            }
        }

		if (tour_count >= tsp_instance.cities.size() - 2)
			return true;
        
        if (connection == second_city)
            return false;
	}

	return true;
}

void connect_cities(tour& t, 
					std::vector<tsp_class::cities_type::size_type>& city_usage, 
					tsp_class::cities_type::size_type first_city, 
					tsp_class::cities_type::size_type second_city)
{
    if (t.connections[first_city].first == tour::no_connection())
        t.connections[first_city].first = second_city;
    else
        t.connections[first_city].second = second_city;

	if (t.connections[second_city].first == tour::no_connection())
        t.connections[second_city].first = first_city;
    else
        t.connections[second_city].second = first_city;

    ++city_usage[first_city];
    ++city_usage[second_city];
}

unsigned int find_next_city(const tsp_class& tsp_instance, 
							tour& parent, 
							tour& child, 
							std::vector<tsp_class::cities_type::size_type>& city_usage, 
							int city)
{
    if (has_valid_connection(tsp_instance, child, city_usage, city, first_ref(parent, city)))
        return first_ref(parent, city);
    
    if (has_valid_connection(tsp_instance, child, city_usage, city, second_ref(parent, city)))
        return second_ref(parent, city);
    
    return tour::no_connection();
}

tour crossover(const tsp_class& tsp_instance, 
			   tour& first_parent, 
			   tour& second_parent, 
			   const random_functor& rnd)
{
	tour child(tsp_instance);
	child.resize_and_reset();

	std::vector<tsp_class::cities_type::size_type> city_usage;
	city_usage.resize(tsp_instance.cities.size());
	std::fill(city_usage.begin(), city_usage.end(), 0);

	tsp_class::cities_type::size_type next_city = 0;
	for (tsp_class::cities_type::size_type city = 0; city != tsp_instance.cities.size(); ++city)
	{
		if (city_usage[city] < 2)
		{
			auto& a = first_ref(first_parent, city);
			auto& b = second_ref(first_parent, city);
			auto& c = first_ref(second_parent, city);
			auto& d = second_ref(second_parent, city);
			
			if (a == c)
			{
				next_city = a;
				if (has_valid_connection(tsp_instance, child, city_usage, city, next_city))
					connect_cities(child, city_usage, city, next_city);
			}

			if (b == d)
			{
				next_city = b;
				if (has_valid_connection(tsp_instance, child, city_usage, city, next_city))
					connect_cities(child, city_usage, city, next_city);
			
			}

			if (a == d)
			{
				next_city = a;
				if (has_valid_connection(tsp_instance, child, city_usage, city, next_city))
					connect_cities(child, city_usage, city, next_city);
			}

			if (b == c)
			{
				next_city = b;
				if (has_valid_connection(tsp_instance, child, city_usage, city, next_city))
					connect_cities(child, city_usage, city, next_city);
			}
		}
	}

	for (tsp_class::cities_type::size_type city = 0; city != tsp_instance.cities.size(); ++city)
    {
        if (city_usage[city] < 2)
        {
            if (city % 2 == 1)
            {
                next_city = find_next_city(tsp_instance, first_parent, child, city_usage, city);
                if (next_city == tour::no_connection())
                    next_city = find_next_city(tsp_instance, second_parent, child, city_usage, city);
            }
            else
            {
                next_city = find_next_city(tsp_instance, second_parent, child, city_usage, city);
                if (next_city == tour::no_connection())
                    next_city = find_next_city(tsp_instance, first_parent, child, city_usage, city);
            }

            if (next_city != tour::no_connection())
            {
                connect_cities(child, city_usage, city, next_city);

				if (city_usage[city] == 1)
                {
                    if (city % 2 != 1)
                    {
                        next_city = find_next_city(tsp_instance, first_parent, child, city_usage, city);
                        if (next_city == tour::no_connection())
                            next_city = find_next_city(tsp_instance, second_parent, child, city_usage, city);
                    }
                    else
                    {
                        next_city = find_next_city(tsp_instance, second_parent, child, city_usage, city);
                        if (next_city == tour::no_connection())
                            next_city = find_next_city(tsp_instance, first_parent, child, city_usage, city);
                    }

                    if (next_city != tour::no_connection())
						connect_cities(child, city_usage, city, next_city);
                }
            }
        }
    }

	for (tsp_class::cities_type::size_type city = 0; city != tsp_instance.cities.size(); ++city)
    {
        while (city_usage[city] < 2)
        {
            do  next_city = rnd();
			while (!has_valid_connection(tsp_instance, child, city_usage, city, next_city));
            connect_cities(child, city_usage, city, next_city);
        }
    }

    return child;
}

void mutate(tour& t, const random_functor& rnd)
{
	int city_number = rnd();
	auto& connection = t.connections[city_number];

	int temp = connection.second;

	if (first_ref(t, connection.first) == city_number)
    {
        if (first_ref(t, connection.second) == city_number)
            first_ref(t, connection.second) = connection.first;
        else
            second_ref(t, connection.second) = connection.first;

		first_ref(t, connection.first) = temp;
    }
    else
    {
        if (first_ref(t, connection.second) == city_number)
            first_ref(t, connection.second) = connection.first;
        else
            second_ref(t, connection.second) = connection.first;
		
		second_ref(t, connection.first) = temp;
    }
	
	int other_city_number = -1;
    do other_city_number = rnd(); while (other_city_number == city_number);
    auto& other_connection = t.connections[other_city_number];

    temp = other_connection.second;
    connection.second = other_connection.second;
    connection.first = other_city_number;
    other_connection.second = city_number;

    if (first_ref(t, temp) == other_city_number)
        first_ref(t, temp) = city_number;
    else
        second_ref(t, temp) = city_number;
}

tour do_crossover_and_mutation(const tsp_class& tsp_instance, 
							   tour& current_best_tour, 
							   tour& first_parent,
							   tour& second_parent,							   				   
							   population_type::size_type mutation,
							   const random_functor& rnd_tour,
							   const random_functor& rnd_accept_mutation_percentage)
{
	auto new_tour = crossover(tsp_instance, first_parent, second_parent, rnd_tour);
	if (static_cast<unsigned int>(rnd_accept_mutation_percentage()) < mutation)
	{
		mutate(new_tour, rnd_tour);
		new_tour.do_fitness();
	}

	if (new_tour.fitness() < current_best_tour.fitness())
		current_best_tour = new_tour;

	return new_tour;
}

void make_children_via_genetic_operation(const tsp_class& tsp_instance, 
										 tour& current_best_tour, 
										 population_type& population, 
										 population_type::size_type group_size, 
										 population_type::size_type mutation, 
										 const random_functor& rnd_population, 
										 const random_functor& rnd_tour, 
										 const random_functor& rnd_accept_mutation_percentage)
{
	std::vector<population_type::size_type> tours;
	tours.reserve(group_size);

    for (unsigned int i = 0; i != group_size; ++i) 
		tours.push_back(rnd_population());

	std::sort(tours.begin(), tours.end(), [&population](const population_type::size_type& lhs, const population_type::size_type& rhs) {
		return population[lhs].fitness() < population[rhs].fitness();
	});

	population[tours[group_size - 1]] = 
		do_crossover_and_mutation(
			tsp_instance, 
			current_best_tour, 
			population[tours[0]], 
			population[tours[1]], 
			mutation, 
			rnd_tour, 
			rnd_accept_mutation_percentage);
	
	population[tours[group_size - 2]] =
		do_crossover_and_mutation(
			tsp_instance, 
			current_best_tour, 
			population[tours[1]], 
			population[tours[0]],
			mutation,
			rnd_tour,
			rnd_accept_mutation_percentage);
}

void genetic_algorithm(tsp_class& tsp_instance,
					   const unsigned int population_size,
					   const unsigned int mutation_percentage,
					   const unsigned int group_size,
					   const unsigned int number_of_generations,
					   const unsigned int nearby_cities,
					   const double nearby_cities_percentage,
					   const unsigned int seed)
{
	auto closest_cities_table = make_closest_cities_table(tsp_instance, nearby_cities);
	random_functor rnd_tour(static_cast<int>(tsp_instance.cities.size()), seed);
	
	tour best_tour(tsp_instance);
	auto population = make_population_via_selection(tsp_instance, best_tour, 
		population_size, closest_cities_table, nearby_cities_percentage, 
		rnd_tour, double_random_functor(), random_functor(nearby_cities));
	
	random_functor rnd_population(static_cast<int>(population.size()), seed);	
	random_functor rnd_accept_mutation_percentage(100, seed);
	
	for(unsigned int g = 0; g < number_of_generations; ++g)
		make_children_via_genetic_operation(tsp_instance, best_tour, population, group_size, mutation_percentage, 
			rnd_population, rnd_tour, rnd_accept_mutation_percentage);

	tsp_instance = tour_to_tsp_class(tsp_instance, best_tour);
}

#endif /* _genetic_algorithm_ */