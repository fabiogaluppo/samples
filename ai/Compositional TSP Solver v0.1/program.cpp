//Sample provided by Fabio Galuppo
//Compositional TSP Solver version 0.1
//November 2013

#include "src\_tsp.hpp"

tsp_class read_att48_tsp()
{
	int coords[] = {
		6734, 1453,
		2233, 10,
		5530, 1424,
		401, 841,
		3082, 1644,
		7608, 4458,
		7573, 3716,
		7265, 1268,
		6898, 1885,
		1112, 2049,
		5468, 2606,
		5989, 2873,
		4706, 2674,
		4612, 2035,
		6347, 2683,
		6107, 669,
		7611, 5184,
		7462, 3590,
		7732, 4723,
		5900, 3561,
		4483, 3369,
		6101, 1110,
		5199, 2182,
		1633, 2809,
		4307, 2322,
		675, 1006,
		7555, 4819,
		7541, 3981,
		3177, 756,
		7352, 4506,
		7545, 2801,
		3245, 3305,
		6426, 3173,
		4608, 1198,
		23, 2216,
		7248, 3779,
		7762, 4595,
		7392, 2244,
		3484, 2829,
		6271, 2135,
		4985, 140,
		1916, 1569,
		7280, 4899,
		7509, 3239,
		10, 2676,
		6807, 2993,
		5185, 3258,
		3023, 1942
	};
	
	tsp_class tsp_instance;
	int N = sizeof(coords) / sizeof(int);
	int city_number = 0;
	for(int i = 0; i < N; i+=2)
	{
		int x = coords[i], y = coords[i+1];
		tsp_class::city_info info = { ++city_number, std::make_pair(x, y) };
		tsp_instance.cities.push_back(info);
	}

	return tsp_instance;
}

/*
 getopt
 declaration: http://svnweb.freebsd.org/base/stable/9/include/unistd.h?view=markup
 definition: http://svnweb.freebsd.org/base/stable/9/lib/libc/stdlib/getopt.c?view=markup
*/
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

int opterr = 1, optind = 1, optopt, optreset;
char *optarg;

#define BADCH   (int)'?'
#define BADARG  (int)':'
#define EMSG    ""

/*
 * getopt --
 *      Parse argc/argv argument vector.
 */
int getopt(int nargc, char * const nargv[], const char *ostr)
{
	static char *place = EMSG;              /* option letter processing */
    char *oli;                              /* option letter list index */
    
    if (optreset || *place == 0) {          /* update scanning pointer */
        optreset = 0;
        place = nargv[optind];
        if (optind >= nargc || *place++ != '-') {
            /* Argument is absent or is not an option */
            place = EMSG;
            return (-1);
        }
        optopt = *place++;
        if (optopt == '-' && *place == 0) {
            /* "--" => end of options */
            ++optind;
            place = EMSG;
            return (-1);
        }
        if (optopt == 0) {
            /* Solitary '-', treat as a '-' option
             if the program (eg su) is looking for it. */
            place = EMSG;
            if (strchr(ostr, '-') == NULL)
                return (-1);
            optopt = '-';
        }
    } else
        optopt = *place++;
    
	/* See if option letter is one the caller wanted... */
    //if (optopt == ':' || (oli = strchr(ostr, optopt)) == NULL) {
    if (optopt == ':' || (oli = const_cast<char*>(strchr(ostr, optopt))) == NULL) {
        if (*place == 0)
            ++optind;
        if (opterr && *ostr != ':')
            (void)fprintf(stderr,
                          //"%s: illegal option -- %c\n", _getprogname(),
                          "%s: illegal option -- %c\n", "getopt",
                          optopt);
        return (BADCH);
    }
	
    /* Does this option need an argument? */
    if (oli[1] != ':') {
        /* don't need argument */
        optarg = NULL;
        if (*place == 0)
            ++optind;
    } else {
        /* Option-argument is either the rest of this argument or the
         entire next argument. */
        if (*place)
            optarg = place;
        else if (nargc > ++optind)
            optarg = nargv[optind];
        else {
            /* option-argument absent */
            place = EMSG;
            if (*ostr == ':')
                return (BADARG);
            if (opterr)
                (void)fprintf(stderr,
                              "%s: option requires an argument -- %c\n",
                              //_getprogname(), optopt);
                              "getopt", optopt);
            return (BADCH);
        }
        place = EMSG;
        ++optind;
    }
    return (optopt);                        /* return option letter */
}

