using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Two functions:
/// a) checked container - indexed by StateVarKey<KEY>, checks if inserted values is of type KEY
/// b) operations on values: union, difference, distance,...
/// TODO: extract a) to superclass "CheckedContainer"
/// </summary>
public class BGoapState : ICloneable, IEnumerable<IStateVarKey>, IEnumerable<KeyValuePair<IStateVarKey, object>> {
    // can change to object
    private volatile Dictionary<IStateVarKey, object> values;

    public BGoapState(BGoapState old)
    {
        lock (old.values)
            values = new Dictionary<IStateVarKey, object>(old.values);
    }

    public BGoapState()
    {
        values = new Dictionary<IStateVarKey, object>();
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
        lock( values) {
            result = new BGoapState(this );
        }
        lock(b.values) {
            foreach(var pair in b.values) {
                if(values.ContainsKey( pair.Key )) { //value is contained in both a and b
                    result.values[pair.Key] = pair.Key.Logic.Add( values[pair.Key], b.values[pair.Key] ); // use this key's logic to combine the two values
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
    /// <param name="doUseDefaults">whether to use key defaults or not</param>
    /// <param name="other"></param>
    /// <returns></returns>
    public BGoapState Difference( BGoapState other, bool doUseDefaults ) {
        BGoapState result = new BGoapState();
        lock(values) lock(other.values) {
                foreach(var key in values.Keys) {
                    if( other.HasKey( key ) ) { //value exists
                        if( !key.Logic.Satisfies( other.values[key], values[key] ) )
                            result.values.Add( key, values[key] );
                    } else if( !(doUseDefaults && key.Logic.Satisfies( key.GetDefaultValue(), values[key] )) )
                            result.values.Add( key, values[key] );
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
    public float Distance( BGoapState from ) {
        float distance = 0;
        foreach (var pair in values) {
            if (from.HasKey(pair.Key)) {
                distance += pair.Key.Distance( from.values[pair.Key], values[pair.Key] );
            } else distance += pair.Key.Distance( pair.Key.GetDefaultValue(), values[pair.Key] );
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
                if(key.Logic.IsConflict( values[key], effects.values[key] ))
                    return true;
            } else if(precond.HasKey( key )) { //only check precond, if this value was not changed 
                if(key.Logic.IsConflict( values[key], precond.values[key] ))
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
            foreach ( var pair in values ){
                    if( goal.HasKey( pair.Key ) )
                        if( pair.Key.Logic.Satisfies( pair.Value, goal.values[pair.Key] ) ) //if variable gets canceled out, it means that it fulfilled goal
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
                result += string.Format("'{0}': {1}, ", pair.Key.Name, pair.Value);
            return result;
        }
    }

    public T Get<T>(AStateVarKey<T> key)
    {
        lock (values)
        {
            if (!values.ContainsKey(key))
                return default(T);
            return (T)values[key];
        }
    }

    public void Set<T>( AStateVarKey<T> key, T value)
    {
        lock (values)
        {
            values[key] = value;
        }
    }

    public void Remove<T>( AStateVarKey<T> key )
    {
        lock (values)
        {
            values.Remove(key);
        }
    }

    public void SetFrom( IStateVarKey state, BGoapState otherState ) {
        values[state] = otherState.values[state];
    }




    //TODO: check for value type
    public void SetFromKeyUntyped(IStateVarKey thisKey, IStateVarKey otherKey, BGoapState otherState ) {
        values[thisKey] = otherState.values[otherKey];

        Type thisKeyType = thisKey.GetType();
        Type otherKeyType = otherKey.GetType();
        if ( !thisKeyType.IsGenericType || !otherKeyType.IsGenericType || thisKeyType.GetGenericArguments()[0]!=otherKeyType.GetGenericArguments()[0] ) {
            throw new ArgumentException("underlying type is different for " + thisKey + " and " + otherKey);
        }
        typeof(BGoapState).GetMethod("SetFromKey").MakeGenericMethod(thisKeyType.GetGenericArguments()[0]).Invoke(this, new object[] { thisKey, otherKey, otherState });
    }

    public void SetFromKey<T>(AStateVarKey<T> thisKey, AStateVarKey<T> otherKey, BGoapState otherState) {
        values[thisKey] = otherState.values[otherKey];
    }





    public bool ValueEqualsUntyped(IStateVarKey thisKey, IStateVarKey otherKey, BGoapState otherState) {
        Type thisKeyType = thisKey.GetType();
        Type otherKeyType = otherKey.GetType();
        if (!thisKeyType.IsGenericType || !otherKeyType.IsGenericType || thisKeyType.GetGenericArguments()[0] != otherKeyType.GetGenericArguments()[0])
        {
            throw new ArgumentException("underlying type is different for " + thisKey + " and " + otherKey);
        }
        return (bool) typeof(BGoapState).GetMethod("ValueEquals").MakeGenericMethod(thisKeyType.GetGenericArguments()[0]).Invoke(this, new object[] { thisKey, otherKey, otherState });
    }

    public bool ValueEquals<T>(AStateVarKey<T> thisKey, AStateVarKey<T> otherKey, BGoapState otherState) {
        return HasKey(thisKey) && otherState.HasKey(otherKey) && values[thisKey] == otherState.values[otherKey];
    }



    public bool IsEmpty() {
        return values.Count == 0;
    }

    public bool HasKey( IStateVarKey key )
    {
        lock (values)
            return values.ContainsKey(key);
    }

    public void Clear()
    {
        values.Clear();
    }

    public IEnumerator<IStateVarKey> GetEnumerator() {
        return values.Keys.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return values.Keys.GetEnumerator();
    }

    IEnumerator<KeyValuePair<IStateVarKey, object>> IEnumerable<KeyValuePair<IStateVarKey, object>>.GetEnumerator() {
        return ((IEnumerable<KeyValuePair<IStateVarKey, object>>)values).GetEnumerator();
    }
};