//Sample provided by Fabio Galuppo
//Compositional TSP Solver version 0.11
//September 2014

#pragma once
#ifndef _tsp_algo_
#define _tsp_algo_

#include "tsp.hpp"
#include "tsp_monad.hpp"
#include "tsp_functors.hpp"
#include "algorithms\simulated_annealing.hpp"
#include "algorithms\nearest_neighbour.hpp"
#include "algorithms\two_opt.hpp"
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

struct SA final
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
	
	tsp_monad operator()(tsp_monad::value_type t)
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
		return unit(t);
	}

	SA() = delete;
	SA(const SA&) = default;
	SA& operator=(const SA&) = delete;

private:
	const double InitialTemperature_;
	const double StoppingCriteriaTemperature_;
	const double DecreasingFactor_;
	const int MonteCarloSteps_;
};

#include <vector>

struct _2OPT final
{
	tsp_monad operator()(tsp_monad::value_type t)
	{
		two_opt_all(ref(t));
		return unit(t);
	}

	_2OPT() = default;
	_2OPT(const _2OPT&) = default;
	_2OPT& operator=(const _2OPT&) = delete;
};

struct NN final
{
	tsp_monad operator()(tsp_monad::value_type t)
	{
		nearest_neighbour(ref(t));
		return unit(t);
	};

	NN() = default;
	NN(const NN&) = default;
	NN& operator=(const NN&) = delete;
};

#endif /* _tsp_algo_ */