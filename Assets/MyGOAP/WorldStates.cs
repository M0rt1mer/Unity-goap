using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WorldStates {

    public static WorldState<Vector3> STATE_POSITION = new WorldState<Vector3>();// = "isAtPosition";
    public static WorldState<float> STATE_FLOAT_HUNGER = new WorldState<float>(true); // "at least X saturation"
    public static WorldState<string> STATE_HAND_LEFT = new WorldState<string>();
    public static WorldState<string> STATE_HAND_RIGHT = new WorldState<string>();

}

public class WorldStateHasItem : WorldState<bool> {
    public readonly DBItem item;
    public WorldStateHasItem( DBItem item ) {
        this.item = item;
        statesList[item] = this;
    }

    public static Dictionary<DBItem, WorldStateHasItem> statesList = new Dictionary<DBItem, WorldStateHasItem>();

    public static WorldStateHasItem GetStateForItem( DBItem item ) {
        if( !statesList.ContainsKey( item ) ) {
            statesList.Add( item, new WorldStateHasItem( item ) );
        }
        return statesList[item];
    }

}

public class WorldStateHasItemCategory : WorldState<bool> {
    public readonly DBItemCategory category;
    public WorldStateHasItemCategory( DBItemCategory category ) {
        this.category = category;
    }

    public static Dictionary<DBItemCategory, WorldStateHasItemCategory> statesList = new Dictionary<DBItemCategory, WorldStateHasItemCategory>();

    public static WorldStateHasItemCategory GetStateForItem( DBItemCategory item ) {
        if(!statesList.ContainsKey( item )) {
            statesList.Add( item, new WorldStateHasItemCategory( item ) );
        }
        return statesList[item];
    }
}

public class WorldState<InnerType> : IWorldState {

    bool isConditionBiggerThan;

    public WorldState( bool isConditionBiggerThan = false ){
        this.isConditionBiggerThan = isConditionBiggerThan;
    }

    public bool IsConditionBiggerThan(){
        return isConditionBiggerThan;
    }
}

public interface IWorldState {
    /// <summary>
    /// If true, the state means "atLeast" it's value. When used in condition, any value equal or bigger than required will pass
    /// </summary>
    /// <returns></returns>
    bool IsConditionBiggerThan();
}