using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class ExecutableAction<Settings> : AbstractExecutableAction where Settings : ExecutableActionSettings, new()
{

    private string actionName; //Object.name cannot be accessed from other threads

    public virtual void OnEnable()
    {
        actionName = name;
    }

    public override void Exit(IReGoapAction nextAction) { }

    public override string GetName()
    {
        return actionName;
    }

    public override string ToString()
    {
        return actionName;
    }

    public override bool IsInterruptable() { return true; }

    protected abstract IEnumerator<SimpleActionExecutionControlElements> Execute(Settings settings, Action fail);

    public override sealed IEnumerator Run(IReGoapAction previousAction, IReGoapAction nextAction, IReGoapActionSettings settingsParam, BGoapState goalState, Action<IReGoapAction> done, Action<IReGoapAction> fail)
    {
        Settings settings = settingsParam as Settings;
        IEnumerator<SimpleActionExecutionControlElements> progress = Execute(settings, () => { fail(this); });
        while (progress.MoveNext())
        {
            if (settings.interruptNextChanceYouHave && progress.Current != SimpleActionExecutionControlElements.CANNOT_INTERRUPT)
            {
                done(this);
                yield break;
            }
            if (progress.Current == SimpleActionExecutionControlElements.WAIT_NEXT_FRAME)
                yield return new WaitForFixedUpdate();
            yield return progress.Current;
        }
        done(this);
    }

    public override sealed void AskForInterruption(IReGoapActionSettings settings)
    {
        (settings as SimpleActionSettings).interruptNextChanceYouHave = true;
    }

    #region =========================================================================================================================== not implemented
    //not used, therefore not implemented
    public override sealed Dictionary<string, object> GetGenericValues()
    {
        throw new NotImplementedException();
    }

    public override void PostPlanCalculations(IReGoapAgent goapAgent) { }

    // used in oneActionPerActor
    public override bool IsActive()
    {
        throw new NotImplementedException();
    }
    #endregion

}

public enum SimpleActionExecutionControlElements
{
    NORMAL, CANNOT_INTERRUPT, WAIT_NEXT_FRAME
}

public class ExecutableActionSettings : IReGoapActionSettings
{
    public bool interruptNextChanceYouHave { set; get; }

}