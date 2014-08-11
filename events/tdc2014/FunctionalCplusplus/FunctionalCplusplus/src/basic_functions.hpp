//Sample provided by Fabio Galuppo 
//August 2014
//Part of Monadic types prototype for C++

#pragma once 

#ifndef __basic_functions__
#define __basic_functions__

#include <algorithm>
#include <string>
#include <sstream>
#include <iterator>
#include <utility>

//Scala mkString:
/*
def  mkString(start: String, sep: String, end: String): String
def  mkString(sep: String): String
def  mkString: String
*/
template<class Container>
std::string mkString(const Container& xs)
{
	std::stringstream ss;
	for (const auto& x : xs) ss << x;
	return std::move(ss.str());
}

template<class Container>
std::string mkString(const Container& xs, std::string sep)
{
	std::stringstream ss;
	auto first = std::begin(xs);
	auto last = std::end(xs);
	auto length = std::distance(first, last);
	if (length > 0)
	{
		typename Container::size_type n = length - 1;
		while (n != 0 && first != last)
		{
			ss << *first << sep;
			++first;
			--n;
		}
		ss << *first;
	}
	return std::move(ss.str());
}

template<class Container>
std::string mkString(const Container& xs, std::string start, std::string sep, std::string end)
{
	return std::move(start + mkString(xs, sep) + end);
}

//Haskell take:
//take :: Int -> [a] -> [a]
template<class Container>
Container take(typename Container::size_type n, const Container& xs)
{
	//requires: cbegin, cend, push_back, move ctor
	
	Container result;
	auto first = std::begin(xs);
	auto last = std::end(xs);
	typename Container::difference_type length = std::distance(first, last);	
	if (length > 0)
	{
		while (n != 0 && first != last)
		{
			result.push_back(*first);
			++first;
			--n;
		}
	}
	
	return std::move(result);
}

//Haskell drop:
//drop :: Int -> [a] -> [a]
template<class Container>
Container drop(typename Container::size_type n, const Container& xs)
{
	Container result;
	auto first = std::begin(xs);
	auto last = std::end(xs);
	typename Container::difference_type length = std::distance(first, last);
	while (n != 0 && first != last)
	{
		++first;
		--n;
	}
	
	while (first != last)
	{
		result.push_back(*first);
		++first;
	}

	return std::move(result);
}

#include <stdexcept>

struct empty_container_exception final : public std::runtime_error
{
	empty_container_exception(std::string function_name) 
		: std::runtime_error(function_name + ": empty container"){}
};

//Haskell head:
//head :: [a] -> a
template<class Container>
typename Container::value_type head(const Container& xs)
{
	auto first = std::begin(xs);
	if (std::distance(first, std::end(xs)) <= 0)
		throw empty_container_exception("head");
	return *first;
}

//Haskell tail:
//tail :: [a] ->[a]
template<class Container>
Container tail(const Container& xs)
{
	if (std::distance(std::begin(xs), std::end(xs)) <= 0)
		throw empty_container_exception("tail");
	return std::move(drop(1, xs));
}

//Haskell init:
//init :: [a] -> [a]
template<class Container>
Container init(const Container& xs)
{
	typename Container::difference_type length = std::distance(std::begin(xs), std::end(xs));
	if (length <= 0)
		throw empty_container_exception("init");
	return std::move(take(length - 1, xs));
}

//Haskell last:
//last :: [a] -> a
template<class Container>
typename Container::value_type last(const Container& xs)
{
	typename Container::difference_type length = std::distance(std::begin(xs), std::end(xs));
	if (length <= 0)
		throw empty_container_exception("last");	
	return *std::begin(drop(length - 1, xs));
}

//F# Seq.iter
//Seq.iter : ('T -> unit) -> seq<'T> -> unit
template<class Container, class Function>
void iter(Function f, const Container& xs)
{
	typename Container::size_type i = 0;
	for (const auto& x : xs) f(x);
}

//F# Seq.iteri:
//Seq.iteri : (int -> 'T -> unit) -> seq<'T> -> unit
template<class Container, class Function>
void iteri(Function f, const Container& xs)
{
	typename Container::size_type i = 0;
	for (const auto& x : xs) f(i++, x);
}

//F# Seq.fold:
//Seq.fold : ('State -> 'T -> 'State) -> 'State -> seq<'T> -> 'State
template<class Container, typename T, class Function>
T fold(Function f, T init, const Container& xs)
{
	T acc = init;
	for (const auto& x : xs) acc = f(acc, x);
	return acc;
}

#endif //__basic_functions__