using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : GoapSensor {

    public List<string> items;

    public override void UpdateSensor() {
        base.UpdateSensor();
        var worldState = GetMemory().GetWorldState();
        foreach(var item in items) {
            worldState.Set( WorldStateHasItem.GetStateForItem(item), true );
            foreach (var itemPrefix in GetAllItemPrefixes(item))
                worldState.Set( WorldStateHasItemCategory.GetStateForItem(itemPrefix) , true);
        }
    }

    public static string StringTrimSuffix( string instring ) {
        int pos = instring.LastIndexOf( '.' );
        if( pos == -1 )
            return null;
        return instring.Substring( 0, pos );
    }

    public static IEnumerable<string> GetAllItemPrefixes(string item) {
        string itemPrefix = item;
        while ((itemPrefix = StringTrimSuffix(itemPrefix)) != null)
            yield return itemPrefix;
    }

}
