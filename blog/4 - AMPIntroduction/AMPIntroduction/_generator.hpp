//Sample provided by Fabio Galuppo
#pragma once

#include <random>

using std::ranlux64_base_01;
using std::normal_distribution;

typedef ranlux64_base_01 RandEngineType;
typedef normal_distribution<float> RandDistributionType;

template<typename generator_policy>
struct Generator : public generator_policy
{
	using generator_policy::next_core;

	const float next()
	{
		return next_core();
	}
};

struct NormalGenerator_Policy
{
	NormalGenerator_Policy();	
		
protected:
	const float next_core();

private:
	RandEngineType eng;
	RandDistributionType distr;
};

typedef Generator<NormalGenerator_Policy> NormalGenerator;

struct PositiveIntegerGenerator_Policy
{
	PositiveIntegerGenerator_Policy();

protected:
	const float next_core();

private:
	float counter;
};

typedef Generator<PositiveIntegerGenerator_Policy> PositiveIntegerGenerator;

struct RandGenerator_Policy
{
	RandGenerator_Policy();

protected:
	const float next_core();
};

typedef Generator<RandGenerator_Policy> RandGenerator;