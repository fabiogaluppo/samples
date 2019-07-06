//Sample provided by Fabio Galuppo
//June 2019
//Inspired by Don Knuth's article on Algorithms for Scientific American in 1977

#ifndef ALGORITHMS_1977_HPP
#define ALGORITHMS_1977_HPP

#include <vector>
#include <string>
#include <cstddef>
#include <algorithm>
#include <memory>
#include <utility>

namespace algorithms_1977
{
	struct word_count final
	{
		std::string word;
		std::size_t count;
	};

	static inline const std::vector<word_count>& words()
	{
		//monotonically decreasing order by count
		static std::vector<word_count> _words
		{
			{ "THE", 15568 }, { "OF", 9767 },   { "AND", 7638 },   { "TO", 5739 },
			{ "A", 5074 },    { "IN", 4312 },   { "THAT", 3017 },  { "IS", 2509 },
			{ "I", 2292 },    { "IT", 2255 },   { "FOR", 1869 },   { "AS", 1853 },
			{ "WITH", 1849 }, { "WAS", 1761 },  { "HIS", 1732 },   { "HE", 1727 },
			{ "BE", 1535 },   { "NOT", 1469 },  { "BY", 1392 },    { "BUT", 1379 },
			{ "HAVE", 1344 }, { "YOU", 1336 },  { "WHICH", 1291 }, { "ARE", 1222 },
			{ "ON", 1155 },   { "OR", 1101 },   { "HER", 1093 },   { "HAD", 1062 },
			{ "AT", 1053 },   { "FROM", 1039 }, { "THIS", 1021 }
		};
		return _words;
	}

	static inline std::vector<word_count> order_by_word(const std::vector<word_count>& words)
	{
		std::vector<word_count> result(std::begin(words), std::end(words));
		std::sort(std::begin(result), std::end(result),
			[](const word_count lhs, const word_count rhs) { return lhs.word < rhs.word; });
		return result;
	}
	
	static inline const std::vector<word_count>& words_ordered_by_word()
	{
		static std::vector<word_count> _words = order_by_word(words());
		return _words;
	}

	static inline std::vector<word_count> order_by_count_decr(const std::vector<word_count>& words)
	{
		std::vector<word_count> result(std::begin(words), std::end(words));
		std::sort(std::begin(result), std::end(result),
			[](const word_count lhs, const word_count rhs) { return lhs.count > rhs.count; });
		return result;
	}

	static inline const std::vector<word_count>& words_ordered_by_count_decr()
	{
		static std::vector<word_count> _words = order_by_count_decr(words());
		return _words;
	}

	const std::size_t NOT_FOUND = -1;

	//Searching a Computer's Memory
	static inline std::size_t sequencial_search(const std::vector<word_count>& words, const std::string& word)
	{
		std::size_t N = words.size();
		while (N)
		{
			if (words[--N].word == word)
				return N;
		}
		return NOT_FOUND;
	}

	static inline std::size_t sequencial_search(const std::vector<word_count>& words, const std::string& word, std::vector<std::size_t>& path)
	{
		std::size_t N = words.size();
		while (N)
		{
			path.push_back(N - 1);
			if (words[--N].word == word)
				return N;
		}
		path.push_back(NOT_FOUND);
		return NOT_FOUND;
	}

	//The Advantage of Order
	static inline std::size_t binary_search(const std::vector<word_count>& words, const std::string& word)
	{
		std::size_t l = 0, r = words.size(), mid;
		while (l != r)
		{
			//mid = (r + l) / 2; //overflow case
			mid = l + (r - l) / 2; //no overflow case
			int comp = word.compare(words[mid].word);
			if (comp == 0)
				return mid;
			else if (comp < 0)
				r = mid;
			else /* if (comp > 0) */
				l = mid + 1;
		}
		return NOT_FOUND;
	}

