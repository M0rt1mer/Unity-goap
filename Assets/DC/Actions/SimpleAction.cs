using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class SimpleAction : ScriptableObject, IReGoapAction {

    private ReGoapState staticEffects;
    string[] parametrizedEffects;
    private ReGoapState staticPreconditions;

    protected abstract void InitializePreconditionsAndEffects( ref ReGoapState staticEffects, ref string[] parametrizedEffects, ref ReGoapState staticPreconditions );

    protected void Awake(){
        InitializePreconditionsAndEffects( ref staticEffects, ref parametrizedEffects, ref staticPreconditions );
    }

    IReGoapActionSettings IReGoapAction.GetSettings(IReGoapAgent goapAgent, ReGoapState goalState)
    {
        throw new NotImplementedException();
    }

    void IReGoapAction.Run(IReGoapAction previousAction, IReGoapAction nextAction, IReGoapActionSettings settings, ReGoapState goalState, Action<IReGoapAction> done, Action<IReGoapAction> fail)
    {
        throw new NotImplementedException();
    }

    void IReGoapAction.Exit(IReGoapAction nextAction)
    {
        throw new NotImplementedException();
    }

    Dictionary<string, object> IReGoapAction.GetGenericValues()
    {
        throw new NotImplementedException();
    }

    string IReGoapAction.GetName()
    {
        throw new NotImplementedException();
    }

    bool IReGoapAction.IsActive()
    {
        throw new NotImplementedException();
    }

    void IReGoapAction.PostPlanCalculations(IReGoapAgent goapAgent)
    {
        throw new NotImplementedException();
    }

    bool IReGoapAction.IsInterruptable()
    {
        throw new NotImplementedException();
    }

    void IReGoapAction.AskForInterruption()
    {
        throw new NotImplementedException();
    }

    ReGoapState IReGoapAction.GetPreconditions(ReGoapState goalState, IReGoapAction next)
    {
        throw new NotImplementedException();
    }

    ReGoapState IReGoapAction.GetEffects(ReGoapState goalState, IReGoapAction next)
    {
        throw new NotImplementedException();
    }

    bool IReGoapAction.CheckProceduralCondition(IReGoapAgent goapAgent, ReGoapState goalState, IReGoapAction nextAction){
        return true;
    }

    float IReGoapAction.GetCost(ReGoapState goalState, IReGoapAction next)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Not used by this implementation. All precalculations are done in GetPreconditions
    /// </summary>
    /// <param name="goapAgent"></param>
    /// <param name="goalState"></param>
    void IReGoapAction.Precalculations(IReGoapAgent goapAgent, ReGoapState goalState){}
}
