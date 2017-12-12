using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// 
/// </summary>
public class BGoapState : ICloneable, IEnumerable<IStateVarKey<object>>, IEnumerable<KeyValuePair<IStateVarKey<object>, object>> {
    
    private volatile Dictionary<IStateVarKey<object>, object> values;

    public BGoapState(BGoapState old)
    {
        lock (old.values)
            values = new Dictionary<IStateVarKey<object>, object>(old.values);
    }

    public BGoapState()
    {
        values = new Dictionary<IStateVarKey<object>, object>();
    }


    /// <summary>
    /// A ∪ B
    //  for each variable v in both A and B, resulting set contains v( A) + v( B)
    //  for each value in either A or B, resulting set contains either v( A) or v( B) respectively
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public BGoapState Union( BGoapState b ) {
        BGoapState result;
        lock(values) {
            result = new BGoapState( this );
        }
        lock(b.values) {
            foreach(var pair in b.values) {
                if(values.ContainsKey( pair.Key )) { //value is contained in both a and b
                    result.values[pair.Key] = pair.Key.logic.Add( values[pair.Key], b.values[pair.Key] ); // use this key's logic to combine the two values
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
    public BGoapState Difference( BGoapState other ) {
        BGoapState result = new BGoapState();
        lock(values) lock(other.values) {
                foreach(var key in values.Keys) {
                    if (other.HasKey(key)) { //value exists
                        if (!key.logic.Satisfies(other.values[key], values[key]))
                            result.values.Add(key, values[key]);
                    }
                    else if (!key.logic.Satisfies(key.GetDefaultValue(), values[key]))
                        result.values.Add(key, values[key]);
                }
            }
        return result;
    }

    public int Count
    {
        get { return values.Count; }
    }

    /// <summary>
    /// Calculates distance from given world state to this requirement
    /// </summary>
    /// <returns></returns>
    public float Distance( BGoapState from ) {
        float distance = 0;
        foreach (var pair in values) {
            if (from.HasKey(pair.Key)) {
                distance += pair.Key.distance( values[pair.Key], from.values[pair.Key] );
            }
        }
        return distance;
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
            if(effects.HasKey( key )) {
                if(key.logic.IsConflict( values[key], effects.values[key] ))
                    return true;
            } else if(precond.HasKey( key )) { //only check precond, if this value was not changed 
                if(key.logic.IsConflict( values[key], precond.values[key] ))
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
            foreach ( var pair in goal.values ){
                    if( goal.HasKey( pair.Key ) )
                        if( pair.Key.logic.Satisfies( pair.Value, goal.values[pair.Key] ) ) //if variable gets canceled out, it means that it fulfilled goal
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
            if (!values.ContainsKey( (IStateVarKey<object>) key ))
                return default(T);
            return (T)values[(IStateVarKey<object>)key];
        }
    }

    public void Set<T>( IStateVarKey<T> key, T value)
    {
        lock (values)
        {
            values[(IStateVarKey<object>)key] = value;
        }
    }

    public void Remove<T>( IStateVarKey<T> key )
    {
        lock (values)
        {
            values.Remove((IStateVarKey<object>)key);
        }
    }

    public void SetFrom( IStateVarKey<object> state, BGoapState otherState ) {
        values[state] = otherState.values[state];
    }

    public bool IsEmpty() {
        return values.Count == 0;
    }

    public bool HasKey( IStateVarKey<object> key )
    {
        lock (values)
            return values.ContainsKey(key);
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
}