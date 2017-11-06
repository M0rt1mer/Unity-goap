using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartObject : MonoBehaviour {

    SmartObjectBehaviorTemplate[] behaviors;

    private void Awake() {
        behaviors = GetComponents<SmartObjectBehaviorTemplate>();
    }

    public IEnumerable<SmartObjectBehaviorTemplate> GetTemplates() {
        return behaviors;
    }

}
