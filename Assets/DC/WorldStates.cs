using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WorldStates {

    public static WorldState<Vector3> STATE_POSITION = new WorldState<Vector3>();// = "isAtPosition";
    public static WorldState<float> STATE_FLOAT_HUNGER = new WorldState<float>();
    public static WorldState<string> STATE_HAND_LEFT = new WorldState<string>();
    public static WorldState<string> STATE_HAND_RIGHT = new WorldState<string>();

}

public class WorldStateHasItem : WorldState<bool> {
    public readonly string item;
    public WorldStateHasItem( string item ) {
        this.item = item;
        statesList[item] = this;
    }

    public static Dictionary<string, WorldStateHasItem> statesList = new Dictionary<string, WorldStateHasItem>();

    public static WorldStateHasItem GetStateForItem( string item ) {
        if( !statesList.ContainsKey( item ) ) {
            statesList.Add( item, new WorldStateHasItem( item ) );
        }
        return statesList[item];
    }

}

public class WorldStateHasItemCategory : WorldState<bool> {
    public readonly string category;
    public WorldStateHasItemCategory( string category ) {
        this.category = category;
    }

    public static Dictionary<string, WorldStateHasItemCategory> statesList = new Dictionary<string, WorldStateHasItemCategory>();

    public static WorldStateHasItemCategory GetStateForItem( string item ) {
        if(!statesList.ContainsKey( item )) {
            statesList.Add( item, new WorldStateHasItemCategory( item ) );
        }
        return statesList[item];
    }
}

public class WorldState<InnerType> : IWorldState {}

public interface IWorldState { }