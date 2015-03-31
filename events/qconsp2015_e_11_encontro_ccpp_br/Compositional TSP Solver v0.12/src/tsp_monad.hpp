//Sample provided by Fabio Galuppo
//Compositional TSP Solver version 0.12
//March 2015

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

//unit: value -> tsp_monad value 
//      [or tsp -> TSP tsp]
//bnd:  tsp_monad value -> (value -> tsp_monad value) -> tsp_monad value 
//      [or TSP tsp -> (t -> TSP tsp) -> TSP tsp]
struct tsp_monad final
{
	typedef Maybe value_type;
	typedef std::function<tsp_monad(value_type)> function_type;

	template <typename U>
	auto map(std::function<U(value_type)> f) const -> U
	{
		return bnd(*this, f);
	}

	auto map(function_type f) const -> tsp_monad
	{
		return std::move(bnd<tsp_monad>(*this, f));
	}

	static auto unit(const value_type& value) -> tsp_monad
	{
		tsp_monad result;
		result.value = value;
		return std::move(result);
	}

	static auto bnd(const tsp_monad& t, function_type f) -> tsp_monad
	{
		auto value = t.value;
		if (has_value(value))
			return std::move(f(value));
		return std::move(unit(nothing()));
	}
	
	template <typename U>
	static auto bnd(const tsp_monad& t, std::function<U(value_type)> f) -> U
	{
		if (has(t))
			return f(t.value);
		return U();
	}
	
	friend auto ref(const tsp_monad& x) -> tsp_class& //keep this member?
	{
		return ::ref(x.value);
	}
	
	friend auto has(const tsp_monad& x) -> bool
	{
		return has_value(x.value);
	}
	
	friend auto operator==(const tsp_monad& lhs, const tsp_monad& rhs) -> bool
	{
		if (!has_value(lhs.value) && !has_value(rhs.value))
			return true;

		return *(lhs.value.first) == *(rhs.value.first);
	}

	friend auto operator!=(const tsp_monad& lhs, const tsp_monad& rhs) -> bool
	{
		return !(lhs == rhs);
	}

	tsp_monad(const tsp_monad&) = default;
	tsp_monad& operator=(const tsp_monad&) = default;

private:
	tsp_monad() = default;
	
	tsp_monad(value_type&& x)
		: value(std::move(x)) {}

	value_type value;
};

static inline auto unit(const tsp_monad::value_type& value) -> tsp_monad
{
	return tsp_monad::unit(value);
}

static inline auto bnd(const tsp_monad& t, tsp_monad::function_type f) -> tsp_monad
{
	return tsp_monad::bnd(t, f);
}

template <typename U>
static inline auto bnd(const tsp_monad& t, std::function<U(typename tsp_monad::value_type)> f) -> U
{
	return tsp_monad::bnd(t, f);
}

#endif /* _tsp_monad_ */