#include <iostream>

void usage(void)
{
    std::cerr << "usage: tspsolution -p:N -g:N -n:N" << std::endl;
	std::cerr << "where" << std::endl;
	std::cerr << "  p: pipeline number" << std::endl;
	std::cerr << "  g: number of generations" << std::endl;
	std::cerr << "  n: number of tasks" << std::endl;
	std::cerr << "  N: an integer greater than 0" << std::endl;
	
    exit(1);
}

void get_args(int& argc, char* argv[], unsigned int& p, unsigned int& g, unsigned int& n)
{
	int ch;
	setlocale(LC_CTYPE, "");
	while ((ch = getopt(argc, argv, "p:g:n:")) != -1)
	{
		switch((char)ch) 
		{
			case 'p': //pipeline #
				++optarg;
				p = static_cast<unsigned int>(atol(optarg));
				break;
			case 'g': //generations #
				++optarg;
				g = static_cast<unsigned int>(atol(optarg));
				break;
			case 'n': //tasks #
				++optarg;
				n = static_cast<unsigned int>(atol(optarg));
				break;
			case '?':
			default:
				usage();
		}
	}

	if (0 == p || 0 == g || 0 == n)
		usage();

	argv += optind;
    argc -= optind;	
}

#include <utility>
#include <functional>
#include <vector>

typedef std::function<void(tsp_class&, unsigned int, unsigned int)> pipeline_type;

TSP make_TSP(const tsp_class& tsp_instance) { return TSP(just(tsp_instance)); }

void MonadicLawsProofing()
{
	//Scala notation: unit(x) flatMap f == f(x)
	{
		//Left Identity (1st Law)
		auto x = just(read_att48_tsp());
		
		auto f = [](TSP::T t){ return ref(t).do_cycle_length(); };

		double fun_result = bnd<double>(ret(x), f); //functional composition
		double oo_result = ret(x).map<double>(f); //object-oriented

		bool fun_holds = fun_result == f(x);
		bool oo_holds = oo_result == f(x);

		std::cout << "1st Law (Left Identity) " << (fun_holds ? "holds" : "doesn't hold") << std::endl;
		std::cout << "1st Law (Left Identity) " << (oo_holds ? "holds" : "doesn't hold") << std::endl;
	}

	//Scala notation: m flatMap unit == m
	{
		//Right Identity (2nd Law)
		auto m = make_TSP(read_att48_tsp());
		
		auto fun_result = bnd(m, ret);
		auto oo_result = m.map(ret);

		bool fun_holds = fun_result == m;
		bool oo_holds = oo_result == m;

		std::cout << "2nd Law (Right Identity) " << (fun_holds ? "holds" : "doesn't hold") << std::endl;
		std::cout << "2nd Law (Right Identity) " << (oo_holds ? "holds" : "doesn't hold") << std::endl;
	}

	//Scala notation: m flatMap f flatMap g == m flatMap (x => f(x) flatMap g)
	{
		//Associativity (3rd Law)
		auto m = make_TSP(read_att48_tsp());

		auto f = [](TSP::T t) -> TSP
		{ 
			auto u = ref(t);
			std::swap(u.cities[0], u.cities[1]);
			return make_TSP(u); 
		};
		
		auto g = [](TSP::T t) -> TSP
		{ 
			auto u = ref(t);
			std::swap(u.cities[2], u.cities[3]);
			return make_TSP(u); 
		};

		auto fun_result_1 = bnd(bnd(m, f), g);
		auto fun_result_2 = bnd(m, [&](TSP::T t) {
			return bnd(f(t), g);
		});

		auto oo_result_1 = m.map(f).map(g);
		auto oo_result_2 = m.map([&](TSP::T t) {
			return f(t).map(g);
		}); 
		
		bool fun_holds = fun_result_1 == fun_result_2;
		bool oo_holds = oo_result_1 == oo_result_2;

		std::cout << "3rd Law (Associativity) " << (fun_holds ? "holds" : "doesn't hold") << std::endl;
		std::cout << "3rd Law (Associativity) " << (oo_holds ? "holds" : "doesn't hold") << std::endl;
	}
}

