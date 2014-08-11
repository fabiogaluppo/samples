//Sample provided by Fabio Galuppo 
//August 2014
//Part of Monadic types prototype for C++

#pragma once 

#ifndef __enumerable_monad__
#define __enumerable_monad__

#include <memory>
#include <algorithm>
#include <iterator>
#include <utility>
#include <functional>
#include <numeric>

//http://referencesource.microsoft.com/#System.Core/System/Linq/Enumerable.cs

template<typename T, template <typename, typename> class ContainerT>
struct enumerable_monad;

template<typename U, template <typename, typename> class ContainerU>
auto make_enumerable(ContainerU<U, std::allocator<U>>&& xs) -> enumerable_monad<U, ContainerU>;

namespace internal
{
	template<typename T, template <typename, typename> class ContainerT>
	struct state final
	{
		ContainerT<T, ::std::allocator<T>> C;
		::std::function<enumerable_monad<T, ContainerT>()> F;
	};

	template<typename T, template <typename, typename> class ContainerT>
	using state_ptr = ::std::shared_ptr<state<T, ContainerT>>;

	template<typename T, template <typename, typename> class ContainerT>
	struct base_state
	{
	protected:
		bool is_lazy() const {
			if (StatePtr_->F)
				return true;
			return false;
		}

		state_ptr<T, ContainerT> StatePtr_;
	};

	template<typename U, template <typename, typename> class ContainerU, class UnaryPredicate>
	struct where_functor final : private base_state<U, ContainerU>
	{
		where_functor(state_ptr<U, ContainerU> statePtr, UnaryPredicate pred)
			: Predicate_(pred)
		{
			this->StatePtr_ = statePtr;
		}

		auto operator()() const -> enumerable_monad<U, ContainerU>
		{
			ContainerU<U, ::std::allocator<U>> result;
			if (this->is_lazy())
			{
				auto m = this->StatePtr_->F();
				::std::copy_if(::std::begin(m.StatePtr_->C), ::std::end(m.StatePtr_->C), ::std::back_inserter(result), Predicate_);
			}
			else
			{
				::std::copy_if(::std::begin(this->StatePtr_->C), ::std::end(this->StatePtr_->C), ::std::back_inserter(result), Predicate_);
			}
			return make_enumerable(::std::move(result));
		}

	private:
		UnaryPredicate Predicate_;
	};
}

template<typename U, template <typename, typename> class ContainerU>
auto make_enumerable(const ContainerU<U, std::allocator<U>>& xs) -> enumerable_monad<U, ContainerU>
{
	auto ptr = std::unique_ptr<internal::state<U, ContainerU>>(new internal::state<U, ContainerU>);
	std::copy(std::begin(xs), std::end(xs), std::back_inserter(ptr->C));
	return enumerable_monad<U, ContainerU>(internal::state_ptr<U, ContainerU>(std::move(ptr)));
}

template<typename U, template <typename, typename> class ContainerU>
auto make_enumerable(ContainerU<U, std::allocator<U>>&& xs) -> enumerable_monad<U, ContainerU>
{
	auto ptr = std::unique_ptr<internal::state<U, ContainerU>>(new internal::state<U, ContainerU>);
	ptr->C = std::move(xs);
	return enumerable_monad<U, ContainerU>(internal::state_ptr<U, ContainerU>(std::move(ptr)));
}

template<typename U, template <typename, typename> class ContainerU>
auto make_enumerable(const std::function<enumerable_monad<U, ContainerU>()>& f) -> enumerable_monad<U, ContainerU>
{
	auto ptr = std::unique_ptr<internal::state<U, ContainerU>>(new internal::state<U, ContainerU>);
	ptr->F = f;
	return enumerable_monad<U, ContainerU>(internal::state_ptr<U, ContainerU>(std::move(ptr)));
}

