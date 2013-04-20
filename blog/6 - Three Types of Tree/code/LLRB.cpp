//Sample provided by Fabio Galuppo
//February 2013

#include <memory>
#include <functional>
#include <queue>
#include <iostream>

template <class T>
struct node
{
	typedef std::shared_ptr<node<T>> llrb_node;

	T value;
	llrb_node left, right;
	bool is_red;

	node(const T& value, bool is_red) : value(value), is_red(is_red) {}
};

#define SHOW_STEPS

template <class T>
struct left_leaning_red_black_bst
{
	typedef const T& const_reference;	
	typedef typename node<T>::llrb_node llrb_node;
	typedef typename llrb_node& llrb_node_reference;
	typedef typename const llrb_node& const_llrb_node_reference;
	typedef std::function<int(T, T)> comparer;
	
	left_leaning_red_black_bst(comparer compare) : 
		Compare_(compare) {}

	void insert(const_reference value)
	{
		#ifdef SHOW_STEPS
		std::cout << value << ":\n";
		#endif

		Root_ = recursive_insert(Root_, value);
		Root_->is_red = BLACK;

		#ifdef SHOW_STEPS
		std::cout << "\n";
		#endif
	}

	llrb_node get_root() const
	{
		return Root_;
	}

private:
	llrb_node recursive_insert(llrb_node_reference n, const_reference value)
	{
		if (nullptr == n)
			return llrb_node(new node<T>(value, RED));
		
		//search insertion point
		int cmp = Compare_(value, n->value);
		if(cmp < 0)
			n->left = recursive_insert(n->left, value);
		else if(cmp > 0)
			n->right = recursive_insert(n->right, value);
		else //ignore
			return n;

		//adjust nodes
		if(is_red(n->right) && !is_red(n->left))
			n = rotate_left(n);

		if (is_red(n->left) && is_red(n->left->left))
			n = rotate_right(n);
		
		if (is_red(n->left) && is_red(n->right)) 
			flip_colors(n);

		return n;
	}

	bool is_red(const_llrb_node_reference n)
	{
		if (nullptr == n)
			return false;
		return n->is_red;
	};

	llrb_node rotate_left(const_llrb_node_reference n)
	{
		#ifdef SHOW_STEPS
		std::cout << "rotate left" << "; ";
		#endif

		llrb_node temp = n->right;
		n->right = temp->left; 
		temp->left = n;
		temp->is_red = n->is_red;
		n->is_red = RED;
		return temp;
	}

	llrb_node rotate_right(const_llrb_node_reference n)
	{
		#ifdef SHOW_STEPS
		std::cout << "rotate right" << "; ";
		#endif
		llrb_node temp = n->left;
		n->left = temp->right;
		temp->right = n;
		temp->is_red = n->is_red;
		n->is_red = RED;
		return temp;
	}

	void flip_colors(const_llrb_node_reference n)
	{
		#ifdef SHOW_STEPS
		std::cout << "flip colors" << "; ";
		#endif

		n->left->is_red = BLACK;
		n->right->is_red = BLACK;
		n->is_red = RED;
	}

private:
	llrb_node Root_;
	comparer Compare_;

	static const bool RED = true;
	static const bool BLACK = false;
};

template<typename T>
void breadth_first_traversal(const left_leaning_red_black_bst<T>& llrb, std::function<void(const T&)> visit)
{
	std::queue<left_leaning_red_black_bst<T>::llrb_node> q; 
	q.push(llrb.get_root());
	while(!q.empty())
	{
		auto temp = q.front();
		q.pop();
		visit(temp->value);
		if (nullptr != temp->left)  q.push(temp->left);
		if (nullptr != temp->right) q.push(temp->right);
	}
}

int main()
{
	left_leaning_red_black_bst<int> llrb([](int lhs, int rhs){
		if (lhs < rhs) return -1;
		if (lhs > rhs) return  1;
		return 0;
	});
	
	for( int i = 1; i <= 10; ++i )
		llrb.insert(i);

	std::cout << "\n";
	std::cout << "Level order traversal sequence: ";
	breadth_first_traversal<int>(llrb, [](const int& i){ std::cout << i << " "; });
	std::cout << "\n";
}