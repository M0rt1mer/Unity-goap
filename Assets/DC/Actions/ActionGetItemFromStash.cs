using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class ActionGetItemFromStash : ActionExecuteTransition<ActionGetItemFromStashSettings,TransitionFactoryGetItemFromStash,TransitionGetItemFromStash,TransitionGetItemFromStashInitializer>{


    public override IReGoapActionSettings Precalculations(IReGoapAgent goapAgent, ReGoapState goalState) {

        HashSet<string> itemsToFind = new HashSet<string>();
        foreach (var state in goalState.GetValues().Keys) {
            if ( state is WorldStateHasItem )
                itemsToFind.Add( (state as WorldStateHasItem).item );
        }

        ReGoapState worldState = goapAgent.GetMemory().GetWorldState();

        SOStash nearestStash = null;
        float nearestStashDistance = float.MaxValue;
        string chosenItem = null;

        foreach(SmartObject SO in goapAgent.GetMemory().GetAvailableSoList() ) {
            var stash = SO as SOStash;
            if(stash != null) {
                float localDistance = Vector3.Distance( stash.GetEntryPoint(), worldState.Get( WorldStates.STATE_POSITION ) );
                if(localDistance < nearestStashDistance) {
                    string localItem = stash.inv.items.FirstOrDefault( item => Inventory.GetAllItemPrefixes( item ).Any( itemPrefix => itemsToFind.Contains( itemPrefix ) ) ); //find first item, which is in itemsToFind, OR whose any prefix is in itemsToFind
                    if(localItem != default(string) ) {
                        nearestStash = stash;
                        chosenItem = localItem;
                    }
                }
            }
        }

        if( nearestStash != null & chosenItem != null ) {
            preconditions.Clear();
            preconditions.Set(WorldStates.STATE_POSITION, nearestStash.GetEntryPoint() );

            return new ActionGetItemFromStashSettings() {
                Stash = nearestStash,
                Item = chosenItem
            };
        }
        return null;
    }

}

public class ActionGetItemFromStashSettings : TransitionGetItemFromStashInitializer, IReGoapActionSettings {

    public SOStash Stash { get; set; }
    public string Item { get; set; }

}