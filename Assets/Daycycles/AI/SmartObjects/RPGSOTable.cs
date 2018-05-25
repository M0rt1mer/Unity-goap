public class RPGSOTable : RPGSmartObjectSimpleState
{
    public override AStateVarKey<bool> GetIndicatingWorldState()
    {
        return WorldStates.TABLE_SWEEPED.MakeGenericInstance(this);
    }
}