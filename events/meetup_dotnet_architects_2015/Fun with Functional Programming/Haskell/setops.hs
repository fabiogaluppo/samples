--Sample provided by Fabio Galuppo 
--February 2015

--Warning: This is only a pedagogical example!

module SetOps (merge, halve, msort, distinct, repeated, union, intersection, relative_complement, symmetric_difference, cartesian_product) where

merge :: Ord a => [a] -> [a] -> [a]
merge [] ys = ys
merge xs [] = xs
merge (x : xs) (y : ys) = 
  if x <= y then 
    x : merge xs (y : ys) 
  else 
    y : merge (x : xs) ys

halve :: [a] -> ([a], [a])
halve xs = splitAt (length xs `div` 2) xs

msort :: Ord a => [a] -> [a]
msort [] = []
msort [x] = [x]
msort xs = merge (msort ys) (msort zs)
  where (ys, zs) = halve xs

--requires [a] ordered
distinct' :: Ord a => [a] -> [a]
distinct' [] = []
distinct' [x] = [x]
distinct' (x : y : xs) = 
  if x == y then
    distinct' (y : xs)
  else 
    x : distinct' (y : xs)

distinct :: Ord a => [a] -> [a]
distinct xs = (distinct' . msort) xs

--requires [a] ordered
repeated' :: Ord a => [a] -> [a]
repeated' [] = []
repeated' [_] = []
repeated' (x : y : xs) = 
  if x == y then
    x : repeated' (y : xs)
 else 
    repeated' (y : xs)

repeated :: Ord a => [a] -> [a]
repeated xs = (distinct . repeated' . msort) xs

--requires [a] ordered
relative_complement' :: Ord a => [a] -> [a] -> [a]
relative_complement' [] _ = []
relative_complement' xs [] = xs
relative_complement' (x : xs) (y : ys) =
  if x < y then 
    x : relative_complement' xs (y : ys)
  else if x > y then  	
  	relative_complement' (x : xs) ys
  else
  	relative_complement' xs ys

--requires [a] ordered
cartesian_product' :: Ord a => Ord b => [a] -> [b] -> [(a,b)]
cartesian_product' _ [] = []
cartesian_product' [] _ = []
cartesian_product' (x : xs) ys = map (\y -> (x, y)) ys ++ cartesian_product' xs ys

--union as concatenation (is wrong, only for think about):
--union :: [a] -> [a] -> [a]
--union [] ys = [] ++ ys
--union (x : xs) ys =  x : union xs ys

--Set basic operations: http://en.wikipedia.org/wiki/Set_(mathematics)#Basic_operations

union :: Ord a => [a] -> [a] -> [a]
union xs ys = distinct (xs ++ ys)

intersection :: Ord a => [a] -> [a] -> [a]
intersection xs ys = repeated (xs ++ ys)

--The relative complement of B in A
relative_complement :: Ord a => [a] -> [a] -> [a]
relative_complement xs ys = relative_complement' xs' ys'
  where xs' = distinct xs
        ys' = distinct ys

symmetric_difference :: Ord a => [a] -> [a] -> [a]
symmetric_difference xs ys = relative_complement xs ys `union` relative_complement ys xs

cartesian_product :: Ord a => Ord b => [a] -> [b] -> [(a,b)]
cartesian_product xs ys = cartesian_product' xs' ys'
  where xs' = distinct xs
        ys' = distinct ys

-----------
--- Test 0:
r0 = msort    [3, 1, 4, 1, 1, 3, 4, 5, 8, 4]
r1 = distinct [0, 1, 1, 1, 3, 3, 4, 4, 4, 4, 4]
r2 = repeated [0, 1, 1, 1, 3, 3, 4, 4, 4, 4, 4]

a  = [1,2,3]
b  = [2,3,4]
c  = union a b
d  = intersection a b
e0 = relative_complement [1,2,3,4] [1,3]
e1 = relative_complement a b
e2 = relative_complement b a
e3 = relative_complement [-3..10] [1,3,5,11]
e4 = relative_complement [-3..0] [1..4]
e5 = relative_complement a a
f0 = symmetric_difference [7..10] [9..12]
f1 = symmetric_difference a b

-----------
--- Test 1:
--Role-Based Security: http://msdn.microsoft.com/en-us/library/shz8h065(v=vs.110).aspx
type Id_tuple = ((String, [String]), (String -> Bool))

permissionSet = ["Manager", "Teller"]

identity :: String -> [String] -> Id_tuple
identity name roles = ((name, roles), \role -> length (roles `intersection` [role]) > 0)

is_in_role :: Id_tuple -> String -> Bool
is_in_role u role = snd u $ role

get_user_name :: Id_tuple -> String
get_user_name u = fst $ fst u

get_roles :: Id_tuple -> [String]
get_roles u = snd $ fst u

has_permission :: Id_tuple -> [String] -> Bool
has_permission userId permissionSet = (length $ filter (\role -> is_in_role userId role) permissionSet) > 0

u = identity "fabiogaluppo" permissionSet

permissionSetA = ["Manager", "UserA"]
permissionSetB = ["Manager", "UserB"]
permissionSetC = ["Manager", "UserC"]
--or:
--permissionSetA = ["UserA"]
--permissionSetB = ["UserB"]
--permissionSetC = ["UserC"]

ps = permissionSetA `union` permissionSetB `union` permissionSetC