//TSP -> NN -> Generations( g, ForkJoin ( n, SA -> 2-OPT ) ) -> TSP'
void Pipeline1(tsp_class& tsp_instance, unsigned int number_of_tasks, unsigned int number_of_generations)
{
	#pragma region "PipelineConfiguration"
	auto a = Args<General_args_type>(make_General_args(number_of_generations, number_of_tasks));
	auto sa = Args<SA_args_type>(make_SA_args(1000.0, 0.00001, 0.999, 400));
	auto aco = Args<ACO_args_type>();
	auto ga = Args<GA_args_type>();

	const char* pipeline_description = "TSP -> NN -> Generations( g, ForkJoin ( n, SA -> 2-OPT ) ) -> TSP'";
	display_args(pipeline_description, a, sa, aco, ga);
	
	auto g = a[0].number_of_iterations_or_generations;
	auto n = a[0].number_of_tasks_in_parallel;
	auto _TSP = TSP(just(tsp_instance));
	auto _DisplayInput = Display("TSP INPUT", DisplayFlags::All);
	auto _NN = Measure(NN(), Display("NEAREST NEIGHBOUR", DisplayFlags::EmitMathematicaGraphPlot));	
	auto _SA_2OPT = Chain(SA(sa[0].initial_temperature, sa[0].stopping_criteria_temperature, 
							 sa[0].decreasing_factor, sa[0].monte_carlo_steps), _2OPT());
	auto _ForkJoin = [](unsigned int n, TSP::transformer_type map_fun){ return Measure(ForkJoin(n, map_fun)); };
	auto _DisplayOutput = Display("TSP OUTPUT", DisplayFlags::EmitMathematicaGraphPlot); 
	#pragma endregion

	//TSP -> NN -> Generations( g, ForkJoin ( n, SA -> 2-OPT ) ) -> TSP'
	auto result = _TSP
					.map(_DisplayInput)
					.map(_NN)
					.map(Generations(g, _ForkJoin(n, _SA_2OPT)))
					.map(_DisplayOutput);
}

//TSP -> NN -> Generations( g, ForkJoin ( n, GA -> 2-OPT ) ) -> TSP'
void Pipeline2(tsp_class& tsp_instance, unsigned int number_of_tasks, 
			                            unsigned int number_of_generations)
{
	#pragma region "PipelineConfiguration"
	auto a = Args<General_args_type>(make_General_args(number_of_generations, number_of_tasks));
	auto sa = Args<SA_args_type>();
	auto aco = Args<ACO_args_type>();
	auto ga = Args<GA_args_type>(make_GA_args(1000, 10, 5, 50000, 10, 0.9));

	const char* pipeline_description = "TSP -> NN -> Generations( g, ForkJoin ( n, GA -> 2-OPT ) ) -> TSP'";
	display_args(pipeline_description, a, sa, aco, ga);
	
	auto g = a[0].number_of_iterations_or_generations;
	auto n = a[0].number_of_tasks_in_parallel;
	auto _TSP = TSP(just(tsp_instance));
	auto _DisplayInput = Display("TSP INPUT", DisplayFlags::All);
	auto _NN = Measure(NN(), Display("NEAREST NEIGHBOUR", DisplayFlags::EmitMathematicaGraphPlot));	
	auto _GA_2OPT = Chain(GA(ga[0].population_size, ga[0].mutation_percentage, ga[0].group_size, 
							 ga[0].number_of_generations, ga[0].nearby_cities, ga[0].nearby_cities_percentage), _2OPT());
	auto _ForkJoin = [](unsigned int n, TSP::transformer_type map_fun){ return Measure(ForkJoin(n, map_fun)); };
	auto _DisplayOutput = Display("TSP OUTPUT", DisplayFlags::EmitMathematicaGraphPlot); 
	#pragma endregion

	//TSP -> NN -> Generations( g, ForkJoin ( n, GA -> 2-OPT ) ) -> TSP'
	auto result = _TSP
					.map(_DisplayInput)
					.map(_NN)
					.map(Generations(g, _ForkJoin(n, _GA_2OPT)))
					.map(_DisplayOutput);
}

