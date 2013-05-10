//Sample provided by Fabio Galuppo
//May 2013

#include <array>
#include <cmath>
#include <functional>
#include <algorithm>
#include <utility>
#include <random>
#include <memory>
#include <ctime>
#include <chrono>
#include <iostream>

template<int N>
struct tsp_class 
{
	typedef std::pair<int /* x */, int /* y */> city_location;
	typedef std::function<int()> rand_function;

	std::array<city_location, N> cities;

	tsp_class()
	{
	}

	tsp_class(const tsp_class& that)
	{
		cities = that.cities;
	}

	auto operator=(const tsp_class& that) -> tsp_class&
	{
		if (this != &that)
			cities = that.cities;
		return *this;
	}
  
	auto do_cycle_lenght() const -> double
	{
		double temp = euclidean_distance(cities[N - 1], cities[0]);

		for (int i = 0, l = N - 1; i < l; ++i)
			temp += euclidean_distance(cities[i], cities[i + 1]);

		return temp;
	}

	auto do_brute_force() -> void
	{
		std::array<int, N> a;
		for(int i = 0; i < N; ++i)
			a[i] = i;

		double minimum_length = do_cycle_lenght();
		tsp_class<N> minimum_tsp = *this;
		
		int count = 0;
		do
		{
			tsp_class<N> temp;
			for(int i = 0; i < N; ++i)
				temp.cities[i] = cities[a[i]];
			
			//debug purpose
			/*std::cout << (++count) << " : ";
			for(auto city : temp.cities)
				std::cout << "(" << city.first << ", " << city.second << ") : ";
			std::cout << temp.do_cycle_lenght() << std::endl;*/

			double temp_cycle_lenght = temp.do_cycle_lenght();

			if (temp_cycle_lenght < minimum_length)
			{
				minimum_length = temp_cycle_lenght;
				minimum_tsp = temp;
			}
		}
		while(std::next_permutation(a.begin(), a.end()));

		std::cout << "optimal : ";
		cities = minimum_tsp.cities;
		for(auto city : cities)
			std::cout << "(" << city.first << ", " << city.second << ") : ";
		std::cout << do_cycle_lenght() << std::endl;
	}

private:


	static auto euclidean_distance(city_location c1, city_location c2) -> double
	{
		auto x1 = c1.first;
		auto y1 = c1.second;
		auto x2 = c2.first;
		auto y2 = c2.second;
		auto dx = std::abs(x1 - x2);
		auto dy = std::abs(y1 - y2);
		return std::sqrt(dx * dx + dy * dy);
	}
};

struct random_functor
{
	random_functor(int min_inclusive, int max_inclusive) : 
		Engine_(new std::default_random_engine(static_cast<unsigned>(std::time(nullptr)))),
		Rnd_(new std::uniform_int_distribution<int>(min_inclusive, max_inclusive))
	{
	}

	random_functor(int max_exclusive) :
		Engine_(new std::default_random_engine(static_cast<unsigned>(std::time(nullptr)))),
		Rnd_(new std::uniform_int_distribution<int>(0, max_exclusive - 1))
	{
	}

	//[0; max_inclusive]
	auto operator()() const -> int { return (*Rnd_)(*Engine_); }

private:
	std::shared_ptr<std::default_random_engine> Engine_;
	std::shared_ptr<std::uniform_int_distribution<int>> Rnd_;
};

template<int N>
auto setup_environment(tsp_class<N>& tsp_instance, std::function<int(void)> rnd_location) -> void
{
	for (int i = 0; i < N; ++i) 
		tsp_instance.cities[i] = std::make_pair(rnd_location(), rnd_location());
}

struct stop_watch
{
	stop_watch() : 
		Start_(now()) 
	{
	}

	auto elapsed_ms() const -> std::chrono::milliseconds
	{
		return std::chrono::duration_cast<std::chrono::milliseconds>(now() - Start_);
	}

	void restart() { Start_ = now(); }

private:
	static auto now() -> std::chrono::high_resolution_clock::time_point
	{ 
		return std::chrono::high_resolution_clock::now(); 
	}

	std::chrono::high_resolution_clock::time_point Start_;
};

template<int N>
auto mathematica_graph_plot(tsp_class<N>& tsp_instance) -> void
{
	std::cout << "GraphPlot[{";
	for (int i = 0, l = N - 1; i < l; ++i)
		std::cout << i << " -> " << i + 1 << ", ";
	std::cout << N - 1 <<  " -> " << 0 << "}, ";
	std::cout << "VertexCoordinateRules -> {";
	for (int i = 0, l = N - 1; i < l; ++i)
	{
		auto& city = tsp_instance.cities[i];
		std::cout << i << " -> {" << city.first << ", " << city.second << "}, ";
	}
	auto& city = tsp_instance.cities[N - 1];
	std::cout << N - 1 << " -> {" << city.first << ", " << city.second << "}}]" << std::endl;
}

template<int N>
auto display(tsp_class<N>& tsp_instance, bool emit_mathematica_graph_plot = false) -> void
{
	for(auto city: tsp_instance.cities)
		std::cout << "(" << city.first << ", " << city.second << ") : ";
	std::cout << tsp_instance.do_cycle_lenght() << std::endl;
	
	if(emit_mathematica_graph_plot)
		mathematica_graph_plot(tsp_instance);
}

auto main() -> int
{
	const int N = 12;
	tsp_class<N> tsp_instance;
	setup_environment(tsp_instance, random_functor(1, 100));
	
	stop_watch sw;

	std::cout << "START SOLUTION:" << std::endl;
	display(tsp_instance, true);

	tsp_instance.do_brute_force();

	std::cout << "OPTIMAL SOLUTION:" << std::endl;
	display(tsp_instance, true);

	std::cout << sw.elapsed_ms().count() << " ms";
}