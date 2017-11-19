using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimpleActionGetItemFromStash<Settings> : SimpleAction<Settings> where Settings:SimpleActionGetItemFromStashSettings, new()
{


    /// <summary>
    /// Finds the item and stash to pick from
    /// </summary>
    /// <param name="goapAgent"></param>
    /// <param name="goalState"></param>
    /// <returns></returns>
    public override IReGoapActionSettings Precalculations(IReGoapAgent goapAgent, ReGoapState goalState)
    {
        Settings settings = base.Precalculations(goapAgent, goalState) as Settings;

        HashSet<string> itemsToFind = new HashSet<string>();
        foreach (var state in goalState.GetValues().Keys)
        {
            if (state is WorldStateHasItem)
                itemsToFind.Add((state as WorldStateHasItem).item);
        }

        ReGoapState worldState = goapAgent.GetMemory().GetWorldState();

        SOStash nearestStash = null;
        float nearestStashDistance = float.MaxValue;
        string chosenItem = null;

        foreach (SmartObject SO in goapAgent.GetMemory().GetAvailableSoList())
        {
            var stash = SO as SOStash;
            if (stash != null)
            {
                float localDistance = Vector3.Distance(stash.GetEntryPoint(), worldState.Get(WorldStates.STATE_POSITION));
                if (localDistance < nearestStashDistance)
                {
                    string localItem = stash.inv.items.FirstOrDefault(item => Inventory.GetAllItemPrefixes(item).Any(itemPrefix => itemsToFind.Contains(itemPrefix))); //find first item, which is in itemsToFind, OR whose any prefix is in itemsToFind
                    if (localItem != default(string))
                    {
                        nearestStash = stash;
                        chosenItem = localItem;
                    }
                }
            }
        }

        if (nearestStash != null & chosenItem != null){
            settings.stash = nearestStash;
            settings.item = chosenItem;
        }
        return settings;
    }

    protected override IEnumerator Execute(Settings settings, Action fail)
    {
        if (settings.stash.inv.items.Remove(settings.item)) {
            settings.agent.GetComponent<Inventory>().items.Add(settings.item);
        }
        yield break; //immediate action, must yield at least once
    }

    protected override ReGoapState GetPreconditionsFromGoal(ReGoapState goalState, Settings settings){
        ReGoapState returnState = new ReGoapState();
        returnState.Set( WorldStates.STATE_POSITION, settings.stash.GetEntryPoint() );
        return returnState;
    }

}

[CreateAssetMenu(menuName = "GOAP/SimpleActions/Get item from stash"]
public class SimpleActionGetItemFromStash : SimpleActionGetItemFromStash<SimpleActionGetItemFromStashSettings> { }

public class SimpleActionGetItemFromStashSettings : SimpleActionSettings {

    public SOStash stash { get; set; }
    public string item { get; set; }

}
