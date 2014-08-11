//Sample provided by Fabio Galuppo 
//August 2014
//Part of Monadic types prototype for C++

#pragma once 

#ifndef __container_monad_ex__
#define __container_monad_ex__

#include <memory>
#include <algorithm>
#include <iterator>
#include <utility>

template<typename T, template <typename, typename> class ContainerT>
struct container_monad_ex;

template<typename U, template <typename, typename> class ContainerU>
auto make_monad(const ContainerU<U, std::allocator<U>>& xs) -> container_monad_ex<U, ContainerU>
{
	container_monad_ex<U, ContainerU> result;
	std::copy(std::begin(xs), std::end(xs), std::back_inserter(result.C));
	return result;
}

template<typename U, template <typename, typename> class ContainerU>
auto make_monad(ContainerU<U, std::allocator<U>>&& xs) -> container_monad_ex<U, ContainerU>
{
	return container_monad_ex<U, ContainerU>(std::move(xs));
}

template<typename T, template <typename, typename> class ContainerT>
struct container_monad_ex final
{
	template<typename U = T, template <typename, typename> class ContainerU = ContainerT, class Function>
	auto flat_map(Function f) const -> container_monad_ex<U, ContainerU>
	{
		ContainerU<U, std::allocator<U>> result;
		for (const auto& x : C)
		{
			const auto& fs = f(x);
			std::copy(std::begin(fs), std::end(fs), std::back_inserter(result));
		}
		return make_monad(std::move(result));
	}

	template<typename U = T, template <typename, typename> class ContainerU = ContainerT, class Function>
	auto map(Function f) const -> container_monad_ex<U, ContainerU>
	{
		ContainerU<U, std::allocator<U>> result;
		std::transform(std::begin(C), std::end(C), std::back_inserter(result), f);
		return make_monad(std::move(result));
	}

	template<class Function>
	auto for_each(Function f) const -> void 
	{ 
		for (const auto& x : C) f(x); 
	}

	template<typename U, template <typename, typename> class ContainerU>
    friend auto make_monad(const ContainerU<U, std::allocator<U>>& xs) -> container_monad_ex<U, ContainerU>;

	template<typename U, template <typename, typename> class ContainerU>
    friend auto make_monad(ContainerU<U, std::allocator<U>>&& xs) -> container_monad_ex<U, ContainerU>;

	container_monad_ex(const container_monad_ex&) = default;
	~container_monad_ex() = default;

private:
	container_monad_ex& operator=(const container_monad_ex&) = delete;
	container_monad_ex() = default;
	container_monad_ex(ContainerT<T, std::allocator<T>>&& xs)
		: C(std::move(xs)) {}

	ContainerT<T, std::allocator<T>> C;
};

#endif //__container_monad_ex__