	static inline std::size_t binary_search(const std::vector<word_count>& words, const std::string& word, std::vector<std::size_t>& path)
	{
		std::size_t l = 0, r = words.size(), mid;
		while (l != r)
		{
			//mid = (r + l) / 2; //overflow case
			mid = l + (r - l) / 2; //no overflow case
			path.push_back(mid);
			int comp = word.compare(words[mid].word);
			if (comp == 0)
				return mid;
			else if (comp < 0)
				r = mid;
			else /* if (comp > 0) */
				l = mid + 1;
		}
		path.push_back(NOT_FOUND);
		return NOT_FOUND;
	}

	//Binary Tree Search
	struct bst_node final
	{
		bst_node(size_t index) : index(index) {}
		std::size_t index = NOT_FOUND;
		std::unique_ptr<bst_node> left = nullptr;
		std::unique_ptr<bst_node> right = nullptr;
	};

	static std::unique_ptr<bst_node> bst_insert_recursive(std::unique_ptr<bst_node> ptr, size_t index, const std::vector<word_count>& words)
	{
		if (ptr == nullptr)
			return std::unique_ptr<bst_node>(new bst_node(index));
		if (words[index].word < words[ptr->index].word)
			ptr->left = bst_insert_recursive(std::move(ptr->left), index, words);
		else
			ptr->right = bst_insert_recursive(std::move(ptr->right), index, words);
		return ptr;
	}

	static inline std::unique_ptr<bst_node> build_binary_search_tree(const std::vector<word_count>& words)
	{
		std::unique_ptr<bst_node> root = nullptr;
		for (std::string word : {"I", "BY", "THAT", "AS", "HAVE", "NOT", "WAS",
			"AND", "BE", "FROM", "HER", "IS", "ON", "THIS", "WITH",
			"A", "ARE", "AT", "BUT", "FOR", "HAD", "HE", "HIS", "IN",
			"IT", "OF", "OR", "THE", "TO", "WHICH", "YOU"})
			root = bst_insert_recursive(std::move(root), binary_search(words, word), words);
		return std::move(root);
	}
	
	static inline const bst_node* words_binary_search_tree_root()
	{
		static std::unique_ptr<bst_node> root = build_binary_search_tree(words_ordered_by_word());
		return root.get();
	}

	static inline std::unique_ptr<bst_node> build_optimum_binary_search_tree(const std::vector<word_count>& words)
	{
		std::unique_ptr<bst_node> root = nullptr;
		for (std::string word : {"OF", "FOR", "THE", "AND", "IN", "THAT", "TO",
			"A", "BE", "HE", "IT", "ON", "THIS", "WITH", "AS",
			"BY", "HAD", "HIS", "IS", "NOT", "OR", "WAS", "YOU",
			"ARE", "AT", "BUT", "FROM", "HAVE", "HER", "I", "WHICH"})
			root = bst_insert_recursive(std::move(root), binary_search(words, word), words);
		return std::move(root);
	}

	static inline const bst_node* words_optimum_binary_search_tree_root()
	{
		static std::unique_ptr<bst_node> root = build_optimum_binary_search_tree(words_ordered_by_word());
		return root.get();
	}

	static std::size_t bst_search_recursive(const bst_node* ptr, const std::vector<word_count>& words, const std::string& word)
	{
		if (ptr == nullptr)
			return NOT_FOUND;
		std::size_t index = ptr->index;
		int comp = word.compare(words[index].word);
		if (comp == 0)
			return index;
		if (comp < 0)
			return bst_search_recursive(ptr->left.get(), words, word);
		return bst_search_recursive(ptr->right.get(), words, word);
	}

	static std::size_t bst_search_recursive(const bst_node* ptr, const std::vector<word_count>& words, const std::string& word, std::vector<std::size_t>& path)
	{
		if (ptr == nullptr)
		{
			path.push_back(NOT_FOUND);
			return NOT_FOUND;
		}
		std::size_t index = ptr->index;
		path.push_back(index);
		int comp = word.compare(words[index].word);
		if (comp == 0)
			return index;
		if (comp < 0)
			return bst_search_recursive(ptr->left.get(), words, word, path);
		return bst_search_recursive(ptr->right.get(), words, word, path);
	}

