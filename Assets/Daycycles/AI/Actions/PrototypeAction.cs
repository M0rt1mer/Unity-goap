using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Prototype action allows user (whoever inherits this class) to write an action with highly parametrizable preconds/effect.
/// Action is defined by a set of variables, a set of preconditions (which can use these variables either in generic argument or value)
/// </summary>
public abstract class PrototypeAction : ExecutableAction<PrototypeActionSettings>
{

    public override void OnEnable()
    {
        base.OnEnable();
        Configure();
    }

    #region configuration
    /*BGoapState staticEffects = new BGoapState();
    Dictionary<IStateVarKey, IStateVarKey> parametrizedEffects = new Dictionary<IStateVarKey, IStateVarKey>();
    BGoapState staticPreconditions = new BGoapState();
    Dictionary<IStateVarKey, IStateVarKey> parametrizedPreconditions = new Dictionary<IStateVarKey, IStateVarKey>();
    Dictionary<object, IStateVarKey> genericParameters = new Dictionary<object, IStateVarKey>();*/

    private List<GoapStateEntry> preconditions = new List<GoapStateEntry>();
    private List<GoapStateEntry> effects = new List<GoapStateEntry>();

    private List<IStateVarKey> variables = new List<IStateVarKey>();

    protected void AddStaticEffect<T>(AStateVarKey<T> key, T staticValue)
    {
        effects.Add(new GoapStateEntry<T>(key, staticValue));
    }
    protected void AddStaticGenericEffect<T, G>(GenericStateVarKeyTemplate<T,G> key, AStateVarKey<G> variable, T staticValue)
    {
        effects.Add(new GoapStateGenericEntry<T,G>(key, staticValue, variable));
        variables.Add(variable);
    }
    protected void AddParametrizedEffect<T>(AStateVarKey<T> key, AStateVarKey<T> variable) {
        effects.Add(new GoapStateParametrizedEntry<T>(key, variable));
        variables.Add(variable);
    }
    protected void AddParametrizedGenericEffect<T,G>(GenericStateVarKeyTemplate<T,G> key, AStateVarKey<T> variableValue, AStateVarKey<G> variableGeneric)
    {
        effects.Add(new GoapStateGenericParametrizedEntry<T,G>(key, variableGeneric, variableValue) );
        variables.Add(variableGeneric);
        variables.Add(variableValue);
    }
    protected void AddStaticPrecondition<T>(AStateVarKey<T> key, T staticValue)
    {
        preconditions.Add(new GoapStateEntry<T>(key, staticValue));
    }
    protected void AddStaticGenericPrecondition<T, G>(GenericStateVarKeyTemplate<T, G> key, AStateVarKey<G> variable, T staticValue)
    {
        preconditions.Add(new GoapStateGenericEntry<T, G>(key, staticValue, variable));
        variables.Add(variable);
    }
    protected void AddParametrizedPrecondition<T>(AStateVarKey<T> key, AStateVarKey<T> variable)
    {
        preconditions.Add(new GoapStateParametrizedEntry<T>(key, variable));
        variables.Add(variable);
    }
    protected void AddParametrizedGenericPrecondition<T, G>(GenericStateVarKeyTemplate<T, G> key, AStateVarKey<T> variableValue, AStateVarKey<G> variableGeneric)
    {
        preconditions.Add(new GoapStateGenericParametrizedEntry<T, G>(key, variableGeneric, variableValue));
        variables.Add(variableValue);
        variables.Add(variableGeneric);
    }
    #endregion

    #region to override

    protected abstract void Configure();

    /// <summary>
    /// Gets a set of lock-in variables, and generates all possible assignments to the remaining variables
    /// </summary>
    /// <param name="lockInValues"></param>
    /// <returns>Each possible assignment. For each returned BGOapState S must hold that lockedInvalues + S must contain ALL variables used in parametrized effects and preconditions</returns>
    protected virtual IEnumerable<BGoapState> GetPossibleVariableCombinations(BGoapState lockInValues, IReGoapAgent goapAgent, BGoapState genericAssignments) {
        yield return new BGoapState();
    }

    /// <summary>
    /// Returns all possible combinations of generic variables. By default returns a single empty state (for when there are none)
    /// </summary>
    /// <param name="goapAgent"></param>
    /// <returns></returns>
    protected virtual IEnumerable<BGoapState> GetPossibleGenericVariableCombinations(IReGoapAgent goapAgent) {
        yield return new BGoapState();
    }

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

