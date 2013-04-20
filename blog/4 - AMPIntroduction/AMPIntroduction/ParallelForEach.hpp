//Sample provided by Fabio Galuppo
#include <vector>
#include <algorithm>

#include <iostream>

#include "_generator.hpp"
#include "_util.hpp"

#include <amp.h>
using namespace concurrency;

namespace ParallelForEach
{
	using std::vector;
	using std::generate;
	using std::for_each;
	
	//const int M = 1024, N = 1024, K = 10;
	const int M = 5, N = 5, K = 5;
	
	void do_vec_mul(const vector<float>& a, const vector<float>& b, vector<float>& c)
	{
		array_view<const float, 2> a_view(M, K, a), b_view(K, N, b);
		array_view<float, 2> c_view(M, N, c);
		c_view.discard_data();
		
		parallel_for_each( c_view.extent, [=](index<2> idx) restrict(amp)
		{
			auto k = a_view.extent[1];
			float sum = 0;
			for(int i = 0; i < k; ++i)
				sum += a_view(idx[0], i) * b_view(i, idx[1]);
			c_view[idx] = sum;
		});
		c_view.synchronize();
	}

	void run()
	{
		using std::cout;

		vector<float> A(M * K), B(K * N), C(M * N);
		
		PositiveIntegerGenerator gen;
		
		generate(A.begin(), A.end(), [&gen](){ return gen.next(); });
		generate(B.begin(), B.end(), [&gen](){ return gen.next(); });

		do_vec_mul(A, B, C);

		cout << "A:\n"; print_matrix(A, M, K, 2, 8); cout << "\n";
		cout << "B:\n"; print_matrix(B, K, N, 2, 8); cout << "\n";
		cout << "C:\n"; print_matrix(C, M, N, 2, 8);
	}
}