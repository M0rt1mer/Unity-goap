using System.Collections.Generic;
using UnityEngine;

public abstract class WorldStates {

    public static StateVarKey<Vector3> STATE_POSITION = new StateVarKey<Vector3>( "position", default( Vector3 ) );// = "isAtPosition";
    public static StateVarKey<float> STATE_FLOAT_SATURATION = new StateVarKeyComparable<float, StateVariableLogicAtLeast>( "saturation", 0, ( (from,to) => { return 3*((float)to-(float)from); } ) ); // "at least X saturation"
    public static StateVarKey<DBItem> STATE_HAND_LEFT = new StateVarKey<DBItem>( "hand_left", default( DBItem ) );
    public static StateVarKey<DBItem> STATE_HAND_RIGHT = new StateVarKey<DBItem>( "hand_right", default( DBItem ) );

    public static StateVarKey<InGameItem> STATE_HAND_LEFT_ITEM = new StateVarKey<InGameItem>("hand_left_item", null );
    public static StateVarKey<InGameItem> STATE_HAND_RIGHT_ITEM = new StateVarKey<InGameItem>("hand_right_item", null );

}

public class WorldStateMinItem : StateVarKeyComparable<int, StateVariableLogicAtLeast> {
    public readonly DBItem item;
    public WorldStateMinItem( DBItem item ) : base( "hasItem:" + item.name, 0, (from,to) => { return (int)to - (int)from; } ) {
        this.item = item;
        statesList[item] = this;
    }

    public static Dictionary<DBItem, WorldStateMinItem> statesList = new Dictionary<DBItem, WorldStateMinItem>();

    public static WorldStateMinItem GetStateForItem( DBItem item ) {
        if(!statesList.ContainsKey( item )) {
            statesList.Add( item, new WorldStateMinItem( item ) );
        }
        return statesList[item];
    }

}

public class WorldStateMinItemCategory : StateVarKeyComparable<int, StateVariableLogicAtLeast> {
    public readonly DBItemCategory category;
    public WorldStateMinItemCategory( DBItemCategory category ) : base( "hasItemCat:" + category.name, 0, ( from, to ) => { return (int)to - (int)from; } ) {
        this.category = category;
    }

    public static Dictionary<DBItemCategory, WorldStateMinItemCategory> statesList = new Dictionary<DBItemCategory, WorldStateMinItemCategory>();

    public static WorldStateMinItemCategory GetStateForItem( DBItemCategory item ) {
        if(!statesList.ContainsKey( item )) {
            statesList.Add( item, new WorldStateMinItemCategory( item ) );
        }
        return statesList[item];
    }
}