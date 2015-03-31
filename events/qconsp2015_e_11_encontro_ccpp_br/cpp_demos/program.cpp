//Sample provided by Fabio Galuppo  
//March 2015  

#include "util.hpp"

#include <type_traits>
#include <string>
#include <vector>
#include <algorithm>
#include <numeric>
#include <functional>

//Regular type is Default Constructible, Assignable, and Equality Comparable
//InterestRate is a regular type
template <typename T>
struct InterestRate final
{
	//static_assert(std::is_arithmetic<T>::value, "not an arithmetic type");
	static_assert(std::is_floating_point<T>::value, "not an floating point type");

	//explicit constructor
	explicit InterestRate(T rate)
		: Rate_(rate)
	{
	}

	//default constructor
	InterestRate()
		: InterestRate(T(0)) 
	{
	} 

	//calculate interest rate on single period
	T operator()(T value) const
	{
		T result = value * (T(1) + Rate_);
		return result;
	}
	
	//equality comparer
	bool operator==(const InterestRate<T>& that) const 
	{ 
		return Rate_ == that.Rate_; 
	}
	
	//inequality comparer in terms of equality comparer
	bool operator!=(const InterestRate<T>& that) const
	{ 
		return !(*this == that); 
	} 

	InterestRate(const InterestRate<T>&) = default; //copy constructor
	InterestRate<T>& operator=(const InterestRate<T>&) = default; //copy assignment
	~InterestRate() = default; //destructor

	template <typename U>
	friend std::wstring std::to_wstring(const InterestRate<U>& _Val);

private:
	T Rate_;
};

namespace std
{
	template <typename T>
	std::wstring to_wstring(const InterestRate<T>& _Val)
	{
		return std::to_wstring(_Val.Rate_ * T(100)) + L"%";
	}
}

using interest_rate = InterestRate<double>; //an alias

#include <iostream>
#include <functional>

template <typename T>
inline std::function<T(T)> interest_rate_hof(T rate) 
{
	static_assert(std::is_floating_point<T>::value, "not an floating point type");
	return [rate](T value) {
		T result = value * (T(1) + rate);
		return result;
	};
}

template<class T, class U, class V>
inline std::function<V(T)> compose(std::function<V(U)> g, std::function<U(T)> f)
{
	return [=](T x) -> V { return g(f(x)); };
}

template<class Container, typename T, class Function> 
T foldl (Function f, T init, const Container& xs)
{
 	//T acc = init;
	//for (const auto& x : xs) acc = f(acc, x);
	//return acc;
	//or:
	using F = typename Container::value_type;
	return std::accumulate(xs.cbegin(), xs.cend(), init, [](T acc, const F& f){ return f(acc); });
}

void interest_rate_sample()
{
	interest_rate p1(0.065);
	double amount = 5000.;
	printLn(std::to_wstring(amount) + L" is the amount");

	interest_rate p2;
	p2 = interest_rate(0.1);
	amount = p1(amount);
	printLn(std::to_wstring(amount) + L" is the new amount after p1");
	printLn(std::to_wstring(p1) + L" is the interest rate in period 1");
	printLn(std::to_wstring(p2) + L" is the interest rate in period 2");
	printLn(std::wstring(L"p1 == p2? ") + (p1 == p2 ? L"true" : L"false"));
	
	amount = p2(amount);
	printLn(std::to_wstring(amount) + L" is the new amount after p2");

	/////////////
	printLn();
	
	amount = 5000.;
	printLn(std::to_wstring(amount) + L" is the amount");
	
	std::function<double(double)> fp1 = p1;

	double rate = 0.1;
	auto fp2 = [rate](double value) {
		double result = value * (double(1) + rate);
		return result;
	};

	amount = fp2(fp1(amount));
	printLn(std::to_wstring(amount) + L" is the new amount after fp1 and fp2");

	/////////////
	printLn();

	amount = 5000.;
	printLn(std::to_wstring(amount) + L" is the amount");

	auto hofp1 = interest_rate_hof(0.065);
	auto hofp2 = interest_rate_hof(0.1);
	auto _p2_p1 = compose(hofp2, hofp1);
	amount = _p2_p1(amount);
	
	printLn(std::to_wstring(amount) + L" is the new amount after applying the composition of hofp1 and hofp2");

	amount = 5000.;
	auto _p2_p1_bnd = std::bind(hofp2, std::bind(hofp1, std::placeholders::_1));
	amount = _p2_p1_bnd(amount);
	printLn(std::to_wstring(amount) + L" is the new amount after applying the std::bind to hofp1 and hofp2");

	/////////////
	printLn();

	amount = 5000.;
	printLn(std::to_wstring(amount) + L" is the amount");

	std::vector<interest_rate> periods{ p1, p2, interest_rate(0.07), interest_rate(0.045) };
	for (auto& p : periods)
		amount = p(amount);

	printLn(periods);
	printLn(std::to_wstring(amount) + L" is the new amount after a sequence of periods");

	amount = 5000.;
	std::for_each(periods.cbegin(), periods.cend(), [&amount](const interest_rate& p){ amount = p(amount); });
	printLn(std::to_wstring(amount) + L" is the new amount after a sequence of periods");

	amount = 5000.;
	amount = std::accumulate(periods.cbegin(), periods.cend(), amount, [](double amount, const interest_rate& p){ return p(amount); });
	printLn(std::to_wstring(amount) + L" is the new amount after a sequence of periods");

	amount = 5000.;
	amount = foldl([](double amount, const interest_rate& p){ return p(amount); }, amount, periods);
	printLn(std::to_wstring(amount) + L" is the new amount after a sequence of periods");
	
	/////////////
	printLn();
}

