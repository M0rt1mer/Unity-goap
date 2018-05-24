using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GoapMemory : MonoBehaviour, IReGoapMemory
{
    protected BGoapState state;
    private IReGoapSensor[] sensors;
    private SmartObject[] availableSoList;
    private InGameItem[] availableItemList;

    #region UnityFunctions
    protected virtual void Awake()
    {
        state = new BGoapState();
        sensors = GetComponents<IReGoapSensor>();
        foreach (var sensor in sensors)
        {
            sensor.Init(this);
        }
        availableSoList = new SmartObject[0];
    }

    protected virtual void Start()
    {
    }

    protected virtual void FixedUpdate()
    {
    }

    protected virtual void Update(){
        state.Clear();
        foreach (var sensor in sensors)
        {
            sensor.UpdateSensor();
        }
    }
    #endregion

    public virtual BGoapState GetWorldState()
    {
        return state;
    }

    public SmartObject[] GetAvailableSoList() {
        return availableSoList;
    }

    public InGameItem[] GetAvailableItemList() {
        return availableItemList;
    }


    public void SetAvailableSoList( SmartObject[] list ) {
        availableSoList = list;
    }

    public void SetAvailableItemList(InGameItem[] list)
    {
        availableItemList = list;
    }
}
