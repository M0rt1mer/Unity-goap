public class ReGoapActionState {

    public IReGoapAction Action;
    public IReGoapActionSettings Settings;

#if DEBUG
    public bool isValid { set; get; }
    public InvalidReason reason { set; get; }
    public enum InvalidReason { EFFECTS_DONT_HELP, CONFLICT, PROCEDURAL_CONDITION }
    public ReGoapState preconditions { set; get; }
    public ReGoapState effects { set; get; }
#endif


    public ReGoapActionState(IReGoapAction action, IReGoapActionSettings settings)
    {
        Action = action;
        Settings = settings;
#if DEBUG
        isValid = true;
#endif

    }
}