    /*
    private IEnumerable<IReGoapActionSettings> GenerateAllPossibleActions(IReGoapAgent goapAgent, BGoapState goalState, BGoapState genericsAssignement) {

        //this is not a real state, just a container for variables
        BGoapState lockedInValues = new BGoapState();

        BGoapState effects = staticEffects.Clone() as BGoapState;
        List<IStateVarKey> freeEffects = new List<IStateVarKey>();
        //set effects
        foreach (IStateVarKey iteratorKey in parametrizedEffects.Keys)
        {
            IStateVarKey key;// if key is a template, use it's appropriate child instead
            if (iteratorKey is IGenericStateVarKeyTemplate)                 //appropriate child is such, that has generic argument in accordance with genericsAssignement
                key = (iteratorKey as IGenericStateVarKeyTemplate).MakeGenericInstance(genericsAssignement.Get(genericParameters[iteratorKey]));
            else
                key = iteratorKey;

            IStateVarKey variableKey = parametrizedEffects[iteratorKey];
            if (goalState.HasKey(key))
            {
                //set locked in values
                if (lockedInValues.HasKey(variableKey))
                {
                    if (lockedInValues.ValueEqualsUntyped(variableKey, key, goalState))
                    { //clash on variable value
                        continue;
                    }
                }
                else
                {
                    lockedInValues.SetFromKeyUntyped(variableKey, key, goalState);
                }
                effects.SetFrom(key, goalState);
            }
            else
            {
                freeEffects.Add(key);
            }
        }

        BGoapState preconditions = staticPreconditions.Clone() as BGoapState;

        List<IStateVarKey> freePreconditions = new List<IStateVarKey>();
        foreach (IStateVarKey iteratorKey in parametrizedPreconditions.Keys)
        {
            IStateVarKey key;// if key is a template, use it's appropriate child instead
            if (iteratorKey is IGenericStateVarKeyTemplate)                 //appropriate child is such, that has generic argument in accordance with genericsAssignement
                key = (iteratorKey as IGenericStateVarKeyTemplate).MakeGenericInstance(genericsAssignement.Get(genericParameters[iteratorKey]));
            else
                key = iteratorKey;

            IStateVarKey variableKey = parametrizedPreconditions[iteratorKey];
            if (lockedInValues.HasKey(variableKey))
            {
                preconditions.SetFromKeyUntyped(key, variableKey, lockedInValues);
            }
            else
            {
                freePreconditions.Add(key);
            }
        }

        if (freeEffects.Count + freePreconditions.Count > 0) //some variables need to be searched
        {
            foreach (BGoapState assignment in GetPossibleVariableCombinations(lockedInValues, goapAgent))
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
        else
        { //all variables locked in
            yield return new PrototypeActionSettings { effects = effects, preconditions = preconditions, agent = goapAgent };
        }

    }
    */

    public override IEnumerable<IReGoapActionSettings> MultiPrecalculations(IReGoapAgent goapAgent, BGoapState goalState)
    {
        foreach (BGoapState genericAssignments in GetPossibleGenericVariableCombinations(goapAgent))
        {
            /// generate all possible generic assignemnts
            GoapStateEntry[] processedEffects = effects.Select( x => x.GenNongenericVersion(genericAssignments) ).ToArray();
            GoapStateEntry[] processedPreconditions = preconditions.Select(x => x.GenNongenericVersion(genericAssignments)).ToArray();

            //lock in values that are in goal 
            BGoapState lockedInValues = new BGoapState();
            foreach (GoapStateEntry effect in processedEffects)
                effect.LockInValues(lockedInValues, goalState);

            //generate all actions available for given locked values
            foreach (BGoapState assignments in GetPossibleVariableCombinations( lockedInValues, goapAgent, genericAssignments ))
            {
                BGoapState combined = new BGoapState(assignments).Union(lockedInValues); //union should work here, as lockedInValues and assignemets should be mutually exclusive

                BGoapState generatedPreconditions = new BGoapState();
                BGoapState generatedEffects = new BGoapState();
                foreach (GoapStateEntry entry in processedPreconditions)
                    entry.AddSelfToState(generatedPreconditions, combined);
                foreach (GoapStateEntry entry in processedEffects)
                    entry.AddSelfToState(generatedEffects, combined);
                yield return new PrototypeActionSettings { effects = generatedEffects, preconditions = generatedPreconditions,
                                                            genericAssignments = genericAssignments, agent = goapAgent };
            }

        }
    }

    public override IReGoapActionSettings Precalculations(IReGoapAgent goapAgent, BGoapState goalState)
    {
        throw new NotImplementedException();
    }

    #region entries

