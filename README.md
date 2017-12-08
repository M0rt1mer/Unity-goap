# Unity-goap
An implementation of GOAP for unity. Uses backwards search, to allow highly configurable actions. WIP, far from usable right now.


StateVariable - a tuple (R,L) where R - set of possible values, L - logic (a set of operators)
State - a map of StateVariable → Value, where Value ∈ StateVariable.R
Action - tuple ( p, e, execution code ) where p: State, e ∈ State

## Backwards search
Builds a plan: a sequence of actions, which is later executed in reverse. For every action, we know before and after State. It must hold that:
* after(0) = search goal
* before(x) = after(x+1)
* after(n) is satisfiable by current agent State, if n is last action

An action can be added to sequence at position X, if it is not in conflict with after(X-1) State.
An action *A* is in conflict with state *S*, if for any StateVariable in both *effects(A)* and *S*, the *effects(A)*'s value is in *soft* conflict with *S*'s value OR for any StateVariable in both *preconditions(A)* and *S*, the *preconditions(A)*'s is in *hard* conflict with *S*'s values

For action *A*, *before* state is computed by subtracting *effects(A)* from *after(A)* and adding *preconditions(A)*

Subtraction(*A*,*B*): results in a State, which has all (StateVariable,Value) pairs from *A*, except for those, where *B*'s Value *fulfills* *A*'s value

State variable can have different logic, which determines conflict and addition operators:
* EQUAL: 
  *  if values *a* and *b* are different, they are in both *hard* and *soft* conflict
  *  two values can never by added
  *  if *a* and *b* are equal, *b* *fulfills* *a*
* AT_LEAST:
  *  if value *a* is lower than *b*, it is in *soft* conflict. There is never a *hard* conflict
  *  add by choosing larger of the two values
  *  if *b* ≥ *a*, *b* *fulfills* *a*
* AT_MOST: 
  *  if value *a* is larger than *b*, it is in soft conflict. There is never a *hard* conflict
  *  add by choosing lower of the two values
  *  if *a* ≤ *b*, *b* fulfills *a*
