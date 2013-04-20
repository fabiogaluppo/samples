//Sample provided by Fabio Galuppo
#include <vector>

#include <iostream>
#include <assert.h>

#include "_generator.hpp"

#include <amp.h>
#include <amp_math.h>

using namespace concurrency;

using namespace concurrency::fast_math;
//using namespace concurrency::precise_math; //requires double precision

namespace BlackScholes
{
	const int DEFAULT_SIZE = 512 * 512;
	const float DEFAULT_RISK_FREE_RATE = -0.01f;
	const float DEFAULT_VOLATILITY = 0.99f;

	const int TILE_SIZE = 256;

	using std::vector;
	using std::cout;
	using std::wcout;

	vector<float> CallOptions, PutOptions;

	struct blackscholes
	{
		blackscholes(float _volatility = DEFAULT_VOLATILITY, float _riskfreerate = DEFAULT_RISK_FREE_RATE, const int _size = DEFAULT_SIZE)
			: data_size(_size), risk_free_rate(_riskfreerate), volatility(_volatility)
		{
			assert( 0 == (data_size % TILE_SIZE) ); //"Tile size must be multiple of data size"
			
			stock_price.resize(data_size);
			option_strike.resize(data_size);
			option_years.resize(data_size);
			PutOptions.resize(data_size); 
			CallOptions.resize(data_size);

			for (int i = 0; i < data_size; ++i)
			{
				stock_price[i] = 100.0f * generator.next();
				option_strike[i] = stock_price[i] * generator.next();
				option_years[i] = 20.0f * generator.next();
				PutOptions[i] = 0;
				CallOptions[i] = 0;
			}
		}

		void execute()
		{
			GpuBlackScholes(CallOptions, PutOptions);
		}
		
		void verify()
		{
			vector<float> call_result_cpu(data_size);
			vector<float> put_result_cpu(data_size);

			CpuBlackScholes(call_result_cpu, put_result_cpu);

			if (!sequence_equal(CallOptions, call_result_cpu) | !sequence_equal(PutOptions, put_result_cpu)) 
			{
				cout << "BlackScholes verification FAILED.\n";
			}
			else 
			{
				cout << "BlackScholes verification PASSED.\n";
			}
		}

	private:
		static void CalculateBlackScholes(float T_minus_t, float S, float K, float r, float lower_sigma, float& out_call_option, float& out_put_option)
		restrict (amp, cpu)
		{
			float sqrtT_minus_t = sqrtf(T_minus_t);
			float d1 = (logf(S / K) + (r + 0.5f * lower_sigma * lower_sigma) * T_minus_t) / (lower_sigma * sqrtT_minus_t);
			float d2 = d1 - lower_sigma * sqrtT_minus_t;

			float n1 = N(d1); //cumulative distribution
			float n2 = N(d2); //cumulative distribution
                
			float exp_RT = expf(- r * T_minus_t);
			out_call_option = S * n1 - K * exp_RT * n2;
			out_put_option = K * exp_RT * (1.0f - n2) - S * (1.0f - n1);
		}

		//standard normal cumulative distribution
		static float N(float x) restrict (amp, cpu)
		{
			const float a1 = 0.31938153f, a2 = -0.356563782f, a3 = 1.781477937f, a4 = -1.821255978f, a5 = 1.330274429f;
			float z = 1.0f / (1.0f + 0.2316419f * fabsf(x));
    		float serie = z * (a1 + z * (a2 + z * (a3 + z * (a4 + z * a5))));
			
			const float one_divBy_sqrt_2pi = 0.39894228040143267793994605993438f;
			float n = one_divBy_sqrt_2pi * expf(-0.5f * x * x) * serie;
			
			return (x > 0) ? 1.0f - n : n;
		}

		bool sequence_equal(vector<float>& ref_data, vector<float>& compute_data)
		{
			const float EPS = 1e-4f;
			for(unsigned i=0; i < ref_data.size(); i++) 
			{
				float a = ref_data[i], b = compute_data[i];

				// Is absolute or relative error less than EPS?
				float m = __max(1, __max(abs(a), abs(b)));
				if (abs(a - b) / m > EPS) 
				{
					cout << "FAIL: "<< a <<',' << b << "\n";
					return false;
				}
			}

			return true;
		}

		void GpuBlackScholes(vector<float>& out_call_result, vector<float>& out_put_result)
		{
			const array<float,1> amp_stock_price(data_size, stock_price.begin());
			const array<float,1> amp_option_strike(data_size, option_strike.begin());
			const array<float,1> amp_option_years(data_size, option_years.begin());
			array<float,1> amp_call_result(data_size);
			array<float,1> amp_put_result(data_size);

			float r = risk_free_rate; //risk free rate
			float lower_sigma = volatility; //volatility

			parallel_for_each(extent<1>(data_size).tile<TILE_SIZE>(),
			[=, &amp_stock_price, &amp_option_strike, &amp_option_years, &amp_call_result, &amp_put_result] 
			(tiled_index<TILE_SIZE> idx)
			restrict(amp)
			{
				auto x = idx.global[0];
				
				float S = amp_stock_price(x); //spot price
				float K = amp_option_strike(x); //strike price
				float T_minus_t = amp_option_years(x); //time to maturity
            
				CalculateBlackScholes(T_minus_t, S, K, r, lower_sigma, amp_call_result[x], amp_put_result[x]);
			});
    
			copy(amp_call_result, out_call_result.begin());
			copy(amp_put_result, out_put_result.begin());
		}

		void CpuBlackScholes(vector<float>& out_call_result, vector<float>& out_put_result)
		{
			for(unsigned i = 0; i < stock_price.size(); ++i) 
			{
				float r = risk_free_rate; //risk free rate
				float lower_sigma = volatility; //volatility
    
				float S = stock_price[i]; //spot price
				float K = option_strike[i]; //strike price
				float T_minus_t = option_years[i]; //time to maturity

				CalculateBlackScholes(T_minus_t, S, K, r, lower_sigma, out_call_result[i], out_put_result[i]);
			}
		}

		vector<float> stock_price, option_strike, option_years; 
		
		int data_size;
		float risk_free_rate;
		float volatility;

		RandGenerator generator;
	};
	
	void run()
	{
		accelerator default_device;
		std::wcout << L"Using device : " << default_device.get_description() << L"\n";
		
		blackscholes bs;
		bs.execute();
		bs.verify();
	}
}