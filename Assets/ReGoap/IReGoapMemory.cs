public interface IReGoapMemory
{
    BGoapState GetWorldState();
    SmartObject[] GetAvailableSoList();
    void SetAvailableSoList( SmartObject[] list );
    void SetAvailableItemList(InGameItem[] list);
}