//Sample provided by Fabio Galuppo 
//August 2014
//Part of Monadic types prototype for C++

#pragma once 

#ifndef __try_monad__
#define __try_monad__

#include <stdexcept>
#include <memory>
#include <functional>

//scala.util.Try
//http://www.scala-lang.org/api/current/#scala.util.Try
template<typename T>
struct try_monad;

template<typename T>
using try_monad_ptr = std::shared_ptr<try_monad<T>>;

//scala.util.Success
//http://www.scala-lang.org/api/current/index.html#scala.util.Success
template<typename T>
struct success_monad;

//scala.util.Failure
//http://www.scala-lang.org/api/current/index.html#scala.util.Failure
template<typename T>
struct failure_monad;

template<typename T>
try_monad_ptr<T> make_try(const T& value)
{
	std::unique_ptr<try_monad<T>> ptr(new success_monad<T>(value));
	return try_monad_ptr<T>(std::move(ptr));
}

template<typename T>
try_monad_ptr<T> make_try(const std::exception& exception)
{
	std::unique_ptr<try_monad<T>> ptr(new failure_monad<T>(exception));
	return try_monad_ptr<T>(std::move(ptr));
}

template<typename T, typename U>
U match(try_monad_ptr<T> ptr,
        std::function<U(const T&)> successCase,
        std::function<U(const std::exception&)> failureCase)
{
	return ptr->match(successCase, failureCase);
}

template<typename T>
struct try_monad
{
	virtual ~try_monad() {}
	
	//abstract  def  get : T
	//Returns the value from this Success or throws the exception if this is a Failure.
	virtual T get() const = 0;

	//abstract  def  isFailure : Boolean
	//Returns true if the Try is a Failure, false otherwise.
	virtual const bool is_failure() const = 0;

	//abstract  def  isSuccess : Boolean
	//Returns true if the Try is a Success, false otherwise.
	virtual const bool is_success() const = 0;

	template<typename U, class Function>
	try_monad_ptr<U> map(Function f) const
	{
		if (is_success())
			return make_try<U>(f(get_value()));
		return make_try<U>(get_exception());
	}

    template<typename U>
	U match(std::function<U(const T&)> successCase, std::function<U(const std::exception&)> failureCase)
	{
		if (is_success())
			return successCase(get_value());
		return failureCase(get_exception());
	}

	try_monad() = default;
	
private:
	try_monad(const try_monad&) = delete;
	try_monad& operator=(const try_monad&) = delete;

	const T& get_value() const { return dynamic_cast<const success_monad<T>*>(this)->get_state(); }
	const std::exception& get_exception() const { return dynamic_cast<const failure_monad<T>*>(this)->get_state(); }
};

template<typename T>
struct success_monad final : public try_monad<T>
{
	success_monad(const T& value)
		: Value_(value) {}

	T get() const override { return Value_; }

	const bool is_failure() const override { return false; }

	const bool is_success() const override { return true; }

	const T& get_state() const { return Value_;	}

private:
	T Value_;
};

template<typename T>
struct failure_monad final : public try_monad<T>
{
	failure_monad(const std::exception& exception)
		: Exception_(exception) {}

	T get() const override { throw Exception_; }

	const bool is_failure() const override { return true; }

	const bool is_success() const override { return false; }

	const std::exception& get_state() const { return Exception_; }

private:
	std::exception Exception_;
};

#endif //__try_monad__