using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : GoapSensor {

    public DBItem[] startingItems;

    public List<InGameItem> items;

    void Awake() {
        items = new List<InGameItem>();
        foreach(var itm in startingItems)
            if(itm != null)
                items.Add( new InGameItem( itm ) );
    }

    public override void UpdateSensor() {
        base.UpdateSensor();
        var worldState = GetMemory().GetWorldState();
        foreach(var item in items) {
            worldState.Set( WorldStateHasItem.GetStateForItem(item.sourceItem), true );
            foreach (var category in item.sourceItem.categories )
                worldState.Set( WorldStateHasItemCategory.GetStateForItem( category ) , true);
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
