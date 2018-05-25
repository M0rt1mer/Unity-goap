using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericSensor : GoapSensor {

    public override void UpdateSensor() {

        this.GetMemory().SetAvailableSoList( GameObject.FindObjectsOfType<SmartObject>() );
        this.GetMemory().SetAvailableItemList(GameObject.FindObjectsOfType<InGameItem>());
        BGoapState worldState = GetMemory().GetWorldState();
        worldState.Set( WorldStates.STATE_POSITION, transform.position );
        foreach (RPGSmartObjectSimpleState so in GameObject.FindObjectsOfType<RPGSmartObjectSimpleState>())
            worldState.Set(so.GetIndicatingWorldState(), so.isStateOn );
    }

}
