//Sample provided by Fabio Galuppo
//February 2013

//cl /I"C:\dev\boost_1_51_0" /EHsc /Fe"bf.exe" /O2 graph.cpp
//g++ -Wall -o bf.exe -O graph.cpp

#include <boost/graph/adjacency_list.hpp>
#include <boost/graph/breadth_first_search.hpp>

using namespace boost;

template<typename TimeMap>
struct bfs_time_visitor : public default_bfs_visitor
{
	typedef typename property_traits<TimeMap>::value_type T;
	
	bfs_time_visitor(TimeMap tmap, T & t) 
		: TimeMap_(tmap), Time_(t) {}

	template<typename Vertex, typename Graph>
	void discover_vertex(Vertex u, const Graph & g) const
	{
		put(TimeMap_, u, Time_++);
	}

private:
	TimeMap TimeMap_;
	T & Time_;
};

int main()
{
	const char* nodes = "ABCDEFGHI";
	enum { a, b, c, d, e, f, g, h, i, N };

	typedef std::pair<int, int> E;

	E edges[] = 
	{ 
		E(f, b), E(f, g), E(b, a), E(b, d), 
		E(d, c), E(d, e), E(g, i), E(i, h) 
	};

	const int n_edges = sizeof(edges) / sizeof(E);

	typedef adjacency_list<vecS, vecS, undirectedS> graph_t;
	graph_t tree(edges, edges + n_edges, 
		graph_traits<graph_t>::vertices_size_type(N));

	typedef graph_traits<graph_t>::vertices_size_type Size;
	std::vector<Size> visited_indices(num_vertices(tree));
  
	Size time = 0;
	breadth_first_search(tree, vertex(f, tree), 
		visitor(bfs_time_visitor<Size*>(&visited_indices[0], time)));

	char result[N];
	for (int i = 0; i < N; ++i)  
		result[visited_indices[i]] = nodes[i];
	
	std::cout << "Level order traversal sequence: ";
	for (int i = 0; i < N; ++i)
		std::cout << result[i] << " ";
	std::cout << std::endl;
}