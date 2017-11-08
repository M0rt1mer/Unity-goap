using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class TransitionFactoryEatItem : LSMTransitionFactory<ActionSettingsEatFromInventory,TransitionEatItem>
{
    public override bool IsInterruptable() {
        return true;
    }

    public override TransitionEatItem MakeTransition(ActionSettingsEatFromInventory init){
        return new TransitionEatItem();
    }
}

public class TransitionEatItem : LSMTransition {




}