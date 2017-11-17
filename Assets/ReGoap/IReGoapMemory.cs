public interface IReGoapMemory
{
    ReGoapState GetWorldState();
    SmartObject[] GetAvailableSoList();
    void SetAvailableSoList( SmartObject[] list );
}