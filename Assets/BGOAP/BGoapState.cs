using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Runtime.CompilerServices;

/// <summary>
/// 
/// </summary>
public class BGoapState : ICloneable, IEnumerable<IStateVarKey<object>>, IEnumerable<KeyValuePair<IStateVarKey<object>, object>> {
    // can change to object
    private volatile Dictionary<VariableKeyWrapper, object> values;

    public BGoapState(BGoapState old)
    {
        lock (old.values)
            values = new Dictionary<VariableKeyWrapper, object>(old.values);
    }

    public BGoapState()
    {
        values = new Dictionary<VariableKeyWrapper, object>();
    }


    /// <summary>
    /// A ∪ B
    //  for each variable v in both A and B, resulting set contains v( A) + v( B)
    //  for each value in either A or B, resulting set contains either v( A) or v( B) respectively
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public BGoapState Union(BGoapState b) {
        BGoapState result;
        lock (values) {
            result = new BGoapState(this);
        }
        lock (b.values) {
            foreach (var pair in b.values) {
                if (values.ContainsKey(pair.Key)) { //value is contained in both a and b
                    result.values[pair.Key] = pair.Key.Add(this, b); // use this key's logic to combine the two values
                } else result.values[pair.Key] = pair.Value; //value was only in b
            }
            return result;
        }
    }

    /// <summary>
    /// Calculates set difference between this state and the other. 
    /// A ∖ B
    ///    for each variable v in both A and B, resulting set contains v( A) if ¬(v( A)-v( B)) (if v( A)-v( B), then the resulting set doesn't contain v at all)
    ///    for each variable v only in A, resulting set contains v( A)
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public BGoapState Difference(BGoapState other) {
        BGoapState result = new BGoapState();
        lock (values) lock (other.values) {
                foreach (var key in values.Keys) {
                    if (other.HasKey(key)) { //value exists
                        if (!key.Satisfies(other, this))
                            result.values.Add(key, values[key]);
                    }
                    else if ( !key.IsSatisfiedByDefault(this) )
                        result.values.Add(key, values[key]);
                }
            }
        return result;
    }

    /// <summary>
    /// Adds two extended states together. returns new extended states. respects WorldStateLogic
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static BGoapState operator +(BGoapState a, BGoapState b)
    {
        BGoapState result;
        lock (a.values)
        {
            result = new BGoapState(a);
        }
        lock (b.values)
        {
            foreach (var pair in b.values) {
                if ( a.values.ContainsKey(pair.Key) ) { //value is contained in both a and b
                    result.values[pair.Key] = pair.Key.Add(a, b); // use this key's logic to combine the two values
                } else result.values[pair.Key] = pair.Value; //value was only in b
            }
            return result;
        }
    }

    public int Count
    {
        get { return values.Count; }
    }

    /// <summary>
    /// Tests for conflict with action that has given preconditions and effects. A conflict arises when effect sets an incompatible value, OR precondition sets an incompatible value and effect doesn't change it.
    /// If there is no conflict, then "this.Difference(effect) + precond" returns a valid state
    /// </summary>
    /// <param name="precond"></param>
    /// <param name="effects"></param>
    /// <returns></returns>
    public bool HasConflict(BGoapState precond, BGoapState effects) {

        //check effects
        foreach (var key in values.Keys) {
            if (effects.HasKey(key)) {
                if (key.IsConflict( this, effects ) )
                    return true;
            } else if (precond.HasKey(key)) { //only check precond, if this value was not changed 
                if (key.IsConflict( this, precond ) )
                    return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Tests if this state has at least one value that fulfills goal's variable
    /// </summary>
    /// <param name="goal"></param>
    /// <returns></returns>
    public bool DoesFullfillGoal(BGoapState goal)
    {
        lock (values) lock (goal.values)
            {
                foreach (var key in values.Keys) {
                    if ( goal.HasKey( key) )
                        if ( key.Satisfies(this, goal)) //if variable gets canceled out, it means that it fulfilled goal
                            return true;
                }
                return false;
            }
    }

    public object Clone()
    {
        var clone = new BGoapState(this);
        return clone;
    }

    public override string ToString()
    {
        lock (values)
        {
            var result = "";
            foreach (var pair in values)
                result += string.Format("'{0}': {1}, ", pair.Key.name, pair.Value);
            return result;
        }
    }

    public T Get<T>(IStateVarKey<T> key) {
        lock (values)
        {
            VariableKeyWrapper wrapper = Wrap(key);
            if (!values.ContainsKey(wrapper))
                return default(T);
            return (T)values[wrapper];
        }
    }

    public void Set<T>(IStateVarKey<T> key, T value)
    {
        lock (values)
        {
            values[Wrap(key)] = value;
        }
    }

    public void Remove<T>(IStateVarKey<T> key)
    {
        lock (values)
        {
            values.Remove(Wrap(key));
        }
    }

    private void SetFrom(VariableKeyWrapper wrapper, BGoapState otherState) {
        values[wrapper] = otherState.values[wrapper];
    }

    public bool IsEmpty() {
        return values.Count == 0;
    }

    public bool HasKey<T>(IStateVarKey<T> key)
    {
        return HasKey(Wrap(key));
    }

    private bool HasKey(VariableKeyWrapper wrapper) {
        lock (values)
            return values.ContainsKey(wrapper);
    }

    public void Clear(){
        values.Clear();
    }

    public IEnumerator<IStateVarKey<object>> GetEnumerator() {
        //return values.Keys.GetEnumerator().Cast<IStateVarKey<object>>();
        return null;
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return values.Keys.GetEnumerator();
    }

    IEnumerator<KeyValuePair<IStateVarKey<object>, object>> IEnumerable<KeyValuePair<IStateVarKey<object>, object>>.GetEnumerator() {
        return ((IEnumerable<KeyValuePair<IStateVarKey<object>, object>>)values).GetEnumerator();
    }

    ///optionally
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private VariableKeyWrapper Wrap<ValueType>( IStateVarKey<ValueType> key ){
        return new VariableKeyWrapper<ValueType>(key);
    }

    private interface VariableKeyWrapper {

        string name { get; }

        bool Satisfies(BGoapState a, BGoapState b);

        bool IsConflict(BGoapState a, BGoapState b);

        object Add(BGoapState a, BGoapState b);

        //TODO: remove this from GoapNode
        bool IsSatisfiedByDefault(BGoapState goal);

    }


    private struct VariableKeyWrapper<ValueType> : VariableKeyWrapper {

        private IStateVarKey<ValueType> key;

        public VariableKeyWrapper(IStateVarKey<ValueType> key){
            this.key = key;
        }

        public string name => key.name;

        public bool Satisfies(BGoapState a, BGoapState b) => key.logic.Satisfies(a.Get(key), b.Get(key));

        public bool IsConflict(BGoapState a, BGoapState b) => key.logic.IsConflict(a.Get(key), b.Get(key));

        public object Add(BGoapState a, BGoapState b) => key.logic.Add(a.Get(key), b.Get(key));

        public bool IsSatisfiedByDefault(BGoapState goal) => key.logic.Satisfies(key.GetDefaultValue(), goal.Get(key));
    }

}