template<typename T, template <typename, typename> class ContainerT>
struct enumerable_monad final : private internal::base_state<T, ContainerT>
{
	template<typename U = T, template <typename, typename> class ContainerU = ContainerT, class Function>
	auto select_many(Function f) const -> enumerable_monad<U, ContainerU> //intermediate
	{
		//TODO: lazy version

		ContainerU<U, std::allocator<U>> result;		
		if (this->is_lazy())
		{
			auto m = this->StatePtr_->F();
			for (const auto& x : m.StatePtr_->C)
			{
				const auto& fs = f(x);
				std::copy(std::begin(fs), std::end(fs), std::back_inserter(result));
			}
		}
		else
		{
			for (const auto& x : this->StatePtr_->C)
			{
				const auto& fs = f(x);
				std::copy(std::begin(fs), std::end(fs), std::back_inserter(result));
			}			
		}
		return make_enumerable(std::move(result));
	}

	template<class UnaryPredicate>
	auto where(UnaryPredicate pred) const -> enumerable_monad<T, ContainerT> //intermediate
	{
		//lazy version

		internal::where_functor<T, ContainerT, UnaryPredicate> where_fun(this->StatePtr_, pred);
		std::function<enumerable_monad<T, ContainerT>()> f = where_fun;
		return make_enumerable(f);
	}

	template<typename U = T, class BinaryOperation>
	auto aggregate(U seed, BinaryOperation func) const -> U //terminal
	{
		if (this->is_lazy())
		{
			auto m = this->StatePtr_->F();
			return std::accumulate<decltype(std::begin(m.StatePtr_->C)), U>(std::begin(m.StatePtr_->C), std::end(m.StatePtr_->C), seed, func);
		}
		return std::accumulate<decltype(std::begin(this->StatePtr_->C)), U>(std::begin(this->StatePtr_->C), std::end(this->StatePtr_->C), seed, func);
	}

	template<typename U = T>
	auto sum() const -> T //terminal
	{
		return aggregate(U(0), [](U acc, U x){ return acc + x; });
	}

	template<class Function>
	auto for_each(Function f) const -> void //terminal
	{
		if (this->is_lazy())
		{
			auto m = this->StatePtr_->F();
			for (const auto& x : m.StatePtr_->C) f(x);
		}
		else
		{
			for (const auto& x : this->StatePtr_->C) f(x);
		}
	}

	template<template <typename, typename> class ContainerU = ContainerT>
	auto to_container() const -> ContainerU<T, std::allocator<T>> //terminal
	{
		ContainerU<T, std::allocator<T>> result;
		if (this->is_lazy())
		{
			auto m = this->StatePtr_->F();
			std::copy(std::begin(m.StatePtr_->C), std::end(m.StatePtr_->C), std::back_inserter(result));
		}
		else
		{
			std::copy(std::begin(this->StatePtr_->C), std::end(this->StatePtr_->C), std::back_inserter(result));
		}
		return std::move(result);
	}

	template<typename U, template <typename, typename> class ContainerU, class UnaryPredicate>
	friend struct internal::where_functor;

	template<typename U, template <typename, typename> class ContainerU>
	friend auto make_enumerable(const ContainerU<U, std::allocator<U>>& xs) -> enumerable_monad<U, ContainerU>;

	template<typename U, template <typename, typename> class ContainerU>
	friend auto make_enumerable(ContainerU<U, std::allocator<U>>&& xs) -> enumerable_monad<U, ContainerU>;

	template<typename U, template <typename, typename> class ContainerU>
	friend auto make_enumerable(const std::function<enumerable_monad<U, ContainerU>()>& f) -> enumerable_monad<U, ContainerU>;

	enumerable_monad(const enumerable_monad&) = default;
	~enumerable_monad() = default;

private:
	enumerable_monad& operator=(const enumerable_monad&) = delete;
	enumerable_monad() = default;

	enumerable_monad(internal::state_ptr<T, ContainerT> statePtr)
	{
		this->StatePtr_ = statePtr;
	}
};

#endif //__enumerable_monad__