using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GoapMemory : MonoBehaviour, IReGoapMemory
{
    protected ReGoapState state;
    private IReGoapSensor[] sensors;
    private SmartObject[] availableSoList;

    #region UnityFunctions
    protected virtual void Awake()
    {
        state = new ReGoapState();
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

    protected virtual void Update()
    {
        foreach (var sensor in sensors)
        {
            sensor.UpdateSensor();
        }
    }
    #endregion

    public virtual ReGoapState GetWorldState()
    {
        return state;
    }

    public SmartObject[] GetAvailableSoList() {
        return availableSoList;
    }

    public void SetAvailableSoList( SmartObject[] list ) {
        availableSoList = list;
    }
}
