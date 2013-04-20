//Sample provided by Fabio Galuppo
#include "_generator.hpp"

#include <amp.h>
using namespace concurrency;

namespace Tiling
{
	using std::vector;
	using std::generate;

	//const int M = 512, N = 512, K = 512;
	//const int M = 16, N = 16, K = 16;
	//const int M = 5, N = 5, K = 5;

	const int M = 2, N = 6, K = 4;

	void do_vec_mul(const vector<float>& a, const vector<float>& b, vector<float>& c)
	{
		array_view<const float, 2> a_view(M, K, a), b_view(K, N, b);
		array_view<float, 2> c_view(M, N, c);
		c_view.discard_data();
		
		parallel_for_each( c_view.extent.tile<16, 16>(), [=](tiled_index<16, 16> idx) restrict(amp)
		{
			auto k = a_view.extent[1]; //a.cols
			int row = idx.global[0];   //y
			int col = idx.global[1];   //x
			float sum = 0;
			for(int i = 0; i < k; ++i)
				sum += a_view(row, i) * b_view(i, col);
			c_view[idx.global] = sum;
		});
		c_view.synchronize();
	}

	const int TILE_SIZE = 2;

	void do_vec_mul_2(const vector<float>& a, const vector<float>& b, vector<float>& c)
	{
		array_view<const float, 2> a_view(M, K, a), b_view(K, N, b);
		array_view<float, 2> c_view(M, N, c);
		c_view.discard_data();
		
		parallel_for_each( c_view.extent.tile<TILE_SIZE, TILE_SIZE>(), [=](tiled_index<TILE_SIZE, TILE_SIZE> idx) restrict(amp)
		{
			auto k = a_view.extent[1]; //a.cols
			int g_row = idx.global[0]; //y.global
			int g_col = idx.global[1]; //x.global
			int l_row = idx.local[0];  //y.local
			int l_col = idx.local[1];  //x.local

			tile_static float a_shared[TILE_SIZE][TILE_SIZE], b_shared[TILE_SIZE][TILE_SIZE];

			float sum = 0;
			for(int j = 0; j < k; j += TILE_SIZE)
			{
				a_shared[l_row][l_col] = a_view(g_row, l_col + j);
				b_shared[l_row][l_col] = b_view(l_row + j, g_col);
				
				idx.barrier.wait();

				for(int i = 0; i < TILE_SIZE; ++i)
					sum += a_shared[l_row][i] * b_shared[i][l_col];
				
				idx.barrier.wait();
			}
			c_view[idx.global] = sum;
		});
		c_view.synchronize();
	}

	void run()
	{
		using std::cout;
		
		vector<float> A(M * K), B(K * N), C(M * N);
		
		/*
		PositiveIntegerGenerator gen;
		
		generate(A.begin(), A.end(), [&gen](){ return gen.next(); });
		generate(B.begin(), B.end(), [&gen](){ return gen.next(); });

		do_vec_mul(A, B, C);
		*/

		PositiveIntegerGenerator gen1, gen2;
		generate(A.begin(), A.end(), [&gen1](){ return gen1.next(); });
		generate(B.begin(), B.end(), [&gen2](){ return gen2.next(); });

		for(auto iter = C.begin(); iter != C.end(); ++iter) 
			*iter = 0;

		do_vec_mul_2(A, B, C);

		cout << "A:\n"; print_matrix(A, M, K, 3, 8); cout << "\n";
		cout << "B:\n"; print_matrix(B, K, N, 3, 8); cout << "\n";
		cout << "C:\n"; print_matrix(C, M, N, 3, 8);
	}
}