#include <random>
#include <ctime>

template <typename TDistribution>
struct randomizer final
{
	randomizer(const TDistribution& distr, unsigned seed = static_cast<unsigned>(std::time(nullptr))) :
		Engine_(seed),
		Distr_(distr)
	{
	}

	double next() 
	{ 
		return Distr_(Engine_); 
	}

	int next(int minInclusive, int maxInclusive) 
	{ 
		return clamp(static_cast<int>(Distr_(Engine_)), minInclusive, maxInclusive); 
	}
	
private:
	int clamp(int value, int minInclusive, int maxInclusive)
	{
		if (value < minInclusive) return minInclusive;
		if (value > maxInclusive) return maxInclusive;
		return value;
	}

	std::default_random_engine Engine_;
	TDistribution Distr_;
};

template <typename TDistribution, typename ... Args>
randomizer<TDistribution> make_randomizer(Args&&... args)
{
	return randomizer<TDistribution>(TDistribution{ std::forward<Args>(args)... });
}

template <typename TDistribution>
void print_histogram(randomizer<TDistribution> rnd, int iterations, const wchar_t* title)
{
	const int MAX = 9;
	
	std::vector<int> histogram(MAX + 1);
	for (int i = 0; i < iterations; ++i)
		++histogram[rnd.next(0, MAX)];

	printLn(title);
	for (std::vector<int>::size_type i = 0; i < histogram.size(); ++i)
		printLn(std::to_wstring(i) + L": " + std::wstring(histogram[i] * 100 / iterations, L'*'));
	printLn();
}

void histogram_sample()
{
	const int MAX_ITER = 10000;
	
	print_histogram(make_randomizer<std::normal_distribution<>>(5., 2.), MAX_ITER, L"normal:");
	print_histogram(make_randomizer<std::geometric_distribution<>>(0.4), MAX_ITER, L"geometric:");
	print_histogram(make_randomizer<std::uniform_int_distribution<>>(0, 9), MAX_ITER, L"uniform:");
	print_histogram(make_randomizer<std::binomial_distribution<>>(9, 0.5), MAX_ITER, L"binomial:");
	print_histogram(make_randomizer<std::poisson_distribution<>>(3.75), MAX_ITER, L"poisson:");
}

#include <future>
#include <chrono>

void future_sample()
{
	const int N = 8;

	std::vector<std::future<int>> fs;
	fs.reserve(N);

	for (int i = 0; i < N; ++i)
	{
		std::future<int> f = std::async([&, i]() {
			std::this_thread::sleep_for(std::chrono::seconds(2));
			return (1 + i) * 100;
		});
		
		fs.push_back(std::move(f));
	}

	std::vector<int> xs(N);
	std::transform(fs.begin(), fs.end(), xs.begin(), [](std::future<int>& f) { 
		return f.get(); 
	});

	printLn(xs);
	printLn();
}

int main()
{
	interest_rate_sample();
	histogram_sample();
	future_sample();

	std::cin.get();
}