using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGHands : GoapSensor {

    public InGameItem leftHandItem;
    public InGameItem rightHandItem;

    public override void UpdateSensor() {
        if (leftHandItem)
        {
            GetMemory().GetWorldState().Set(WorldStates.STATE_HAND_LEFT_ITEM, leftHandItem);
            GetMemory().GetWorldState().Set(WorldStates.STATE_HAND_LEFT, leftHandItem.sourceItem);
        }
        if (rightHandItem)
        {
            GetMemory().GetWorldState().Set(WorldStates.STATE_HAND_RIGHT, rightHandItem.sourceItem);
            GetMemory().GetWorldState().Set(WorldStates.STATE_HAND_RIGHT_ITEM, rightHandItem);
        }
    }

    public void PickItemRight(InGameItem item) {
        if (this.rightHandItem != null) {
            Debug.LogError("Trying to pick item with full hand");
            return;
        }
        this.rightHandItem = item;
        rightHandItem.transform.SetParent(GameObject.Find("B_R_Hand").transform, false);
    }

    public void DropItemRight() {
        if (rightHandItem == null) {
            Debug.LogError("Trying drop item with empty hands");
            return;
        }
        rightHandItem.transform.SetParent(null, true);
        rightHandItem = null;
    }

}
