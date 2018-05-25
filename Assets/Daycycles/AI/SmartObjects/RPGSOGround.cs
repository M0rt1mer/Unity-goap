
public class RPGSOGround : RPGSmartObjectSimpleState
{
    public override AStateVarKey<bool> GetIndicatingWorldState()
    {
        return WorldStates.GROUND_SWEEPED.MakeGenericInstance(this);
    }
}