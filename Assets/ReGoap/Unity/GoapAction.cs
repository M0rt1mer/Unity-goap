﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class GoapAction : MonoBehaviour, IReGoapAction
{
    public string Name = "GoapAction";

    protected BGoapState preconditions;
    protected BGoapState effects;
    public float Cost = 1;

    protected Action<IReGoapAction> doneCallback;
    protected Action<IReGoapAction> failCallback;
    protected IReGoapAction previousAction;
    protected IReGoapAction nextAction;

    protected IReGoapAgent agent;
    protected Dictionary<string, object> genericValues;
    protected bool interruptWhenPossible;

    #region UnityFunctions
    protected virtual void Awake()
    {
        enabled = false;

        effects = new BGoapState();
        preconditions = new BGoapState();

        genericValues = new Dictionary<string, object>();
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {

    }

    protected virtual void FixedUpdate()
    {
    }
    #endregion

    public virtual bool IsActive()
    {
        return enabled;
    }

    public virtual void PostPlanCalculations(IReGoapAgent goapAgent)
    {
        agent = goapAgent;
    }

    public virtual bool IsInterruptable()
    {
        return true;
    }

    public virtual void AskForInterruption( IReGoapActionSettings settings )
    {
        interruptWhenPossible = true;
    }

    public virtual IReGoapActionSettings Precalculations(IReGoapAgent goapAgent, BGoapState goalState)
    {
        agent = goapAgent;
        return null;
    }

    public IEnumerable<IReGoapActionSettings> MultiPrecalculations(IReGoapAgent goapAgent, BGoapState goalState)
    {
        yield return Precalculations(goapAgent, goalState);
    }

    public virtual bool CheckProceduralCondition(IReGoapAgent goapAgent, BGoapState goalState, IReGoapAction next = null)
    {
        return true;
    }

    public virtual IEnumerator Run(IReGoapAction previous, IReGoapAction next, IReGoapActionSettings settings,
        BGoapState goalState, Action<IReGoapAction> done, Action<IReGoapAction> fail)
    {
        interruptWhenPossible = false;
        enabled = true;
        doneCallback = done;
        failCallback = fail;

        previousAction = previous;
        nextAction = next;
        yield return null;
    }

    public virtual void Exit(IReGoapAction next)
    {
        if (gameObject != null)
            enabled = false;
    }

    public virtual Dictionary<string, object> GetGenericValues()
    {
        return genericValues;
    }

    public virtual string GetName()
    {
        return Name;
    }

    public override string ToString()
    {
        return string.Format("GoapAction('{0}')", Name);
    }

    public virtual BGoapState GetPreconditions(BGoapState goalState, IReGoapActionSettings settings, IReGoapAction next = null)
    {
        return preconditions;
    }

    public virtual BGoapState GetEffects(BGoapState goalState, IReGoapActionSettings settings, IReGoapAction next = null)
    {
        return effects;
    }

    public virtual bool CheckProceduralCondition(IReGoapAgent goapAgent, IReGoapActionSettings settings, BGoapState goalState, IReGoapAction nextAction = null)
    {
        return true;
    }

    public virtual float GetCost(BGoapState goalState, IReGoapActionSettings settings, IReGoapAction next = null)
    {
        return Cost;
    }

}
