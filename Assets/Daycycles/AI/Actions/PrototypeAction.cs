using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class PrototypeAction : ExecutableAction<PrototypeActionSettings>
{

    public override void OnEnable()
    {
        base.OnEnable();
        Configure();
    }

    #region configuration
    BGoapState staticEffects = new BGoapState();
    Dictionary<IStateVarKey, IStateVarKey> parametrizedEffects = new Dictionary<IStateVarKey, IStateVarKey>();
    BGoapState staticPreconditions = new BGoapState();
    Dictionary<IStateVarKey, IStateVarKey> parametrizedPreconditions = new Dictionary<IStateVarKey, IStateVarKey>();

    protected void AddStaticEffect<T>(StateVarKey<T> state, T staticValue)
    {
        staticEffects.Set(state, staticValue);
    }
    protected void AddStaticPrecondition<T>(StateVarKey<T> state, T staticValue)
    {
        staticPreconditions.Set(state, staticValue);
    }
    protected void AddParametrizedEffect<T>(StateVarKey<T> key, StateVarParameter<T> variableKey)
    {
        parametrizedEffects.Add(key, variableKey);
    }
    protected void AddParametrizedPrecondition<T>(StateVarKey<T> key, StateVarParameter<T> variableKey)
    {
        parametrizedPreconditions.Add(key, variableKey);
    }
    #endregion

    #region to override

    protected abstract void Configure();

    /// <summary>
    /// Gets a set of lock-in variables, and generates all possible assignments to the remaining variables
    /// </summary>
    /// <param name="lockInValues"></param>
    /// <returns>Each possible assignment. For each returned BGOapState S must hold that lockedInvalues + S must contain ALL variables used in parametrized effects and preconditions</returns>
    protected abstract IEnumerable<BGoapState> GetPossibleEffectSets(BGoapState lockInValues, IReGoapAgent goapAgent);


    protected abstract override IEnumerator<SimpleActionExecutionControlElements> Execute(PrototypeActionSettings settings, Action fail);
    #endregion


    public override bool CheckProceduralCondition(IReGoapAgent goapAgent, IReGoapActionSettings settings, BGoapState goalState, IReGoapAction nextAction = null)
    {
        return true;
    }

    public override float GetCost(BGoapState goalState, IReGoapActionSettings settings, IReGoapAction next = null)
    {
        return 1;
    }

    #region sealed
    public override sealed BGoapState GetEffects(BGoapState goalState, IReGoapActionSettings settings, IReGoapAction next = null)
    {
        return (settings as PrototypeActionSettings).effects;
    }

    public override sealed BGoapState GetPreconditions(BGoapState goalState, IReGoapActionSettings settings, IReGoapAction next = null)
    {
        return (settings as PrototypeActionSettings).preconditions;
    }

    #endregion
    public override IEnumerable<IReGoapActionSettings> MultiPrecalculations(IReGoapAgent goapAgent, BGoapState goalState)
    {
        //this is not a real state, just a container for variables
        BGoapState lockedInValues = new BGoapState();

        BGoapState effects = staticEffects.Clone() as BGoapState;
        List<IStateVarKey> freeEffects = new List<IStateVarKey>();
        //set effects
        foreach (IStateVarKey key in parametrizedEffects.Keys) {
            IStateVarKey variableKey = parametrizedEffects[key];
            if (goalState.HasKey(key)) {
                //set locked in values
                if (lockedInValues.HasKey(variableKey)) {
                    if ( lockedInValues.ValueEqualsUntyped(variableKey, key, goalState) ) { //clash on variable value
                        continue;
                    }
                } else {
                    lockedInValues.SetFromKeyUntyped(variableKey, key, goalState);
                }
                effects.SetFrom( key, goalState);
            }
            else
            {
                freeEffects.Add(key);
            }
        }

        BGoapState preconditions = staticPreconditions.Clone() as BGoapState;

        List<IStateVarKey> freePreconditions = new List<IStateVarKey>();
        foreach (IStateVarKey key in parametrizedPreconditions.Keys) {
            IStateVarKey variableKey = parametrizedPreconditions[key];
            if (lockedInValues.HasKey(variableKey))
            {
                preconditions.SetFromKeyUntyped(key, variableKey, lockedInValues);
            }
            else {
                freePreconditions.Add(key);
            }
        }

        if (freeEffects.Count + freePreconditions.Count > 0) //some varables need to be searched
        {
            foreach (BGoapState assignment in GetPossibleEffectSets(lockedInValues, goapAgent))
            {
                BGoapState finalEffects = effects.Clone() as BGoapState;
                BGoapState finalPreconditions = preconditions.Clone() as BGoapState;
                foreach (IStateVarKey key in freeEffects)
                {
                    IStateVarKey variableKey = parametrizedEffects[key];
                    finalEffects.SetFromKeyUntyped(key, variableKey, assignment);
                }
                foreach (IStateVarKey key in freePreconditions)
                {
                    IStateVarKey variableKey = parametrizedPreconditions[key];
                    finalPreconditions.SetFromKeyUntyped(key, variableKey, assignment);
                }
                yield return new PrototypeActionSettings { effects = finalEffects, preconditions = finalPreconditions, agent = goapAgent };
            }
        }
        else { //all variables locked in
            yield return new PrototypeActionSettings { effects = effects, preconditions = preconditions, agent = goapAgent };
        }

    }

    public override IReGoapActionSettings Precalculations(IReGoapAgent goapAgent, BGoapState goalState)
    {
        throw new NotImplementedException();
    }

}


public class PrototypeActionSettings : ExecutableActionSettings
{

    public BGoapState effects { get; set; }
    public BGoapState preconditions { get; set; }

    public IReGoapAgent agent { get; set; }

}

#region parameter
/// <summary>
/// Special state var, which is used in PrototypeAction to index variables. Has no logic
/// </summary>
/// <typeparam name="ValueType"></typeparam>
public class StateVarParameter<ValueType> : AStateVarKey<ValueType>
{
    public override IStateVariableLogic Logic
    {
        get
        {
            throw new NotImplementedException();
        }

        protected set
        {
            throw new NotImplementedException();
        }
    }

    public override string Name
    {
        get
        {
            throw new NotImplementedException();
        }

        protected set
        {
            throw new NotImplementedException();
        }
    }

    public override float Distance(object a, object b)
    {
        throw new NotImplementedException();
    }

    public override object GetDefaultValue()
    {
        throw new NotImplementedException();
    }
}
#endregion