using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class ActionGetItemFromStash : ActionExecuteTransition<ActionGetItemFromStashSettings,TransitionFactoryGetItemFromStash,TransitionGetItemFromStash,TransitionGetItemFromStashInitializer>{


    public override void Precalculations(IReGoapAgent goapAgent, ReGoapState goalState) {

        HashSet<string> itemsToFind = new HashSet<string>();
        foreach (var state in goalState.GetValues().Keys) {
            if (state.StartsWith("hasItem:"))
                itemsToFind.Add(state.Substring(8));
        }

        ReGoapState worldState = goapAgent.GetMemory().GetWorldState();

        SOStash nearestStash = null;
        float nearestStashDistance = float.MaxValue;
        string chosenItem = null;

        foreach(SmartObject SO in worldState.Get<SmartObject[]>( "soList" )) {
            var stash = SO as SOStash;
            if(stash != null) {
                float localDistance = Vector3.Distance( stash.GetEntryPoint(), worldState.Get<Vector3>( WorldStates.STATE_POSITION ) );
                if(localDistance < nearestStashDistance) {
                    string localItem = stash.inv.items.First( item => Inventory.GetAllItemPrefixes( item ).Any( itemPrefix => itemsToFind.Contains( itemPrefix ) ) ); //find first item, which is in itemsToFind, OR whose any prefix is in itemsToFind
                    if(localItem != null) {
                        nearestStash = stash;
                        chosenItem = localItem;
                    }
                }
            }
        }

        if( nearestStash != null & chosenItem != null ) {
            preconditions.Clear();
            preconditions.Set(WorldStates.STATE_POSITION, nearestStash);

            //effects.Set()

            settings = new ActionGetItemFromStashSettings() {
                Stash = nearestStash,
                Item = chosenItem
            };
        }

    }

}

public class ActionGetItemFromStashSettings : TransitionGetItemFromStashInitializer, IReGoapActionSettings {

    public SOStash Stash { get; set; }
    public string Item { get; set; }

}