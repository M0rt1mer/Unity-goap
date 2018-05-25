using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NPCController : EditorWindow{

    static GUIStyle activeGoal;

    [MenuItem("Window/GOAP goal controller")]
    static void Init() {
        EditorWindow window = EditorWindow.GetWindow(typeof(NPCController));
        window.Show();

        activeGoal = new GUIStyle();
        activeGoal.normal.textColor = Color.blue;

    }


    IEnumerable<GoapGoal> goalSet;
    public void Update()
    {
        goalSet = GameObject.FindObjectOfType<GoapAgent>().transform.GetComponentsInChildren<GoapGoal>();
    }

    public void OnGUI() {

        if (goalSet == null)
            return;

        GoapGoal newPickedGoal = null;
        foreach (GoapGoal goal in goalSet) {
            GUILayout.BeginHorizontal();

            GUILayout.Label(goal.Name, GUI.skin.label );
            if( GUILayout.Button("Activate"))
            {
                newPickedGoal = goal;
            }
            GUILayout.EndHorizontal();
        }

        if (newPickedGoal != null)
        {
            foreach (GoapGoal goal in goalSet)
            {
                goal.enabled = (goal == newPickedGoal);
            }
            GameObject.FindObjectOfType<GoapAgent>().WarnGoalEnd( newPickedGoal );
        }


    }
}
