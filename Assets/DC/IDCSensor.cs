using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDCSensor {

    void UpdateWorld( ReGoapState state, DCAgent agent );

}
