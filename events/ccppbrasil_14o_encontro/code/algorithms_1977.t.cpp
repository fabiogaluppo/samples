//Sample provided by Fabio Galuppo
//June 2019
//Inspired by Don Knuth's article on Algorithms for Scientific American in 1977

#include "algorithms_1977.hpp"

#include <iostream>
#include <string>
#include <vector>
#include <cstddef>

namespace algorithms_1977
{
	static void display_recursive(const bst_node* ptr, std::size_t depth, const std::vector<word_count>& words)
	{
		if (ptr)
		{
			for (std::size_t i = 0; i < depth; ++i) std::cout << "--"; 
			if (depth) std::cout << " ";
			std::size_t index = ptr->index;
			std::cout << index << ":" << words[index].word << "\n";
			display_recursive(ptr->left.get(), depth + 1, words);
			display_recursive(ptr->right.get(), depth + 1, words);
		}
	}

	static void display_path(const std::vector<size_t>& path, const std::vector<word_count>& words)
	{
		std::cout << "path: ";
		for (size_t i : path)
			std::cout << (i == NOT_FOUND ? "-" : words[i].word) << " ";
		std::cout << "\n";
	}

	static void test_interactive()
	{
		std::cout << "?: display words                      1: Searching a Computer's Memory\n";
		std::cout << "!: display words (ordered)            2: The Advantage of Order\n";
		std::cout << "*: display words (tree)               3: Binary Tree Search\n";
		std::cout << "#: display words (hash table)         4: Hashing (linear probing)\n";
		std::cout << "$: display words (ordered hash table) 5: Improving Unsuccessful Searches\n";

		std::string cmd; 
		while (std::cin >> cmd)
		{
			switch (cmd[0])
			{
			case '?':
				{
					std::size_t i = 0;
					for (const word_count& wc : words())
						std::cout << (i++) << ":" << wc.word << " ";
					std::cout << "\n";			
				}
				break;
			case '!':
				{
					std::size_t i = 0;
					for (const word_count& wc : words_ordered_by_word())
						std::cout << (i++) << ":" << wc.word << " ";
					std::cout << "\n";
				}
				break;
			case '*':
				{
					const bst_node* root = words_binary_search_tree_root();
					//const bst_node* root = words_optimum_binary_search_tree_root();
					display_recursive(root, 0, words_ordered_by_word());
				}
				break;
			case '#':
				{
					std::size_t i = 0;
					const std::vector<word_count>& words = words_hash_table();
					std::size_t bucket_size = words.size();
					for (const word_count& wc : words)
						if (!wc.word.empty())
							std::cout << (i++) << ":[" << wc.word << "," << hash(wc.word, bucket_size) << "]\n";
						else
							std::cout << (i++) << ":[]\n";
					std::cout << "\n";
				}
				break;
			case '$':
				{
					std::size_t i = 0;
					const std::vector<word_count>& words = words_ordered_hash_table();
					std::size_t bucket_size = words.size();
					for (const word_count& wc : words)
						if (!wc.word.empty())
							std::cout << (i++) << ":[" << wc.word << "," << hash(wc.word, bucket_size) << "] ";
						else
							std::cout << (i++) << ":[] ";
					std::cout << "\n";
				}
				break;
			case '1':
				{
					std::cout << "Searching a Computer's Memory\n";
					std::cout << "-----------------------------\n";
					std::cout << "word to find? ";
					std::string word; std::cin >> word;
					std::cout << "display path? ";
					bool show_path; std::cin >> show_path;
					std::vector<std::size_t> path;
					const std::vector<word_count>& words_ = words();
					std::size_t pos =
						show_path ?
						sequencial_search(words_, word, path) :
						sequencial_search(words_, word);
					if (pos != NOT_FOUND)
						std::cout << "found at position " << pos << "\n";
					else
						std::cout << "not found\n";
					if (show_path)
						display_path(path, words_);
				}
				break;
			case '2':
				{
					std::cout << "The Advantage of Order\n";
					std::cout << "----------------------\n";
					std::cout << "word to find? ";
					std::string word; std::cin >> word;
					std::cout << "display path? ";
					bool show_path; std::cin >> show_path;
					std::vector<std::size_t> path;
					const std::vector<word_count>& words_ = words_ordered_by_word();
					std::size_t pos =
						show_path ?
						binary_search(words_, word, path) :
						binary_search(words_, word);
					if (pos != NOT_FOUND)
						std::cout << "found at position " << pos << "\n";
					else
						std::cout << "not found\n";
					if (show_path)
						display_path(path, words_);
				}
				break;
			case '3':
				{
					std::cout << "Binary Tree Search\n";
					std::cout << "------------------\n";
					std::cout << "word to find? ";
					std::string word; std::cin >> word;
					std::cout << "display path? ";
					bool show_path; std::cin >> show_path;
					std::vector<std::size_t> path;
					const std::vector<word_count>& words_ = words_ordered_by_word();
					const bst_node* root = words_binary_search_tree_root();
					//const bst_node* root = words_optimum_binary_search_tree_root();
					std::size_t pos =
						show_path ?
						bst_search_recursive(root, words_, word, path) :
						bst_search_recursive(root, words_, word);
					if (pos != NOT_FOUND)
						std::cout << "found at position " << pos << "\n";
					else
						std::cout << "not found\n";
					if (show_path)
						display_path(path, words_);
				}
				break;
			case '4':
				{
					std::cout << "Hashing\n";
					std::cout << "-------\n";
					std::cout << "word to find? ";
					std::string word; std::cin >> word;
					std::cout << "display path? ";
					bool show_path; std::cin >> show_path;
					std::vector<std::size_t> path;
					const std::vector<word_count>& dic = words_hash_table();
					std::size_t pos = 
						show_path ?
						hash_table_search(dic, word, path) :
						hash_table_search(dic, word);
					if (pos != NOT_FOUND)
						std::cout << "found at position " << pos << "\n";
					else
						std::cout << "not found\n";
					if (show_path)
						display_path(path, dic);
				}
				break;
			case '5':
				{
					std::cout << "Improving Unsuccessful Searches\n";
					std::cout << "-------------------------------\n";
					std::cout << "word to find? ";
					std::string word; std::cin >> word;
					std::cout << "display path? ";
					bool show_path; std::cin >> show_path;
					std::vector<std::size_t> path;
					const std::vector<word_count>& dic = words_ordered_hash_table();
					std::size_t pos =
						show_path ?
						ordered_hash_table_search(dic, word, path) :
						ordered_hash_table_search(dic, word);
					if (pos != NOT_FOUND)
						std::cout << "found at position " << pos << "\n";
					else
						std::cout << "not found\n";
					if (show_path)
						display_path(path, dic);
				}
				break;
			default:
				std::cout << "input ignored\n";
				break;
			}			
		}
	}
}

int algorithms_1977_main(int argc, char *argv[])
{
	using algorithms_1977::test_interactive;

	test_interactive();
	return 0;
}