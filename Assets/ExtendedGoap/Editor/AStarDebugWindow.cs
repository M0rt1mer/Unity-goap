using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Text;
using SD.Tools.Algorithmia.GeneralDataStructures;

public class AStarDebugWindow : EditorWindow {

    private Vector2 offset;
    private Vector2 drag;
    private Vector2 totalDrag;

    static GUIStyle popupArea;
    static GUIStyle action;
    static GUIStyle worldState;

    public void OnEnable() {
        popupArea = new GUIStyle();
        popupArea.normal.background = EditorGUIUtility.FindTexture( "OL box" );

        //popupArea = GUI.skin.box;

        action = new GUIStyle();
        action.normal.background = EditorGUIUtility.FindTexture( "OL box" );

        //action = GUI.skin.box;

        worldState = new GUIStyle();
    }

    [MenuItem( "Window/AStar debugger" )]
    static void Init() {
        EditorWindow.GetWindow<AStarDebugWindow>().Show();
        failedCondition.normal.textColor = Color.red;
    }

    void OnGUI() {

        DrawGrid( 20, 0.2f, Color.gray );
        DrawGrid( 100, 0.4f, Color.gray );

        //UpdateGoapNodes( Selection.activeGameObject );
        DrawNodes();

        //ProcessNodeEvents( Event.current );
        ProcessEvents( Event.current );

        Repaint();
    }


    private void DrawNodes() {

        if(AStar<ReGoapState>.lastSearchExplored != null) {

            MultiValueDictionary<ReGoapNode, ReGoapNode> childNodes = new MultiValueDictionary<ReGoapNode, ReGoapNode>();
            ReGoapNode root = null;

            foreach(INode<ReGoapState> inode in AStar<ReGoapState>.lastSearchExplored.Values.Concat( AStar<ReGoapState>.lastSearchFrontier) ){
                if(inode.GetParent() != null)
                    childNodes.Add( inode.GetParent() as ReGoapNode, inode as ReGoapNode );
                else {
                    root = inode as ReGoapNode;
                }
            }

            DrawSingleNode( root, 0, new int[100], childNodes );

        }

    }

    private static GUIStyle failedCondition = new GUIStyle();
    private static Color failedBackgroundColor = new Color( 0.7f,0.57f,0.57f );

    private Rect DrawSingleNode( ReGoapNode node, int level, int[] offsetInLevel, MultiValueDictionary<ReGoapNode, ReGoapNode> childNodes ) {

        Rect position = new Rect( new Vector2(-300 * level,offsetInLevel[level] + 10) + offset, new Vector2(200,80) );

        GUILayout.BeginArea( position, GUI.skin.box );
        GUILayout.Space( 1 );
        foreach(var value in (node.GetState() as IEnumerable<KeyValuePair<IWorldState, object>>)) {
            GUILayout.Label( value.Key.name + " : " + value.Value );
        }
        GUILayout.EndArea();
        offsetInLevel[level] += 100;

        if( position.Contains( Event.current.mousePosition ) ){
            List<ReGoapActionState> actions = new List<ReGoapActionState>();
            var enumerator = node.GetPossibleActionsEnumerator( true );
            while(enumerator.MoveNext())
                actions.Add( enumerator.Current );

            GUILayout.BeginArea( new Rect( position.position - new Vector2( 310, 10 + actions.Count * 55 ), new Vector2( 320, 1000 ) ) );
            {
                GUILayout.BeginVertical( GUI.skin.box );
                {
                    for(int i = 0; i < actions.Count; i++) {
                        Color previousColor = GUI.backgroundColor;
                        if(actions[i].isValid)
                            GUI.backgroundColor = failedBackgroundColor;

                        GUILayout.Space( 10 );
                        //GUI.Box( new Rect( position.position - new Vector2( 300, actions.Count * 50 - i * 110 + 10 ), new Vector2( 300, 100 ) ), "" );
                        GUI.backgroundColor = previousColor;

                        GUILayout.BeginVertical( GUI.skin.box );
                        {
                            GUILayout.Label( actions[i].Action.ToString() );
                            GUILayout.Label( actions[i].preconditions.ToString(), (actions[i].reason == ReGoapActionState.InvalidReason.CONFLICT) ? failedCondition : GUIStyle.none );
                            GUILayout.Label( actions[i].effects.ToString(), (actions[i].reason == ReGoapActionState.InvalidReason.EFFECTS_DONT_HELP) ? failedCondition : GUIStyle.none );
                            if(actions[i].reason == ReGoapActionState.InvalidReason.PROCEDURAL_CONDITION)
                                GUILayout.Label( "PROCEDURAL CONDITION FAILED", failedCondition );
                        }
                        GUILayout.EndVertical();
                    }
                    GUILayout.Space( 10 );
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndArea();
        }
        foreach(var child in childNodes.GetValues( node, true )) {
            Rect childPos = DrawSingleNode( child, level + 1, offsetInLevel, childNodes );
            Handles.DrawLine( new Vector3( position.xMin, position.y, 0 ), new Vector3( childPos.xMax, childPos.y, 0 ) );
        }
        return position;
    }

    private void DrawGrid( float gridSpacing, float gridOpacity, Color gridColor ) {
        int widthDivs = Mathf.CeilToInt( position.width / gridSpacing );
        int heightDivs = Mathf.CeilToInt( position.height / gridSpacing );

        Handles.BeginGUI();
        Handles.color = new Color( gridColor.r, gridColor.g, gridColor.b, gridOpacity );

        offset += drag * 0.5f;
        Vector3 newOffset = new Vector3( offset.x % gridSpacing, offset.y % gridSpacing, 0 );

        for(int i = 0; i < widthDivs; i++)
            Handles.DrawLine( new Vector3( gridSpacing * i, -gridSpacing, 0 ) + newOffset, new Vector3( gridSpacing * i, position.height, 0f ) + newOffset );

        for(int j = 0; j < heightDivs; j++)
            Handles.DrawLine( new Vector3( -gridSpacing, gridSpacing * j, 0 ) + newOffset, new Vector3( position.width, gridSpacing * j, 0f ) + newOffset );

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    private void ProcessEvents( Event e ) {
        drag = Vector2.zero;

        switch(e.type) {
            case EventType.MouseDrag:
                if(e.button == 0) {
                    OnDrag( e.delta );
                }
                break;
        }
    }


    private void ProcessContextMenu( Vector2 mousePosition ) {
    }

    private void OnDrag( Vector2 delta ) {
        totalDrag += delta;
        drag = delta;

        GUI.changed = true;
    }

}
