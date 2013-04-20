//Adaptation and Sample provided by Fabio Galuppo
//February 2013


//Linux kernel files inspected:
//kernel.h:
//http://git.kernel.org/?p=linux/kernel/git/torvalds/linux.git;a=blob;f=include/linux/kernel.h;h=c566927efcbd254efa6c30c0a82fd10ccb11f363;hb=c727b4c63c9bf33c65351bbcc738161edb444b24
//rbtree_augmented.h:
//http://git.kernel.org/?p=linux/kernel/git/torvalds/linux.git;a=blob;f=include/linux/rbtree_augmented.h;h=fea49b5da12a99bfa6c87b865a461a2a28e0495b;hb=c727b4c63c9bf33c65351bbcc738161edb444b24
//rbtree.h
//http://git.kernel.org/?p=linux/kernel/git/torvalds/linux.git;a=blob;f=include/linux/rbtree.h;h=0022c1bb1e26398c9db767a8eebc4fa153bff559;hb=c727b4c63c9bf33c65351bbcc738161edb444b24
//rbtree.c:
//http://git.kernel.org/?p=linux/kernel/git/torvalds/linux.git;a=blob;f=lib/rbtree.c;h=c0e31fe2fabf5b99c160fb01daf618cb7c0488b3;hb=c727b4c63c9bf33c65351bbcc738161edb444b24
//rbtree_test.c:
//http://git.kernel.org/?p=linux/kernel/git/torvalds/linux.git;a=blob;f=lib/rbtree_test.c;h=af38aedbd874f0f2ad4f5411c8bfc2187aa2c738;hb=c727b4c63c9bf33c65351bbcc738161edb444b24

//FG:
#define NULL 0
#define typeof decltype
#define offsetof(type, member) (reinterpret_cast<size_t>(&((type*)0)->member))

//kernel.h:
/**
 * container_of - cast a member of a structure out to the containing structure
 * @ptr:        the pointer to the member.
 * @type:       the type of the container struct this is embedded in.
 * @member:     the name of the member within the struct.
 *
*/
//#define container_of(ptr, type, member) ({                      \
//        const typeof( ((type *)0)->member ) *__mptr = (ptr);    \
//        (type *)( (char *)__mptr - offsetof(type,member) );})

//FG: little trick using lambda
#define container_of(ptr, type, member) [&]() -> type*{				    \
        const typeof( ((type *)0)->member ) *__mptr = (ptr);			\
        return (type *)( (char *)__mptr - offsetof(type,member) ); }()

//rbtree.h:
#define rb_parent(r)   ((struct rb_node *)((r)->__rb_parent_color & ~3))

#define rb_entry(ptr, type, member) container_of(ptr, type, member)

__declspec(align(4)) struct rb_node
{
	unsigned long  __rb_parent_color;
	struct rb_node *rb_right;
	struct rb_node *rb_left;
};

struct rb_root 
{
	struct rb_node *rb_node;
};

static inline void rb_link_node(struct rb_node* node, struct rb_node * parent, struct rb_node ** rb_link)
{
	node->__rb_parent_color = (unsigned long)parent;
	node->rb_left = node->rb_right = NULL;
	*rb_link = node;
}

//rbtree_augmented.h:
#define RB_RED	 0
#define RB_BLACK 1

#define __rb_parent(pc)    ((struct rb_node *)(pc & ~3))
 
#define __rb_color(pc)     ((pc) & 1)
#define __rb_is_black(pc)  __rb_color(pc)
#define __rb_is_red(pc)    (!__rb_color(pc))
#define rb_color(rb)       __rb_color((rb)->__rb_parent_color)
#define rb_is_red(rb)      __rb_is_red((rb)->__rb_parent_color)
#define rb_is_black(rb)    __rb_is_black((rb)->__rb_parent_color)

static inline void rb_set_parent_color(struct rb_node *rb, struct rb_node *p, int color)
{
	rb->__rb_parent_color = (unsigned long)p | color;
}

static inline void __rb_change_child(struct rb_node *old, struct rb_node *new_, struct rb_node *parent, struct rb_root *root)
{
	if (parent) {
		if (parent->rb_left == old) parent->rb_left = new_; else parent->rb_right = new_;
	} 
	else
	root->rb_node = new_;
}

//rbtree.c:
static inline struct rb_node *rb_red_parent(struct rb_node *red)
{
	return (struct rb_node *)red->__rb_parent_color;
}

static inline void __rb_rotate_set_parents(struct rb_node *old, struct rb_node *new_, struct rb_root *root, int color)
{
	struct rb_node *parent = rb_parent(old);
	new_->__rb_parent_color = old->__rb_parent_color;
	rb_set_parent_color(old, new_, color);
	__rb_change_child(old, new_, parent, root);
}