    private interface GoapStateEntry {
        void AddSelfToState(BGoapState state, BGoapState variableAssignments);
        GoapStateEntry GenNongenericVersion( BGoapState variableAssignments);
        /// <summary>
        /// Not finished for !!!GenericParametrized!!!
        /// </summary>
        /// <param name="lockInValues"></param>
        /// <param name="goal"></param>
        /// <returns></returns>
        bool LockInValues(BGoapState lockInValues, BGoapState goal);
    }

    private class GoapStateEntry<T> : GoapStateEntry
    {

        protected AStateVarKey<T> key;
        protected T value;

        public GoapStateEntry(AStateVarKey<T> key, T value)
        {
            this.key = key;
            this.value = value;
        }

        public virtual void AddSelfToState(BGoapState state, BGoapState variableAssignments)
        {
            state.Set(key, value);
        }

        public virtual GoapStateEntry GenNongenericVersion(BGoapState variableAssignments)
        {
            return this;
        }

        public virtual bool LockInValues(BGoapState lockInValues, BGoapState goal)
        {
            return true; //we are fixed, nothing to lock in
        }
    }

    private class GoapStateParametrizedEntry<T> : GoapStateEntry<T>
    {
        protected AStateVarKey<T> variableAsParameter;

        public GoapStateParametrizedEntry(AStateVarKey<T> key, AStateVarKey<T> variableAsParameter) : base(key, default(T) )
        {
            this.variableAsParameter = variableAsParameter;
        }

        public override void AddSelfToState(BGoapState state, BGoapState variableAssignments)
        {
            state.Set(key, variableAssignments.Get(variableAsParameter) );
        }

        public override GoapStateEntry GenNongenericVersion(BGoapState variableAssignments)
        {
            return this;
        }

        public override bool LockInValues(BGoapState lockedInValues, BGoapState goalState)
        {
            if ( goalState.HasKey(key) )
            {
                //set locked in values
                if (lockedInValues.HasKey(variableAsParameter))
                {
                    if (lockedInValues.ValueEquals(variableAsParameter, key, goalState))
                    { //clash on variable value
                        return false;
                    }
                }
                else
                {
                    lockedInValues.SetFromKey(variableAsParameter, key, goalState);
                }
            }
            return true; //we are fixed, nothing to lock in
        }
    }

    private class GoapStateGenericEntry<T,Generic> : GoapStateEntry<T> {

        protected AStateVarKey<Generic> variableAsGeneric;


        public GoapStateGenericEntry(GenericStateVarKeyTemplate<T,Generic> key, T value, AStateVarKey<Generic> variableAsGeneric) : base(key, value)
        {
            this.variableAsGeneric = variableAsGeneric;
        }

        public override void AddSelfToState(BGoapState state, BGoapState variableAssignments)
        {

            state.Set( (key as GenericStateVarKeyTemplate<T, Generic>).MakeGenericInstance( variableAssignments.Get(variableAsGeneric) ), value );
        }

        public override GoapStateEntry GenNongenericVersion(BGoapState variableAssignments) {
            return new GoapStateEntry<T>((key as GenericStateVarKeyTemplate<T, Generic>).MakeGenericInstance(variableAssignments.Get(variableAsGeneric)), value);
        }

    }

    private class GoapStateGenericParametrizedEntry<T, Generic> : GoapStateGenericEntry<T, Generic> {

        protected AStateVarKey<T> variableAsParameter;

        public GoapStateGenericParametrizedEntry(GenericStateVarKeyTemplate<T, Generic> key, AStateVarKey<Generic> variableAsGeneric, AStateVarKey<T> variableAsParameter) : base(key, default(T), variableAsGeneric)
        {
            this.variableAsParameter = variableAsParameter;
        }
        public override void AddSelfToState(BGoapState state, BGoapState variableAssignments)
        {

            state.Set((key as GenericStateVarKeyTemplate<T, Generic>).MakeGenericInstance(variableAssignments.Get(variableAsGeneric)), variableAssignments.Get(variableAsParameter));
        }

        public override GoapStateEntry GenNongenericVersion(BGoapState variableAssignments)
        {
            return new GoapStateParametrizedEntry<T>((key as GenericStateVarKeyTemplate<T, Generic>).MakeGenericInstance(variableAssignments.Get(variableAsGeneric)), variableAsParameter);
        }
    }

    #endregion
}

public class PrototypeActionSettings : ExecutableActionSettings
{

    public BGoapState effects { get; set; }
    public BGoapState preconditions { get; set; }
    public BGoapState genericAssignments { get; set; }


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