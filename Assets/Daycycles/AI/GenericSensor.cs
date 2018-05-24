using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericSensor : GoapSensor {

    public override void UpdateSensor() {

        this.GetMemory().SetAvailableSoList( GameObject.FindObjectsOfType<SmartObject>() );
        this.GetMemory().SetAvailableItemList(GameObject.FindObjectsOfType<InGameItem>());
        this.GetMemory().GetWorldState().Set( WorldStates.STATE_POSITION, transform.position );
    }

}
