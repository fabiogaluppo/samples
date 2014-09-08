//Sample provided by Fabio Galuppo
//Compositional TSP Solver version 0.11
//September 2014

#pragma once
#ifndef __tsp_support_
#define __tsp_support_

struct GeneralArgs final
{
	GeneralArgs(const unsigned int number_of_iterations_or_generations, const unsigned int number_of_tasks_in_parallel) :
		number_of_iterations_or_generations(number_of_iterations_or_generations),
		number_of_tasks_in_parallel(number_of_tasks_in_parallel)
	{
	}

	GeneralArgs() = delete;
	GeneralArgs(const GeneralArgs&) = default;
	GeneralArgs& operator=(const GeneralArgs&) = delete;

	const unsigned int number_of_iterations_or_generations; 
	const unsigned int number_of_tasks_in_parallel;
};

struct SAArgs final
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

	SAArgs() = delete;
	SAArgs(const SAArgs&) = default;
	SAArgs& operator=(const SAArgs&) = delete;

	const double initial_temperature;
	const double stopping_criteria_temperature;
	const double decreasing_factor;
	const int monte_carlo_steps;
};

typedef GeneralArgs General_args_type;
typedef SAArgs SA_args_type;

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

#include <vector>
typedef std::vector<General_args_type> General_args_type_collection;
typedef std::vector<SA_args_type> SA_args_type_collection;

void display_args(const char* description,
	              const General_args_type_collection& gs,
				  const SA_args_type_collection& sas)
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
	
	std::cout << "--------------------------------------------------" << std::endl;
}

//TODO: Variadic template on this
template <typename A>
struct Args final
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