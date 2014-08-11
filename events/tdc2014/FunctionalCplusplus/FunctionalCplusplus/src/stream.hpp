//Sample provided by Fabio Galuppo 
//August 2014
//Part of Monadic types prototype for C++

#pragma once 

#ifndef __stream__
#define __stream__

#include <memory>
#include <algorithm>
#include <iterator>
#include <utility>
#include <numeric>
#include <tuple>

//http://docs.oracle.com/javase/8/docs/api/java/util/stream/Stream.html

template<typename T, template <typename, typename> class ContainerT>
struct stream;

template<typename U, template <typename, typename> class ContainerU>
auto make_stream(const ContainerU<U, std::allocator<U>>& xs) -> stream<U, ContainerU>
{
	stream<U, ContainerU> result;
	std::copy(std::begin(xs), std::end(xs), std::back_inserter(result.C));
	return result;
}

template<typename U, template <typename, typename> class ContainerU>
auto make_stream(ContainerU<U, std::allocator<U>>&& xs) -> stream<U, ContainerU>
{
	return stream<U, ContainerU>(std::move(xs));
}

//warning: in this implementation all intermediates are eager
template<typename T, template <typename, typename> class ContainerT>
struct stream final
{
	template<typename U = T, template <typename, typename> class ContainerU = ContainerT, class Function>
	auto flat_map(Function f) const -> stream<U, ContainerU> //intermediate
	{
		ContainerU<U, std::allocator<U>> result;
		for (const auto& x : C)
		{
			const auto& fs = f(x);
			std::copy(std::begin(fs), std::end(fs), std::back_inserter(result));
		}
		return make_stream(std::move(result));
	}

	template<typename U = T, template <typename, typename> class ContainerU = ContainerT, class Function>
	auto map(Function f) const -> stream<U, ContainerU> //intermediate
	{
		ContainerU<U, std::allocator<U>> result;
		std::transform(std::begin(C), std::end(C), std::back_inserter(result), f);
		return make_stream(std::move(result));
	}

	template<typename U = T, template <typename, typename> class ContainerU = ContainerT, class UnaryPredicate>
	auto filter(UnaryPredicate pred) const -> stream<U, ContainerU> //intermediate
	{
		//eager version

		ContainerU<U, std::allocator<U>> result;
		std::copy_if(std::begin(C), std::end(C), std::back_inserter(result), pred);
		return make_stream(std::move(result));
	}

	template<class Function>
	auto for_each(Function f) const -> void //terminal
	{
		for (const auto& x : C) f(x);
	}

	template<typename U = T, class BinaryOperation>
	auto reduce(U init, BinaryOperation binary_op) const -> U //terminal
	{
		return std::accumulate<decltype(C.begin()), U>(std::begin(C), std::end(C), init, binary_op);
	}

	template<typename U = T, template <typename, typename> class ContainerU = ContainerT>
	auto zip(const ContainerU<U, std::allocator<U>>& xs) const -> ContainerU<std::tuple<T, U>, std::allocator<std::tuple<T, U>>> //terminal
	{
		ContainerU<std::tuple<T, U>, std::allocator<std::tuple<T, U>>> result;
		auto first1 = std::begin(C);
		auto last1 = std::end(C);
		auto first2 = std::begin(xs);
		auto last2 = std::end(xs);

		while (first1 != last1 && first2 != last2)
		{
			result.push_back(std::make_tuple(*first1, *first2));
			++first1;
			++first2;
		}
		return std::move(result);
	}

	template<typename U = T, template <typename, typename> class ContainerU = ContainerT>
	auto zip(const stream<U, ContainerU>& xs) const -> ContainerU<std::tuple<T, U>, std::allocator<std::tuple<T, U>>> //terminal
	{
		return std::move(zip(xs.C));
	}

	template<template <typename, typename> class ContainerU = ContainerT>
	auto to_container() const -> ContainerU<T, std::allocator<T>> //terminal
	{
		ContainerU<T, std::allocator<T>> result;
		std::copy(std::begin(C), std::end(C), std::back_inserter(result));
		return std::move(result);
	}

	auto sorted() const -> stream<T, ContainerT> //intermediate
	{
		auto result = to_container();
		std::sort(std::begin(result), std::end(result));
		return make_stream(std::move(result));
	}

	template<class Compare>
	auto sorted(Compare comp) const -> stream<T, ContainerT> //intermediate
	{
		auto result = to_container();
		std::sort(std::begin(result), std::end(result), comp);
		return make_stream(std::move(result));
	}

	template<typename U, template <typename, typename> class ContainerU>
	friend auto make_stream(const ContainerU<U, std::allocator<U>>& xs) -> stream<U, ContainerU>;

	template<typename U, template <typename, typename> class ContainerU>
	friend auto make_stream(ContainerU<U, std::allocator<U>>&& xs) -> stream<U, ContainerU>;

	stream(const stream&) = default;
	~stream() = default;

private:
	stream& operator=(const stream&) = delete;
	stream() = default;
	stream(ContainerT<T, std::allocator<T>>&& xs)
		: C(std::move(xs)) {}

	ContainerT<T, std::allocator<T>> C;
};

#endif //__stream__