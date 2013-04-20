//Sample provided by Fabio Galuppo
#include <vector>
#include <algorithm>
#include "_generator.hpp"

namespace CPPRefresh
{
	using std::vector;
	using std::generate;
	using std::for_each;
	
	//const int M = 1024, N = 1024;
	const int M = 5, N = 5;

	void do_vec_sum(const vector<float>& a, const vector<float>& b, vector<float>& c)
	{
		for(int i = 0; i < M; ++i)
			for(int j = 0; j < N; ++j)
				c[i * N + j] = a[i * N + j] + b[i * N + j];
	}
	
	void do_vec_sum_2(const vector<float>& a, const vector<float>& b, vector<float>& c)
	{
		int n = 0;
		for_each(c.begin(), c.end(), [&a, &b, &n](float& x){ x = a[n] + b[n]; ++n; });
	}

	void run()
	{
		vector<float> A(M * N), B(M * N), C(M * N);
		
		NormalGenerator gen;
		
		generate(A.begin(), A.end(), [&gen](){ return gen.next(); });
		generate(B.begin(), B.end(), [&gen](){ return gen.next(); });

		do_vec_sum(A, B, C);
		do_vec_sum_2(A, B, C);
	}
}