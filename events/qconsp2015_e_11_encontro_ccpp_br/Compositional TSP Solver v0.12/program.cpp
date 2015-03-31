//Sample provided by Fabio Galuppo
//Compositional TSP Solver version 0.12
//March 2015

#include "src/_tsp.hpp"

#pragma region "read functions for test"
void read_tsp(int coords[], size_t size, tsp_class& tsp_instance)
{
	int city_number = 0;
	for (size_t i = 0; i < size; i += 2)
	{
		int x = coords[i], y = coords[i + 1];
		tsp_class::city_info info = { ++city_number, std::make_pair(x, y) };
		tsp_instance.cities.push_back(info);
	}
}

tsp_class read_berlin52_opt_tsp()
{
	int coords[] = {
		/* 1 */ 565, 575,
		/* 49 */ 605, 625,
		/* 32 */ 575, 665, 
		/* 45 */ 555, 815,
		/* 19 */ 510, 875,
		/* 41 */ 475, 960,
		/* 8 */ 525, 1000,
		/* 9 */ 580, 1175,
		/* 10 */ 650, 1130,
		/* 43 */ 875, 920,
		/* 33 */ 1150, 1160,
		/* 51 */ 1340, 725,
		/* 11 */ 1605, 620,
		/* 52 */ 1740, 245,
		/* 14 */ 1530, 5,
		/* 13 */ 1465, 200,
		/* 47 */ 1170, 65,		
		/* 26 */ 1215, 245,
		/* 27 */ 1320, 315,
		/* 28 */ 1250, 400,
		/* 12 */ 1220, 580,
		/* 25 */ 975, 580,
		/* 4 */ 945, 685,
		/* 6 */ 880, 660,
		/* 15 */ 845, 680,
		/* 5 */ 845, 655,
		/* 24 */ 835, 625,
		/* 48 */ 830, 610,
		/* 38 */ 795, 645,
		/* 37 */ 770, 610,
		/* 40 */ 760, 650,
		/* 39 */ 720, 635,
		/* 36 */ 685, 610,
		/* 35 */ 685, 595,
		/* 34 */ 700, 580,
		/* 44 */ 700, 500,
		/* 46 */ 830, 485,
		/* 16 */ 725, 370,
		/* 29 */ 660, 180,
		/* 50 */ 595, 360,
		/* 20 */ 560, 365,
		/* 23 */ 480, 415,
		/* 30 */ 410, 250,
		/* 2 */ 25, 185,
		/* 7 */ 25, 230,
		/* 42 */ 95, 260,
		/* 21 */ 300, 465,
		/* 17 */ 145, 665,
		/* 3 */ 345, 750,
		/* 18 */ 415, 635,
		/* 31 */ 420, 555,
		/* 22 */ 520, 585,
	};

	tsp_class tsp_instance;
	read_tsp(coords, sizeof(coords) / sizeof(int), tsp_instance);
	return tsp_instance;
}

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
	read_tsp(coords, sizeof(coords) / sizeof(int), tsp_instance);
	return tsp_instance;
}
#pragma endregion

#pragma region "getopt"
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
	static const char *place = EMSG;        /* option letter processing */
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
	}
	else
		optopt = *place++;

	/* See if option letter is one the caller wanted... */
	if (optopt == ':' || (oli = const_cast<char*>(strchr(ostr, optopt))) == NULL) {
		if (*place == 0)
			++optind;
		if (opterr && *ostr != ':')
			(void) fprintf(stderr,			
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
	}
	else {
		/* Option-argument is either the rest of this argument or the
		entire next argument. */
		if (*place)
			optarg = const_cast<char*>(place);
		else if (nargc > ++optind)
			optarg = nargv[optind];
		else {
			/* option-argument absent */
			place = EMSG;
			if (*ostr == ':')
				return (BADARG);
			if (opterr)
				(void) fprintf(stderr,
				"%s: option requires an argument -- %c\n",				
				"getopt", optopt);
			return (BADCH);
		}
		place = EMSG;
		++optind;
	}
	return (optopt); /* return option letter */
}

