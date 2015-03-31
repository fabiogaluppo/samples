//Sample provided by Fabio Galuppo
//Compositional TSP Solver version 0.12
//March 2015

#ifndef _tsp_functors_
#define _tsp_functors_

#include "tsp.hpp"
#include "tsp_monad.hpp"

tsp_monad Propagator(const tsp_monad& oldValue, const tsp_monad& newValue, bool propagate_inferior_results = false)
{
	//accept inferior results (if it's an inferior result!)
	if (propagate_inferior_results)	
		return newValue;
	
	//do not propagate inferior results
	if (ref(oldValue).do_cycle_length() > ref(newValue).do_cycle_length())
		return newValue;

	return oldValue;
};

enum base_args_id
{
	FORK_JOIN_ARGS_ID = 1
};

template<typename Function, typename... Functions>
struct Chain final
{
	typedef tsp_monad::function_type  F;

	Chain(Function f, Functions... fs) : Func_(f), Fs_(fs...) {}

	tsp_monad operator()(tsp_monad::value_type t)
	{
		Func_(t); //do work
		for (auto f : { Fs_ }) f(t); //call next in chain
		return unit(t);		
	}

	Chain() = delete;
	Chain(const Chain&) = default;
	Chain& operator=(const Chain&) = delete;

private:
	F Func_;
	Chain<Functions...> Fs_;
};

template<typename Function> 
struct Chain<Function> final
{
	typedef tsp_monad::function_type F;

	Chain(Function f) : Func_(f) {}

	tsp_monad operator()(tsp_monad::value_type t)
	{
		Func_(t);
		return unit(t);
	}

	Chain() = delete;
	Chain(const Chain&) = default;	
	Chain& operator=(const Chain&) = delete;

private:
	F Func_;
};

struct ForkJoin_args final : public base_args
{
	ForkJoin_args(unsigned int i, unsigned int number_of_tasks_in_parallel) : 
		base_args(FORK_JOIN_ARGS_ID), 
		i(i), 
		number_of_tasks_in_parallel(number_of_tasks_in_parallel) {}
	
	ForkJoin_args() = delete;
	ForkJoin_args(const ForkJoin_args&) = default;
	ForkJoin_args& operator=(const ForkJoin_args&) = delete;
	
	const unsigned int i;
	const unsigned int number_of_tasks_in_parallel;
};

#include <future>
#include <memory>

struct ForkJoin final
{
	typedef tsp_monad::function_type F;

	ForkJoin(unsigned int number_of_tasks_in_parallel, F f, bool propagate_inferior_results = false) : 
		NumberOfTasksInParallel_(number_of_tasks_in_parallel),
		Func_(f),
		PropagateInferiorResults_(propagate_inferior_results)
	{
	}

	tsp_monad operator()(tsp_monad::value_type t)
	{
		if (0 == NumberOfTasksInParallel_)			
			return unit(t);
		
		std::vector<std::future<tsp_async_state>> tsp_solutions;

		std::cout << "START SOLUTION:" << std::endl;
		
		auto tsp_instance = ref(t);
		display_solution(tsp_instance);

		for (unsigned int i = 0; i < NumberOfTasksInParallel_; ++i)
		{
			//async
			tsp_solutions.push_back(std::async(std::launch::async, [&, i, tsp_instance] 
			{
				Maybe a = just(tsp_instance);
				std::unique_ptr<ForkJoin_args> args(new ForkJoin_args(i, NumberOfTasksInParallel_));
				a.second = args.get();
				return tsp_async_state(Func_(a));
			}));
		}

		std::cout << "CANDIDATE SOLUTIONS:" << std::endl;
		std::vector<tsp_class> candidate_solutions;
		int i = 0;
		for (auto& tsp_solution : tsp_solutions)
		{
			//async result
			auto candidate = tsp_solution.get();
			if (has(candidate.state))
			{
				candidate_solutions.push_back(ref(candidate.state));
				display(ref(candidate.state), false);
			}
		}

		std::cout << "CANDIDATE LENGTHS:" << std::endl;
		for (auto& solution : candidate_solutions)
			std::cout << solution.do_cycle_length() << std::endl;

		std::sort(candidate_solutions.begin(), candidate_solutions.end(), 
			[](const tsp_class& left, const tsp_class& right) 
				{ return left.do_cycle_length() < right.do_cycle_length(); });

		std::cout << "CANDIDATE LENGTHS (ORDERED):" << std::endl;
		for (auto& solution : candidate_solutions)
			std::cout << solution.do_cycle_length() << std::endl;

		std::cout << "SELECTED SOLUTION:" << std::endl;
		auto result = Propagator(unit(t), unit(just(*candidate_solutions.begin())), PropagateInferiorResults_);
		display_solution(ref(result));
		return result;
	}

	ForkJoin() = delete;
	ForkJoin(const ForkJoin&) = default;
	ForkJoin& operator=(const ForkJoin&) = delete;

private:
	unsigned int NumberOfTasksInParallel_;
	F Func_;
	bool PropagateInferiorResults_;

	static void display_solution(tsp_class& tsp_instance)
	{
		display(tsp_instance, true);
		std::cout << tsp_instance.do_cycle_length() << std::endl;
	}

	struct tsp_async_state
	{
		tsp_async_state() 
			: state(unit(nothing())) {}
		explicit tsp_async_state(const tsp_monad& state) 
			: state(state) {}
		tsp_monad state;
	};
};

struct Generations final
{
	typedef tsp_monad::function_type F;

	Generations(unsigned int number_of_generations, F f) : 
		NumberOfGenerations_(number_of_generations),
		Func_(f)
	{
	}

	tsp_monad operator()(tsp_monad::value_type value)
	{
		tsp_monad::value_type result = value;
		for(unsigned int g = 0; g < NumberOfGenerations_; ++g) 
		{
			std::cout << "GENERATION:" << g + 1 << std::endl;
			auto new_result = Func_(result);			
			result = just(ref(new_result));
		}
		return unit(result);
	}

	Generations() = delete;
	Generations(const Generations&) = default;
	Generations& operator=(const Generations&) = delete;

private:
	unsigned int NumberOfGenerations_;
	F Func_;
};

struct Identity final
{
	tsp_monad operator()(tsp_monad::value_type value)
	{
		return unit(value);
	}

	Identity() = default;
	Identity(const Identity&) = default;
	Identity& operator=(const Identity&) = delete;
};

#endif /* _tsp_functors_ */