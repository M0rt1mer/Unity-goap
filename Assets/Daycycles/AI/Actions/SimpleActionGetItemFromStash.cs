using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu( menuName = "GOAP/SimpleActions/Get item from stash" )]
public class SimpleActionGetItemFromStash : SimpleActionGetItemFromStash<SimpleActionGetItemFromStashSettings> { }

public class SimpleActionGetItemFromStash<Settings> : SimpleAction<Settings> where Settings:SimpleActionGetItemFromStashSettings, new() {


    public DBItemCategory tmpSolution_category;

    /// <summary>
    /// Finds the item and stash to pick from
    /// </summary>
    /// <param name="goapAgent"></param>
    /// <param name="goalState"></param>
    /// <returns></returns>
    /// 

    protected override void InitializePreconditionsAndEffects( BGoapState staticEffects, ref BGoapState parametrizedEffects, BGoapState staticPreconditions ) {
        parametrizedEffects.Set( WorldStateMinItemCategory.GetStateForItem( tmpSolution_category ), 1 );
    }

    protected override BGoapState GetPreconditionsFromGoal( BGoapState goalState, Settings settings ) {
        BGoapState returnState = new BGoapState();

        if(FindStash( settings )) {
            StateVarKey<int> key = WorldStateMinItemCategory.GetStateForItem( tmpSolution_category );
            returnState.Set( key, settings.effects.Get( key ) - 1 ); //require one less item than effect (=one item is added)
            returnState.Set( WorldStates.STATE_POSITION, settings.stash.GetEntryPoint() );
        }
        return returnState;
    }

    public bool FindStash( Settings settings ) {

        DBItemCategory category = tmpSolution_category;

        SOStash nearestStash = null;
        float nearestStashDistance = float.MaxValue;
        InGameItem chosenItem = null;

        BGoapState worldState = settings.agent.GetMemory().GetWorldState();

        foreach ( SmartObject SO in settings.agent.GetMemory().GetAvailableSoList() ) {
            var stash = SO as SOStash;
            if (stash != null) {
                float localDistance = Vector3.Distance(stash.GetEntryPoint(), worldState.Get(WorldStates.STATE_POSITION) );
                if (localDistance < nearestStashDistance){
                    //Debug.Log( string.Join( ",", stash.inv.items.Select( x => "(" + string.Join(";",x.sourceItem.categories.Select(y=>y.name).ToArray() ) + ")" ).ToArray() ) );
                    InGameItem localItem = stash.inv.items.FirstOrDefault( item => item.sourceItem.categories.Any( itemCategory => category == itemCategory ) );
                    if (localItem != default(InGameItem)){
                        nearestStash = stash;
                        chosenItem = localItem;
                    }
                }
            }
        }

        if (nearestStash != null & chosenItem != null){
            settings.stash = nearestStash;
            settings.item = chosenItem;
            return true;
        }
        return false;
    }

    protected override IEnumerator<SimpleActionExecutionControlElements> Execute(Settings settings, Action fail)
    {
        if(settings.stash.inv.items.Remove( settings.item )) {
            settings.agent.GetComponent<Inventory>().items.Add( settings.item );
            yield break; //immediate action, must yield at least once
        } 
        else fail(); //desired item not in stash
        
    }

    public override bool CheckProceduralCondition( IReGoapAgent goapAgent, IReGoapActionSettings settings, BGoapState goalState, IReGoapAction nextAction ) {
        //Debug.Log( settings );
        SimpleActionGetItemFromStashSettings sett = settings as SimpleActionGetItemFromStashSettings;
        return sett.effects.Get( WorldStateMinItemCategory.GetStateForItem( tmpSolution_category ) ) > 0 && sett.stash != null;
    }

}

public class SimpleActionGetItemFromStashSettings : SimpleActionSettings {

    public SOStash stash { get; set; }
    public InGameItem item { get; set; }

    public override string ToString() {
        return string.Format( "Eff: {0}\nStash:{1}\nItem:{2}", this.effects, this.stash, this.item );
    }

}
