//Sample provided by Fabio Galuppo 
//August 2014
//Part of Monadic types prototype for C++

#include <algorithm>
#include <numeric>
#include <iterator>
#include <vector>
#include <tuple>
#include <utility>
#include <iostream>

#include "basic_functions.hpp"

using Z = std::vector<int>;
using ZxZ_Item = std::tuple <int, int> ;
using ZxZ = std::vector<ZxZ_Item>;

void print(const char* function_name, const Z& input_domain, const ZxZ& mapping_domain, const Z& output_domain)
{
	auto mkStr = [](const Z& xs){ return mkString(xs, "{", ", ", "}"); };
	auto mkStr2 = [](const ZxZ& xs){ 
		std::vector<std::string> ys;
		std::transform(xs.begin(), xs.end(), std::back_inserter(ys), [](const ZxZ_Item& x) {
			std::vector<int> temp{ std::get<0>(x), std::get<1>(x) };
			return mkString(temp, "(", ", ", ")");
		});
		return mkString(ys, "{", ", ", "}"); 
	};
	std::cout << function_name << ": " << mkStr(input_domain) << " -- " << mkStr2(mapping_domain) 
		<< " --> " << mkStr(output_domain) << "\n";
}

Z&& f(const Z& input, const Z& expected_output)
{
	ZxZ result; //mapping f: input -> output

	//Injective (one-to-one) and surjective (onto), also bijective = X -> Y1
	//X.map(x -> 2 * x)
	std::transform(input.begin(), input.end(), std::back_inserter(result), [](int x){ return std::make_tuple(x, 2 * x); });
	print("f", input, result, expected_output);

	Z output;
	std::transform(result.begin(), result.end(), std::back_inserter(output), [](const ZxZ_Item& x){ return std::get<1>(x); });
	return std::move(output);
}

Z&& g(const Z& input, const Z& expected_output)
{
	ZxZ result; //mapping g: input -> output

	//Injective and non-surjective X -> Y2
	//X.map(x -> 10 * x)
	std::transform(input.begin(), input.end(), std::back_inserter(result), [](int x){ return std::make_tuple(x, 10 * x); });
	print("g", input, result, expected_output);

	Z output;
	std::transform(result.begin(), result.end(), std::back_inserter(output), [](const ZxZ_Item& x){ return std::get<1>(x); });
	return std::move(output);
}

Z&& h(const Z& input, const Z& expected_output)
{
	ZxZ result; //mapping h: input -> output

	//Non-injective and surjective X -> X
	//X.map(x -> x > 2 ? 3 : x)
	std::transform(input.begin(), input.end(), std::back_inserter(result), [](int x){ return std::make_tuple(x, x > 2 ? 3 : x); });
	print("h", input, result, expected_output);

	Z output;
	std::transform(result.begin(), result.end(), std::back_inserter(output), [](const ZxZ_Item& x){ return std::get<1>(x); });
	return std::move(output);
}

Z&& m(const Z& input, const Z& expected_output)
{
	ZxZ result; //mapping m: input -> output

	//Non-injective and non-surjective X -> Y2
	//X.map(x -> x > 2 ? 3 : x)
	std::transform(input.begin(), input.end(), std::back_inserter(result), [](int x){ return std::make_tuple(x, x > 2 ? 50 : 10 * x); });
	print("m", input, result, expected_output);

	Z output;
	std::transform(result.begin(), result.end(), std::back_inserter(output), [](const ZxZ_Item& x){ return std::get<1>(x); });
	return std::move(output);
}

Z&& n(const Z& input, const Z& expected_output)
{
	ZxZ result; //mapping n: input -> output

	//injective partial function X -> X
	//X.filter(x -> x > 2)
	Z partial_result;
	std::copy_if(input.begin(), input.end(), std::back_inserter(partial_result), [](int x){ return x > 2; });
	std::transform(partial_result.begin(), partial_result.end(), std::back_inserter(result), [](int x){ return std::make_tuple(x, x); });
	print("n", input, result, expected_output);

	Z output;
	std::transform(result.begin(), result.end(), std::back_inserter(output), [](const ZxZ_Item& x){ return std::get<1>(x); });
	return std::move(output);
}

Z&& o(const Z& input, const Z& expected_output)
{
	ZxZ result; //mapping o: input -> output

	//total function that is not-injective X -> Y4
	//X.reduce(0, (acc, x) -> acc + x)
	auto total = std::accumulate(input.begin(), input.end(), 0, [](int acc, int x) { return acc + x; });
	std::transform(input.begin(), input.end(), std::back_inserter(result), [total](int x){ return std::make_tuple(x, total); });

	print("o", input, result, expected_output);

	Z output;
	std::transform(result.begin(), result.end(), std::back_inserter(output), [](const ZxZ_Item& x){ return std::get<1>(x); });
	return std::move(output);
}

void run_basic_functions_and_mappings()
{
	//Let's apply some basic concepts from Set Theory...
	
	Z X{ 1, 2, 3, 4 }; //X = {1, 2, 3, 4}
	Z Y1{ 2, 4, 6, 8 }; //Y1 = {2, 4, 6, 8}
	Z Y2{ 10, 20, 30, 40, 50 }; //Y2 = {10, 20, 30, 40, 50}
	Z Y3{ 1, 2, 3 }; //Y3 = {1, 2, 3}
	Z Y4{ 10 }; //Y4 = {10}

	f(X, Y1);
	g(X, Y2);
	h(X, Y3);
	m(X, Y2);
	n(X, X);
	o(X, Y4);
}