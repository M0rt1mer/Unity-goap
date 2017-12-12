using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGHands : GoapSensor {

    public string leftHandItem;
    public string rightHandItem;

    public override void UpdateSensor() {
        GetMemory().GetWorldState().Set( WorldStates.STATE_HAND_LEFT, leftHandItem );
        GetMemory().GetWorldState().Set( WorldStates.STATE_HAND_RIGHT, rightHandItem );
    }

}
