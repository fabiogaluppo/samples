//Sample provided by Fabio Galuppo  
//March 2016 

#ifndef _group_sample_
#define _group_sample_

#define AdditiveGroup typename
//+ as associativy binary operation
//e as neutral element
//has inverse

//Abelian group (Group with commutative binary operation):
//Real numbers:
//(a + b) + c = a + (b + c)
//0.0 as neutral element
//-a as inverse

//Matrix 2x2 (for example):
//[a1 a2, a3 a4] == |a1 a2| 
//                  |a3 a4|
//([a1 a2, a3 a4] + [b1 b2, b3 b4]) + [c1 c2, c3 c4] = [a1 a2, a3 a4] + ([b1 b2, b3 b4] + [c1 c2, c3 c4])
//[0 0, 0 0] as neutral element
//[-a1 -a2, -a3 -a4] as inverse

#include <string>

template <AdditiveGroup T>
struct Matrix2x2 final
{
	//default constructor
	Matrix2x2() :
		Matrix2x2(T(0)) {}

	//constructor
	Matrix2x2(T a) : //implicit
		Matrix2x2(a, a, a, a) {}

	//constructor
	Matrix2x2(T a1, T a2, T a3, T a4) :
		a1(a1), a2(a2), a3(a3), a4(a4) {}

	//copy constructor
	Matrix2x2(const Matrix2x2<T>& that) :
		a1(that.a1), a2(that.a2), a3(that.a3), a4(that.a4) {}

	//copy assignment
	Matrix2x2<T>& operator=(const Matrix2x2<T>& that)
	{
		a1 = that.a1; a2 = that.a2;
		a3 = that.a3; a4 = that.a4;
		return this;
	}

	//destructor
	~Matrix2x2() = default;

	//equality
	bool operator==(const Matrix2x2<T>& that) const
	{
		if (this == &that)
			return true;

		return a1 == that.a1 && a2 == that.a2 &&
			a3 == that.a3 && a4 == that.a4;
	}

	friend Matrix2x2<T> operator+(const Matrix2x2<T>& lhs, const Matrix2x2<T>& rhs)
	{
		return Matrix2x2<T>(lhs.a1 + rhs.a1, lhs.a2 + rhs.a2,
			lhs.a3 + rhs.a3, lhs.a4 + rhs.a4);
	}

	friend Matrix2x2<T> operator-(const Matrix2x2<T>& that)
	{
		return Matrix2x2<T>(-that.a1, -that.a2, -that.a3, -that.a4);
	}

	std::string to_string() const
	{
		std::string temp = ("[" + std::to_string(a1) + " " + std::to_string(a2) + ", ");
		temp += (std::to_string(a3) + " " + std::to_string(a4) + "]");
		return temp;
	}

private:
	T a1, a2, a3, a4;
};

#include <iostream>
#include <iomanip>
using std::cout;
using std::boolalpha;

template <AdditiveGroup T>
static inline std::ostream& operator<< (std::ostream& os, Matrix2x2<T> val)
{
	os << val.to_string();
	return os;
}

template <AdditiveGroup T>
void test_requirements(const T& x)
{
	T neutral_element { 0 };
	T x_inverse = -x;
	cout << x << " + " << x_inverse << " == " << neutral_element << "?\n";
	cout << boolalpha << (x + x_inverse == neutral_element) << "\n";
}

template <AdditiveGroup T>
void test_associativity(const T& x, const T& y, const T& z)
{
	cout << "(" << x << " + " << y << ") + " << z << " == ";
	cout << x << " + (" << y << " + " << z << ")" << "?\n";
	cout << boolalpha << ((x + y) + z == x + (y + z)) << "\n";
}

#include <complex>

int group_sample_main()
{
	test_requirements(float(12.34)); //Additive Group of Reals
	test_requirements(std::complex<double>(3.0, 8.0) /* 3.0 + 8.0i */); //Additive Group of Complex
	test_requirements(Matrix2x2<short>(1, 2, 3, 4)); //Additive Group of Matrices

	test_associativity(float(12.3), float(45.6), float(78.9));
	test_associativity(std::complex<double>(1.0, 2.0), std::complex<double>(3.0, 4.0), std::complex<double>(5.0, 6.0));
	test_associativity(Matrix2x2<short>(10, 20, 30, 40), Matrix2x2<short>(11, 22, 33, 44), Matrix2x2<short>(6, 7, 8, 9));

	return 0;
}

#endif /* _group_sample_ */