//Sample provided by Fabio Galuppo
#include "_generator.hpp"

//NormalGenerator policy
NormalGenerator_Policy::NormalGenerator_Policy() : distr(3.0f /* mean */, 4.0f /* sigma */)
{
}
	
const float NormalGenerator_Policy::next_core() 
{ 
	return distr(eng); 
}

//PositiveIntegerGenerator policy
PositiveIntegerGenerator_Policy::PositiveIntegerGenerator_Policy() : counter(1.0)
{
}

const float PositiveIntegerGenerator_Policy::next_core() 
{ 
	return counter++; 
}

#include <time.h>

//RandGenerator policy
RandGenerator_Policy::RandGenerator_Policy()
{
	time_t t;
	time(&t);
	tm ti;
	localtime_s(&ti, &t);
	
	srand(1900 + ti.tm_year);
}

const float RandGenerator_Policy::next_core()
{
	return static_cast<float>(rand()) / RAND_MAX;
}