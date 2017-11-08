using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Generic class for transitions. Actual factory methods differ per instance
/// </summary>
public abstract class LSMTransitionFactory<Initializer,Transition> : ScriptableObject where Transition : LSMTransition{

    public abstract bool IsInterruptable();

    public abstract Transition MakeTransition( Initializer init );

}

public abstract class LSMTransition {

}
