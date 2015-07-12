//Sample provided by Fabio Galuppo 
//July 2015

#ifndef _measure_
#define _measure_

#include "stop_watch.hpp"

template<typename ElapsedPolicy>
struct Elapsed : private ElapsedPolicy
{
	double fun(stop_watch& sw) const { return elapsed_function(sw); }
	const char* sym() const { return elapsed_symbol(); }

private:
	using ElapsedPolicy::elapsed_function;
	using ElapsedPolicy::elapsed_symbol;
};

struct ElapsedPolicySeconds
{
protected:
	double elapsed_function(stop_watch& sw) const { return double(sw.elapsed_s().count()); }
	const char* elapsed_symbol() const { return "s"; }
};

struct ElapsedPolicyMilliseconds
{
protected:
	double elapsed_function(stop_watch& sw) const { return double(sw.elapsed_ms().count()); }
	const char* elapsed_symbol() const { return "ms"; }
};

struct ElapsedPolicyMicroseconds
{
protected:
	double elapsed_function(stop_watch& sw) const { return double(sw.elapsed_us().count()); }
	const char* elapsed_symbol() const { return "us"; }
};

struct ElapsedPolicyNanoseconds
{
protected:
	double elapsed_function(stop_watch& sw) const { return double(sw.elapsed_ns().count()); }
	const char* elapsed_symbol() const { return "ns"; }
};

#include <functional>
#include <string>
#include <sstream>
#include <iomanip>

template <class ElapsedPolicy>
std::string do_measure(std::function<void(void)> f, const char* text, double& elapsed)
{
	Elapsed<ElapsedPolicy> e;
	stop_watch sw;
	f();
	elapsed = e.fun(sw);
	std::stringstream ss;
	ss << std::setw(7) << elapsed << " " << e.sym() << " " << text;
	return ss.str();
}

using ElapsedSeconds = Elapsed<ElapsedPolicySeconds>;
using ElapsedMilliseconds = Elapsed<ElapsedPolicyMilliseconds>;
using ElapsedMicroseconds = Elapsed<ElapsedPolicyMicroseconds>;
using ElapsedNanoseconds = Elapsed<ElapsedPolicyNanoseconds>;

#endif /* _measure_ */