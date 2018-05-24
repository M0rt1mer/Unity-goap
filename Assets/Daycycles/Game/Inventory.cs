using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : GoapSensor {

    public DBItem[] startingItems;

    public List<InGameItem> items;

    void Awake() {
        items = new List<InGameItem>();
        foreach (var itm in startingItems)
            if (itm != null)
            {
                GameObject newItem = GameObject.CreatePrimitive(PrimitiveType.Cube);
                newItem.name = "an item("+itm.name+")";
                newItem.transform.localScale = new Vector3(0.2f,0.2f,0.2f);
                newItem.transform.SetParent(gameObject.transform);
                InGameItem newInGameItem = newItem.AddComponent<InGameItem>();
                newInGameItem.sourceItem = itm;
                items.Add(newInGameItem);
            }
    }

    public override void UpdateSensor() {
        base.UpdateSensor();

        Dictionary<DBItem, int> itemCounts = new Dictionary<DBItem, int>();
        Dictionary<DBItemCategory, int> categoryCounts = new Dictionary<DBItemCategory, int>();

        var worldState = GetMemory().GetWorldState();
        foreach(var item in items) {
            if(!itemCounts.ContainsKey( item.sourceItem ))
                itemCounts.Add( item.sourceItem, 1 );
            else
                itemCounts.Add( item.sourceItem, itemCounts[item.sourceItem] + 1 );

            foreach(var category in item.sourceItem.categories)
                if(!categoryCounts.ContainsKey( category ))
                    categoryCounts.Add( category, 1 );
                else
                    categoryCounts.Add( category, categoryCounts[category] + 1 );
        }

        foreach(var pair in itemCounts)
            worldState.Set( WorldStateMinItem.GetStateForItem( pair.Key ), pair.Value );
        foreach(var pair in categoryCounts)
            worldState.Set( WorldStateMinItemCategory.GetStateForItem( pair.Key ), pair.Value );

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
