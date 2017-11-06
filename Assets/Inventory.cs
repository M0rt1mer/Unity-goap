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
            worldState.Set( "hasItem:" + item, true );
            string itemPrefix = item;
            while( ( itemPrefix = StringTrimSuffix( itemPrefix ) ) != null )
                worldState.Set( "hasItem:" + item, true );
        }
    }

    private string StringTrimSuffix( string instring ) {
        int pos = instring.LastIndexOf( '.' );
        if( pos == -1 )
            return null;
        return instring.Substring( 0, pos );
    }
}