	//Hashing
	static inline std::size_t hash(const std::string& word, std::size_t bucket_size)
	{
		std::size_t h = 0;
		for (char ch : word)
			h += (ch - 'A' + 1);		
		return --h % bucket_size;
	}

	static inline void hash_table_insert(std::vector<word_count>& dic, const word_count& wc)
	{
		std::size_t bucket_size = dic.size();
		std::size_t h = hash(wc.word, bucket_size);
		while (!dic[h].word.empty())
			if (--h == NOT_FOUND)
				h = bucket_size - 1;
		dic[h] = wc;
	}

	static inline std::vector<word_count> build_hash_table(const std::vector<word_count>& words)
	{
		std::vector<word_count> dic(1 + words.size(), word_count());
		for (const word_count& w : words)
			hash_table_insert(dic, w);
		return dic;
	}

	static inline const std::vector<word_count>& words_hash_table()
	{
		static std::vector<word_count> hash_table = build_hash_table(words_ordered_by_count_decr());
		return hash_table;
	}

	static inline std::size_t hash_table_search(const std::vector<word_count>& dic, const std::string& word)
	{
		std::size_t bucket_size = dic.size();
		std::size_t h = hash(word, bucket_size);
		while (!dic[h].word.empty())
		{
			if (word == dic[h].word)
				return h;
			if (--h == NOT_FOUND)
				h = bucket_size - 1;
		}
		return NOT_FOUND;
	}

	static inline std::size_t hash_table_search(const std::vector<word_count>& dic, const std::string& word, std::vector<std::size_t>& path)
	{
		std::size_t bucket_size = dic.size();
		std::size_t h = hash(word, bucket_size);
		while (!dic[h].word.empty())
		{
			path.push_back(h);
			if (word == dic[h].word)
				return h;
			if (--h == NOT_FOUND)
				h = bucket_size - 1;
		}
		path.push_back(NOT_FOUND);
		return NOT_FOUND;
	}
	
	//Improving Unsuccessful Searches
	static inline void ordered_hash_table_insert(std::vector<word_count>& dic, word_count wc)
	{
		std::size_t bucket_size = dic.size();
		std::size_t h = hash(wc.word, bucket_size);
		while (!dic[h].word.empty())
		{
			if (wc.word.compare(dic[h].word) > 0)
				std::swap(dic[h], wc);
			if (--h == NOT_FOUND)
				h = bucket_size - 1;
		}	
		dic[h] = wc;
	}
	
	static inline std::vector<word_count> build_ordered_hash_table(const std::vector<word_count>& words)
	{
		std::vector<word_count> dic(1 + words.size(), word_count());
		for (const word_count& w : words)
			ordered_hash_table_insert(dic, w);
		return dic;
	}

	static inline const std::vector<word_count>& words_ordered_hash_table()
	{
		static std::vector<word_count> ordered_hash_table = build_ordered_hash_table(words_ordered_by_word());
		return ordered_hash_table;
	}

	static inline std::size_t ordered_hash_table_search(const std::vector<word_count>& dic, const std::string& word)
	{
		std::size_t bucket_size = dic.size();
		std::size_t h = hash(word, bucket_size);
		while (!dic[h].word.empty())
		{
			int comp = word.compare(dic[h].word);
			if (comp > 0)
				break;
			if (comp == 0)
				return h;
			if (--h == NOT_FOUND)
				h = bucket_size - 1;
		}
		return NOT_FOUND;
	}

	static inline std::size_t ordered_hash_table_search(const std::vector<word_count>& dic, const std::string& word, std::vector<std::size_t>& path)
	{
		std::size_t bucket_size = dic.size();
		std::size_t h = hash(word, bucket_size);
		while (!dic[h].word.empty())
		{
			path.push_back(h);
			int comp = word.compare(dic[h].word);
			if (comp > 0)
				break;
			if (comp == 0)
				return h;
			if (--h == NOT_FOUND)
				h = bucket_size - 1;
		}
		path.push_back(NOT_FOUND);
		return NOT_FOUND;
	}
}

#endif //ALGORITHMS_1977_HPP