//Sample provided by Fabio Galuppo 
//August 2014
//Part of Monadic types prototype for C++

#pragma once 

#ifndef __container_monad__
#define __container_monad__

#include <memory>
#include <algorithm>
#include <iterator>
#include <utility>

template<typename T, template <typename, typename> class ContainerT>
struct container_monad;

template<typename U, template <typename, typename> class ContainerU>
auto unit(const ContainerU<U, std::allocator<U>>& xs) -> container_monad<U, ContainerU>
{
	container_monad<U, ContainerU> result;
	std::copy(std::begin(xs), std::end(xs), std::back_inserter(result.C));
	return result;
}

template<typename U, template <typename, typename> class ContainerU>
auto unit(ContainerU<U, std::allocator<U>>&& xs) -> container_monad<U, ContainerU>
{
	return container_monad<U, ContainerU>(std::move(xs));
}

template<typename T, template <typename, typename> class ContainerT>
struct container_monad final
{
	template<typename U = T, template <typename, typename> class ContainerU = ContainerT, class Function>
	auto bind(Function f) const -> container_monad<U, ContainerU>
	{
		ContainerU<U, std::allocator<U>> result;
		for (const auto& x : C)
		{
			const auto& fs = f(x);
			std::copy(std::begin(fs), std::end(fs), std::back_inserter(result));
		}
		return unit(std::move(result));
	}

	template<typename U, template <typename, typename> class ContainerU>
	friend auto unit(const ContainerU<U, std::allocator<U>>& xs) -> container_monad<U, ContainerU>;

	template<typename U, template <typename, typename> class ContainerU>
	friend auto unit(ContainerU<U, std::allocator<U>>&& xs) -> container_monad<U, ContainerU>;

	container_monad(const container_monad&) = default;
	~container_monad() = default;

private:
	container_monad& operator=(const container_monad&) = delete;
	container_monad() = default;
	container_monad(ContainerT<T, std::allocator<T>>&& xs)
		: C(std::move(xs)) {}

	ContainerT<T, std::allocator<T>> C;
};

#endif //__container_monad__