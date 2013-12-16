//Sample provided by Fabio Galuppo
//Compositional TSP Solver version 0.1
//October 2013

#pragma once
#ifndef _tsp_monad_
#define _tsp_monad_

/*
class Monad m where
    (>>=)  :: m a -> (a -> m b) -> m b
    return :: a -> m a
*/

//The monad laws - the three fundamental laws: 
//(return x) >>= f == f x
//m >>= return == m
//(m >>= f) >>= g == m >>= (\x -> f x >>= g) 

//The first law requires that return is a left-identity with respect to >>= 
//The second law requires that return is a right-identity with respect to >>= 
//The third law is a kind of associativity law for >>=

#include "tsp.hpp"

#include <memory>
#include <utility>

struct base_args
{
	base_args(int id) : Id_(id)
	{
	}

	virtual ~base_args()
	{
	}

	int get_id() const
	{
		return Id_;
	}

private:
	int Id_;
};

typedef std::pair<std::shared_ptr<tsp_class>, base_args*> Maybe;

Maybe just(tsp_class t) { return std::make_pair(std::make_shared<tsp_class>(t), nullptr); }
Maybe nothing() { std::shared_ptr<tsp_class> t(nullptr); return std::make_pair(t, nullptr); }
bool has_value(Maybe m) { return m.first.get() != nullptr; }
tsp_class& ref(Maybe m) { return *m.first; }
const tsp_class& cref(Maybe m) { return *m.first; }

//t: shared_ptr<tsp_class> <=> Maybe
//ret : t -> TSP t
//bnd : TSP t -> (t -> TSP t) -> TSP t
struct TSP
{
	typedef Maybe T;
	typedef std::function<TSP(T)> transformer_type;
	
	explicit TSP(T t) : t(t)	
	{
	}

	//TODO: Semiregular

	//Regular:
	friend bool operator==(const TSP& lhs, const TSP& rhs) 
	{
		if (!has_value(lhs.t) && !has_value(rhs.t))
			return true;

		return *(lhs.t.first) == *(rhs.t.first);
	}
	
	friend bool operator!=(const TSP& lhs, const TSP& rhs)
	{
		 return !(lhs == rhs);
	}

	//map
	TSP map(transformer_type transformer) const
	{
		return bnd(*this, transformer);
	}

	//map
	template <typename U>
	U map(std::function<U(T)> transformer) const
	{
		return bnd(*this, transformer);
	}

	//return
	static friend TSP ret(T a) 
	{ 
		return TSP(just(*a.first));
	}

	//bind
	static friend TSP bnd(TSP a, transformer_type transformer)
	{
		auto aa = a.t;
		if (has_value(aa))
			return transformer(aa);
		return TSP(nothing());
	}

	//bind
	template <typename U>
	static friend U bnd(TSP a, std::function<U(T)> transformer)
	{
		if (has(a))
			return transformer(a.t);
		return U();

	}

	static friend tsp_class& ref(TSP a)
	{
		return ::ref(a.t);
	}

	static friend bool has(TSP a)
	{
		return has_value(a.t);
	}

private:
	T t;
};

Maybe just(TSP t) { return just(ref(t)); }

#endif /* _tsp_monad_ */