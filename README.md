# Unity-goap
An implementation of GOAP for unity. Uses backwards search, to allow highly configurable actions. WIP, far from usable right now.


StateVariable - a tuple (R,L) where R - set of possible values, L - logic (a set of operators)
State - a map of StateVariable → Value, where Value ∈ StateVariable.R
Action - tuple ( p, e, execution code ) where p: State, e ∈ State

A State can be considered a *requirement* or *world state*. One can compute a union of two *requirements*. A *world state* can be subtracted from a *requirement*, resulting in a slightly more permissive *requirement*.
* *A* ∪ *B*
  * for each variable *v* in both *A* and *B*, resulting set contains *v(A) + v(B)*
  * for each value in either *A* or *B*, resulting set contains either *v(A)* or *v(B)* respectively
* *A* ∖ *B*
  * for each variable *v* in both *A* and *B*, resulting set contains *v(A)* if *¬(v(A)-v(B))* (if *v(A)-v(B)*, then the resulting set doesn't contain *v* at all)
  * for each variable *v* only in *A*, resulting set contains *v(A)*

logic is a set of operators:
* *a* + *b* - returns a value, which represents a combined *requirement* of the two values. Not all logics support union. This addition is **not commutative**
* satisfies(*a*,*b*) - indicates whether *world state* *a* satisfies *requirement* *b* 
  * satisfies(*a*,*b*) ⇔ (*b* - *a*) In other words, if *a* satisfies *b*, *b - a* is true
* *a* - *b* - returns true, if *world state* *b* satisfies *a*.
* *conflict(a,b)* - is true, if *a + b* cannot be computed

## Backwards search
Builds a plan: a sequence of actions, which is later executed in reverse. For every action, we know before and after State. It must hold that:
* after(0) = search goal
* before(x) = after(x+1)
* after(n) is satisfiable by current agent State, if n is last action

An action *A* can be added to sequence at position X, if:
* for each StateVariable *v* in *before(x-1)*
  * if *v* in *effects(A)*, satisfies( *v(effects(A))*, *v(before(x-1))*
  * else if *v* in *preconditions(A)*, not *conflict( v(before(x-1), v(precondtions(A))*

For action *A*, *before* state is computed by subtracting *effects(A)* from *after(A)* and adding *preconditions(A)*

*before(A) = ( after(A) ∖ effects(A) ) ∪ preconditions(A)*

State variable can have different logic, which determines conflict and addition operators:
* EQUAL: 
  *  *a + b* is *a*, if *a=b*, otherwise cannot be computed
  *  *satisfies(a,b)* - true if *a* is equal to *b*
  * *conflict(a,b)* - true if *a* not equal to *b*
* AT_LEAST:
  * *a + b* = *b*
  * *satisfies(a,b)* = true, if *a >= b*
  * *conflict(a,b)* = true, if *a < b*
* AT_MOST: 
  * *a + b* = *b*
  * *satisfies(a,b)* = true, if *a <= b*
  * *conflict(a,b)* = true, if *a > b*