#include <iostream>

void usage(void)
{
	std::cerr << "usage: tspsystem -g:N -n:N" << std::endl;
	std::cerr << "where" << std::endl;
	std::cerr << "  g: number of generations" << std::endl;
	std::cerr << "  n: number of tasks" << std::endl;
	std::cerr << "  N: an integer greater than 0" << std::endl;
	exit(1);
}

void get_args(int& argc, char* argv[], unsigned int& g, unsigned int& n)
{
	int ch;
	setlocale(LC_CTYPE, "");
	while ((ch = getopt(argc, argv, "g:n:")) != -1)
	{
		switch ((char) ch)
		{
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

	if (0 == g || 0 == n)
		usage();

	argv += optind;
	argc -= optind;
}

#pragma endregion

#pragma region "Pipelines"

#pragma region "Pipeline 1"
//TSP -> NN -> Generations( g, ForkJoin ( n, SA -> 2-OPT ) ) -> TSP'
void Pipeline1(tsp_class& tsp_instance, unsigned int number_of_tasks, unsigned int number_of_generations)
{
#pragma region "PipelineConfiguration"
	auto a = Args<General_args_type>(make_General_args(number_of_generations, number_of_tasks));
	//auto sa = Args<SA_args_type>(make_SA_args(1000.0, 0.00001, 0.999, 400));
	//auto sa = Args<SA_args_type>(make_SA_args(1200.0, 0.0000001, 0.99, 15));
	auto sa = Args<SA_args_type>(make_SA_args(1200.0, 0.0000001, 0.991, 120));

	const char* pipeline_description = "TSP -> NN -> Generations( g, ForkJoin ( n, SA -> 2-OPT ) ) -> TSP'";
	display_args(pipeline_description, a, sa);

	auto g = a[0].number_of_iterations_or_generations;
	auto n = a[0].number_of_tasks_in_parallel;
	auto _TSP = unit(just(tsp_instance));
	auto _DisplayInput = Display("TSP INPUT", DisplayFlags::All);
	auto _NN = Measure(NN(), Display("NEAREST NEIGHBOUR", DisplayFlags::EmitMathematicaGraphPlot));
	auto _SA_2OPT = Chain<SA, _2OPT>(SA(sa[0].initial_temperature, sa[0].stopping_criteria_temperature,
		sa[0].decreasing_factor, sa[0].monte_carlo_steps), _2OPT());
	auto _ForkJoin = [](unsigned int n, tsp_monad::function_type map_fun){ return Measure(ForkJoin(n, map_fun)); };
	auto _DisplayOutput = Display("TSP OUTPUT", DisplayFlags::EmitMathematicaGraphPlot);
#pragma endregion

	//TSP -> NN -> Generations( g, ForkJoin ( n, SA -> 2-OPT ) ) -> TSP'
	auto result = _TSP
		.map(_DisplayInput)
		.map(_NN)
		.map(Generations(g, _ForkJoin(n, _SA_2OPT)))
		.map(_DisplayOutput);
}
#pragma endregion

#pragma region "Pipeline 0"
//TSP -> TSP
void Pipeline0(tsp_class& tsp_instance)
{
	auto _TSP = unit(just(tsp_instance));
	auto _DisplayInput = Display("TSP INPUT", DisplayFlags::All);	
	auto result = _TSP.map(_DisplayInput);
}
#pragma endregion

#pragma region "Pipeline 2"
//TSP -> (SA -> 2-OPT) -> TSP'
void Pipeline2(tsp_class& tsp_instance)
{
#pragma region "PipelineConfiguration"
	auto a = Args<General_args_type>();

	auto sa = Args<SA_args_type>(make_SA_args(1200.0, 0.0000001, 0.99, 15));	

	const char* pipeline_description = "TSP -> (SA -> 2-OPT) -> TSP'";
	display_args(pipeline_description, a, sa);

	auto _TSP = unit(just(tsp_instance));

	auto _DisplayInput = Display("TSP INPUT", DisplayFlags::All);	
	
	auto _SA_2OPT = Chain<SA, _2OPT> (
		SA(sa[0].initial_temperature, sa[0].stopping_criteria_temperature, sa[0].decreasing_factor, sa[0].monte_carlo_steps), 
		_2OPT()
	);	
	
	auto _DisplayOutput = Display("TSP OUTPUT", DisplayFlags::EmitMathematicaGraphPlot);
#pragma endregion

	//TSP -> (SA -> 2-OPT) -> TSP'
	auto result = _TSP
		.map(_DisplayInput)
		.map(_SA_2OPT)		
		.map(_DisplayOutput);
}
#pragma endregion

#pragma endregion

#pragma region "Monadic Laws Proofing"

void MonadicLawsProofing()
{
	//Scala notation: unit(x) flatMap f == f(x)
	{
		//Left Identity (1st Law)
		auto x = just(read_att48_tsp());

		auto f = [](tsp_monad::value_type t){ return ref(t).do_cycle_length(); };

		double fun_result = bnd<double>(unit(x), f); //functional composition
		double oo_result = unit(x).map<double>(f); //object-oriented

		bool fun_holds = fun_result == f(x);
		bool oo_holds = oo_result == f(x);

		std::cout << "1st Law (Left Identity) " << (fun_holds ? "holds" : "doesn't hold") << std::endl;
		std::cout << "1st Law (Left Identity) " << (oo_holds ? "holds" : "doesn't hold") << std::endl;
	}


	//Scala notation: m flatMap unit == m
	{
		//Right Identity (2nd Law)
		auto m = unit(just(read_att48_tsp()));

		auto fun_result = bnd(m, unit);
		auto oo_result = m.map(unit);

		bool fun_holds = fun_result == m;
		bool oo_holds = oo_result == m;

		std::cout << "2nd Law (Right Identity) " << (fun_holds ? "holds" : "doesn't hold") << std::endl;
		std::cout << "2nd Law (Right Identity) " << (oo_holds ? "holds" : "doesn't hold") << std::endl;
	}


	//Scala notation: m flatMap f flatMap g == m flatMap (x => f(x) flatMap g)
	{
		//Associativity (3rd Law)
		auto m = unit(just(read_att48_tsp()));

		auto f = [](const tsp_monad::value_type& t) -> tsp_monad
		{
			auto u = ref(t);
			std::swap(u.cities[0], u.cities[1]);
			return unit(just(u));
		};

		auto g = [](const tsp_monad::value_type& t) -> tsp_monad
		{
			auto u = ref(t);
			std::swap(u.cities[2], u.cities[3]);
			return unit(just(u));
		};

		auto fun_result_1 = bnd(bnd(m, f), g);
		auto fun_result_2 = bnd(m, [&](const tsp_monad::value_type& t) {
			return bnd(f(t), g);
		});

		auto oo_result_1 = m.map(f).map(g);
		auto oo_result_2 = m.map([&](const tsp_monad::value_type& t) {
			return f(t).map(g);
		});

		bool fun_holds = fun_result_1 == fun_result_2;
		bool oo_holds = oo_result_1 == oo_result_2;

		std::cout << "3rd Law (Associativity) " << (fun_holds ? "holds" : "doesn't hold") << std::endl;
		std::cout << "3rd Law (Associativity) " << (oo_holds ? "holds" : "doesn't hold") << std::endl;
	}
}

#pragma endregion

//tsplibreader  "C:\Users\Fabio Galuppo\Documents\Visual Studio 2012\Projects\TSPSolution\_tsplib\berlin52.tsp" | tspsystem -g:32 -n:32 > berlin52.txt
int main(int argc, char* argv[])
{
	//MonadicLawsProofing();

	unsigned int g = 0, n = 0;
	get_args(argc, argv, g, n);
	
	//auto tsp_instance = read_tsp_instance();
	auto tsp_instance = read_att48_tsp();
	//auto tsp_instance = read_berlin52_opt_tsp();
	
	Pipeline1(tsp_instance, n, g);
	//Pipeline0(tsp_instance);
	//Pipeline2(tsp_instance);
}