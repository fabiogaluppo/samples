//Sample provided by Fabio Galuppo
#include <vector>
#include <algorithm>
#include "_generator.hpp"

#include <amp.h>
using namespace concurrency;

namespace ArrayView
{
	using std::vector;
	using std::generate;
	using std::for_each;
	
	//const int M = 1024, N = 1024;
	const int M = 5, N = 5;

	void do_vec_sum(const vector<float>& a, const vector<float>& b, vector<float>& c)
	{
		array_view<const float, 1> a_view(M * N, a), b_view(M * N, b);
		array_view<float, 1> c_view(M * N, c);
		
		for(int i = 0; i < M; ++i)
			for(int j = 0; j < N; ++j)
				c_view(i * N + j) = a_view(i * N + j) + b_view(i * N + j);
	}

	void do_vec_sum_2(const vector<float>& a, const vector<float>& b, vector<float>& c)
	{
		extent<1> e(M * N);
		array_view<const float, 1> a_view(e, a), b_view(e, b);
		array_view<float, 1> c_view(e, c);
		
		for(int i = 0; i < M; ++i)
			for(int j = 0; j < N; ++j)
				c_view(i * N + j) = a_view(i * N + j) + b_view(i * N + j);
	}

	void do_vec_sum_3(const vector<float>& a, const vector<float>& b, vector<float>& c)
	{
		extent<2> e(M, N);
		array_view<const float, 2> a_view(e, a), b_view(e, b);
		array_view<float, 2> c_view(e, c);
		
		for(int i = 0; i < M; ++i)
			for(int j = 0; j < N; ++j)
				c_view(i, j) = a_view(i, j) + b_view(i, j);
	}

	void do_vec_sum_4(const vector<float>& a, const vector<float>& b, vector<float>& c)
	{
		extent<2> e(M, N);
		array_view<const float, 2> a_view(e, a), b_view(e, b);
		array_view<float, 2> c_view(e, c);

		index<2> idx(0, 0);
		for(idx[0]; idx[0] < e[0]; ++idx[0]) //y
			for(idx[1]; idx[1] < e[1]; ++idx[1]) //x
				c_view[idx] = a_view[idx] + b_view[idx];
	}
	
	void run()
	{
		vector<float> A(M * N), B(M * N), C(M * N);
		
		PositiveIntegerGenerator gen;
		
		generate(A.begin(), A.end(), [&gen](){ return gen.next(); });
		generate(B.begin(), B.end(), [&gen](){ return gen.next(); });

		do_vec_sum(A, B, C);
		do_vec_sum_2(A, B, C);
		do_vec_sum_3(A, B, C);
		do_vec_sum_4(A, B, C);
	}
}