//TSP -> NN -> Generations( g, ForkJoin ( n, ACO -> 2-OPT ) ) -> TSP'
void Pipeline3(tsp_class& tsp_instance, unsigned int number_of_tasks, 
			                            unsigned int number_of_generations)
{
	#pragma region "PipelineConfiguration"
	auto a = Args<General_args_type>(make_General_args(number_of_generations, number_of_tasks));
	auto sa = Args<SA_args_type>();
	auto ga = Args<GA_args_type>();

	const int aco_iterations = static_cast<int>(tsp_instance.cities.size() * 100);
	const ants_type::size_type number_of_ants = tsp_instance.cities.size();
	const double BASE_PHEROMONE = 1.0f / static_cast<double>(tsp_instance.cities.size());
	const double ALPHA = 1.0;
	const double BETA  = 1.0;
	const double RHO   = 0.9;
	const double QVAL  = 70;
	auto aco = Args<ACO_args_type>(make_ACO_args(aco_iterations, number_of_ants, 
								   BASE_PHEROMONE, ALPHA, BETA, RHO, QVAL));	

	const char* pipeline_description = "TSP -> NN -> Generations( g, ForkJoin ( n, ACO -> 2-OPT ) ) -> TSP'";
	display_args(pipeline_description, a, sa, aco, ga);
	
	auto g = a[0].number_of_iterations_or_generations;
	auto n = a[0].number_of_tasks_in_parallel;
	auto _TSP = TSP(just(tsp_instance));
	auto _DisplayInput = Display("TSP INPUT", DisplayFlags::All);
	auto _NN = Measure(NN(), Display("NEAREST NEIGHBOUR", DisplayFlags::EmitMathematicaGraphPlot));	
	auto _ACO_2OPT = Chain(ACO(aco[0].aco_iterations, aco[0].number_of_ants, aco[0].base_pheromone, 
								aco[0].favor_pheromone_level_over_distance, 
								aco[0].favor_distance_over_pheromone_level, 
								aco[0].value_for_intensification_and_evaporation, 
								aco[0].pheronome_distribution), _2OPT());
	auto _ForkJoin = [](unsigned int n, TSP::transformer_type map_fun){ return Measure(ForkJoin(n, map_fun)); };
	auto _DisplayOutput = Display("TSP OUTPUT", DisplayFlags::EmitMathematicaGraphPlot); 
	#pragma endregion

	//TSP -> NN -> Generations( g, ForkJoin ( n, ACO -> 2-OPT ) ) -> TSP'
	auto result = _TSP
					.map(_DisplayInput)
					.map(_NN)
					.map(Generations(g, _ForkJoin(n, _ACO_2OPT)))
					.map(_DisplayOutput);
}

//tsplibreader ..\_tsplib\att48.tsp | tspsolution -p:1 -g:2 -n:4  > tsp_result.txt
int main(int argc, char* argv[])
{
	//MonadicLawsProofing();

	unsigned int p = 0, g = 0, n = 0;
	get_args(argc, argv, p, g, n);

	std::vector<pipeline_type> pipelines;
	pipelines.push_back(Pipeline1);  //TSP -> NN -> Generations( g, ForkJoin ( n, SA -> 2-OPT ) ) -> TSP'
	pipelines.push_back(Pipeline2);  //TSP -> NN -> Generations( g, ForkJoin ( n, GA -> 2-OPT ) ) -> TSP'
	pipelines.push_back(Pipeline3);  //TSP -> NN -> Generations( g, ForkJoin ( n, ACO -> 2-OPT ) ) -> TSP'
	
	if (p > pipelines.size())
	{
		std::cerr << "Invalid pipeline #" << p 
			<< ", max # of pipelines = " << pipelines.size() << std::endl;
		exit(1);
	}

	auto tsp_instance = read_tsp_instance();
	pipelines[p - 1](tsp_instance, n, g);
}