static inline void __rb_insert(struct rb_node *node, struct rb_root *root, void (*augment_rotate)(struct rb_node *old, struct rb_node *new_))
{
	struct rb_node *parent = rb_red_parent(node), *gparent, *tmp;

	while (true) {
			/*
				* Loop invariant: node is red
				*
				* If there is a black parent, we are done.
				* Otherwise, take some corrective action as we don't
				* want a red root or two consecutive red nodes.
				*/
			if (!parent) {
					rb_set_parent_color(node, NULL, RB_BLACK);
					break;
			} else if (rb_is_black(parent))
					break;

			gparent = rb_red_parent(parent);

			tmp = gparent->rb_right;
			if (parent != tmp) {    /* parent == gparent->rb_left */
					if (tmp && rb_is_red(tmp)) {
							/*
								* Case 1 - color flips
								*
								*       G            g
								*      / \          / \
								*     p   u  -->   P   U
								*    /            /
								*   n            N
								*
								* However, since g's parent might be red, and
								* 4) does not allow this, we need to recurse
								* at g.
								*/
							rb_set_parent_color(tmp, gparent, RB_BLACK);
							rb_set_parent_color(parent, gparent, RB_BLACK);
							node = gparent;
							parent = rb_parent(node);
							rb_set_parent_color(node, parent, RB_RED);
							continue;
					}

					tmp = parent->rb_right;
					if (node == tmp) {
							/*
								* Case 2 - left rotate at parent
								*
								*      G             G
								*     / \           / \
								*    p   U  -->    n   U
								*     \           /
								*      n         p
								*
								* This still leaves us in violation of 4), the
								* continuation into Case 3 will fix that.
								*/
							parent->rb_right = tmp = node->rb_left;
							node->rb_left = parent;
							if (tmp)
								rb_set_parent_color(tmp, parent, RB_BLACK);
							rb_set_parent_color(parent, node, RB_RED);
							augment_rotate(parent, node);
							parent = node;
							tmp = node->rb_right;
					}

					/*
						* Case 3 - right rotate at gparent
						*
						*        G           P
						*       / \         / \
						*      p   U  -->  n   g
						*     /                 \
						*    n                   U
						*/
					gparent->rb_left = tmp;  /* == parent->rb_right */
					parent->rb_right = gparent;
					if (tmp)
							rb_set_parent_color(tmp, gparent, RB_BLACK);
					__rb_rotate_set_parents(gparent, parent, root, RB_RED);
					augment_rotate(gparent, parent);
					break;
			} else {
					tmp = gparent->rb_left;
					if (tmp && rb_is_red(tmp)) {
							/* Case 1 - color flips */
							rb_set_parent_color(tmp, gparent, RB_BLACK);
							rb_set_parent_color(parent, gparent, RB_BLACK);
							node = gparent;
							parent = rb_parent(node);
							rb_set_parent_color(node, parent, RB_RED);
							continue;
					}

					tmp = parent->rb_left;
					if (node == tmp) {
							/* Case 2 - right rotate at parent */
							parent->rb_left = tmp = node->rb_right;
							node->rb_right = parent;
							if (tmp)
								rb_set_parent_color(tmp, parent, RB_BLACK);
							rb_set_parent_color(parent, node, RB_RED);
							augment_rotate(parent, node);
							parent = node;
							tmp = node->rb_left;
					}

					/* Case 3 - left rotate at gparent */
					gparent->rb_right = tmp;  /* == parent->rb_left */
					parent->rb_left = gparent;
					if (tmp)
							rb_set_parent_color(tmp, gparent, RB_BLACK);
					__rb_rotate_set_parents(gparent, parent, root, RB_RED);
					augment_rotate(gparent, parent);
					break;
			}
	}
}

static inline void dummy_rotate(struct rb_node *old, struct rb_node *new_) 
{
}

 void rb_insert_color(struct rb_node *node, struct rb_root *root)
{
	__rb_insert(node, root, dummy_rotate);
}

 void __rb_insert_augmented(struct rb_node *node, struct rb_root *root, void (*augment_rotate)(struct rb_node *old, struct rb_node *new_))
{
	__rb_insert(node, root, augment_rotate);
}

//rbtree_augmented.h:
#define RB_DECLARE_CALLBACKS(rbstatic, rbname, rbstruct, rbfield,       \
                            rbtype, rbaugmented, rbcompute)             \
