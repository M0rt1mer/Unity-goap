public class ReGoapTestAction : GoapAction
{
    public void Init()
    {
        Awake();
    }

    public void SetEffects(BGoapState effects)
    {
        this.effects = effects;
    }

    public void SetPreconditions(BGoapState preconditions)
    {
        this.preconditions = preconditions;
    }
}