static inline void                                                      \
rbname ## _propagate(struct rb_node *rb, struct rb_node *stop)          \
{                                                                       \
        while (rb != stop) {                                            \
                rbstruct *node = rb_entry(rb, rbstruct, rbfield);       \
                rbtype augmented = rbcompute(node);                     \
                if (node->rbaugmented == augmented)                     \
                        break;                                          \
                node->rbaugmented = augmented;                          \
                rb = rb_parent(&node->rbfield);                         \
        }                                                               \
}                                                                       \
static inline void                                                      \
rbname ## _copy(struct rb_node *rb_old, struct rb_node *rb_new)         \
{                                                                       \
        rbstruct *old = rb_entry(rb_old, rbstruct, rbfield);            \
        rbstruct *new_ = rb_entry(rb_new, rbstruct, rbfield);           \
        new_->rbaugmented = old->rbaugmented;                           \
}                                                                       \
static void                                                             \
rbname ## _rotate(struct rb_node *rb_old, struct rb_node *rb_new)       \
{                                                                       \
        rbstruct *old = rb_entry(rb_old, rbstruct, rbfield);            \
        rbstruct *new_ = rb_entry(rb_new, rbstruct, rbfield);           \
        new_->rbaugmented = old->rbaugmented;                           \
        old->rbaugmented = rbcompute(old);                              \
}                                                                       \
rbstatic const struct rb_augment_callbacks rbname = {                   \
        rbname ## _propagate, rbname ## _copy, rbname ## _rotate        \
};

struct rb_augment_callbacks 
{
	void (*propagate)(struct rb_node *node, struct rb_node *stop);
	void (*copy)(struct rb_node *old, struct rb_node *new_);
	void (*rotate)(struct rb_node *old, struct rb_node *new_);
};

static inline void rb_insert_augmented(struct rb_node *node, struct rb_root *root, const struct rb_augment_callbacks *augment)
{
	__rb_insert_augmented(node, root, augment->rotate);
}

//rbtree_test.c:
typedef unsigned int u32;

struct test_node 
{
	struct rb_node rb;
	u32 key;

	/* following fields used for testing augmented rbtree functionality */
	u32 val;
	u32 augmented;
};

static void insert(struct test_node *node, struct rb_root *root)
{
    struct rb_node **new_ = &root->rb_node, *parent = NULL;
    u32 key = node->key;

    while (*new_) 
	{
		parent = *new_;
        if (key < rb_entry(parent, struct test_node, rb)->key)
			new_ = &parent->rb_left;
        else
			new_ = &parent->rb_right;
    }

    rb_link_node(&node->rb, parent, new_);
    rb_insert_color(&node->rb, root);
}

static inline u32 augment_recompute(struct test_node *node)
{
	u32 max = node->val, child_augmented;
    	if (node->rb.rb_left) 
	{
		auto x = rb_entry(node->rb.rb_left, struct test_node, rb);
		child_augmented = x->augmented;
        	if (max < child_augmented) max = child_augmented;
    	}
    
	if (node->rb.rb_right) 
	{
		child_augmented = rb_entry(node->rb.rb_right, struct test_node, rb)->augmented;
		if (max < child_augmented) max = child_augmented;
	}
    
	return max;
}

RB_DECLARE_CALLBACKS(static, augment_callbacks, struct test_node, rb, u32, augmented, augment_recompute)

static void insert_augmented(struct test_node *node, struct rb_root *root)
{
    struct rb_node **new_ = &root->rb_node, *rb_parent = NULL;
    u32 key = node->key;
    u32 val = node->val;
    struct test_node *parent;

    while (*new_) 
	{
        rb_parent = *new_;
        parent = rb_entry(rb_parent, struct test_node, rb);
        if (parent->augmented < val) 
			parent->augmented = val;
        if (key < parent->key)
            new_ = &parent->rb.rb_left;
        else
            new_ = &parent->rb.rb_right;
    }

    node->augmented = val;
    rb_link_node(&node->rb, rb_parent, new_);
    rb_insert_augmented(&node->rb, root, &augment_callbacks);
}

//test case:
#include <queue>
#include <iostream>

void bfs(rb_root* r)
{
	typedef struct test_node* node_ptr;
	
	std::cout <<  "Level order traversal sequence:\n";
	
	std::queue<node_ptr> q;

	q.push(reinterpret_cast<node_ptr>(r->rb_node));
	while(!q.empty())
	{
		node_ptr pnode = q.front();
		q.pop();
		
		std::cout << pnode->key << "(" << (rb_color(&(pnode->rb)) ? "BLK" : "RED") << ")" << " ";
		
		if(nullptr != pnode->rb.rb_left)
			q.push(reinterpret_cast<node_ptr>(pnode->rb.rb_left));
		
		if(nullptr != pnode->rb.rb_right)
			q.push(reinterpret_cast<node_ptr>(pnode->rb.rb_right));
	}

	std::cout <<  "\n";
}

int main()
{
	for(size_t j  = 1; j <= 10; ++j)
	{
		struct test_node* nodes = new struct test_node[j];
		struct rb_root root = { NULL };

		for (size_t i = 0; i < j; i++)
			nodes[i].val = nodes[i].key = i + 1;
	
		for (size_t i = 0; i < j; i++)
			insert_augmented(nodes + i, &root);

		bfs(&root);

		delete [] nodes;
	}
}