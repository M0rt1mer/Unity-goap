using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Text;
using SD.Tools.Algorithmia.GeneralDataStructures;
using GraphVisualizer;

public class AStarDebugWindow : EditorWindow {

    private Vector2 offset;
    private Vector2 drag;
    private Vector2 totalDrag;
    private int fontSize;


    static GUIStyle popupArea;
    static GUIStyle action;
    static GUIStyle worldState;

    static GUIStyle styleToolbar;

    public void OnEnable() {
        popupArea = new GUIStyle();
        popupArea.normal.background = EditorGUIUtility.FindTexture( "OL box" );

        //popupArea = GUI.skin.box;

        action = new GUIStyle();
        action.normal.background = EditorGUIUtility.FindTexture( "OL box" );

        //action = GUI.skin.box;

        worldState = new GUIStyle();

        styleToolbar = new GUIStyle();
        styleToolbar.normal.background = (Texture2D)Resources.Load( "Toolbar" );

        offset = drag = totalDrag = Vector2.zero;
        fontSize = 10;
    }

    [MenuItem( "Window/AStar debugger" )]
    static void Init() {
        EditorWindow.GetWindow<AStarDebugWindow>().Show();
        failedCondition.normal.textColor = Color.red;
    }

    void OnGUI() {

        Rect toolbar = new Rect( 0, 0, position.width, 20 );
        Rect canvas = new Rect( 0, 20, position.width, position.height - 20 );


        DrawToolbar( toolbar );
        DrawGrid( canvas, fontSize*2, 0.2f, Color.gray );
        DrawGrid( canvas, fontSize*10, 0.4f, Color.gray );

        //UpdateGoapNodes( Selection.activeGameObject );
        DrawNodesUsingGraphAPI( canvas );

        //ProcessNodeEvents( Event.current );
        ProcessEvents( canvas, Event.current );

        Repaint();
    }

    int selectedAgent = 0;
    int selectedSearch = 0;
    AStarDebugRecording selectedRecording = null;

    void DrawToolbar( Rect toolbarArea ) {

        GUILayout.BeginArea( toolbarArea, styleToolbar );
    
        GUILayout.BeginHorizontal( GUILayout.ExpandWidth( true ) );

        IReGoapAgent[] agentList = AStarDebugRecorder.recordings.Keys.ToArray();
        selectedAgent = EditorGUILayout.Popup( selectedAgent, agentList.Select( x=> x.ToString() ).ToArray(), GUILayout.ExpandWidth(false) );
        if(selectedAgent < agentList.Count()) {
            AStarDebugRecording[] nodes = AStarDebugRecorder.recordings.GetValues( agentList[selectedAgent], false ).ToArray();
            selectedSearch = EditorGUILayout.Popup( selectedSearch, nodes.Select( (node, index) => "search #"+index ).ToArray(), GUILayout.ExpandWidth( false ) );
            selectedRecording = nodes[selectedSearch];
        }

        GUILayout.FlexibleSpace();

        if(GUILayout.Button( "Reset view", GUILayout.Width( 100 ), GUILayout.Height( 18 ) )) {
            offset = Vector2.zero;
            fontSize = 10;
        }

        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }

    private AStarDebugGraph graph;
    private AStarDebugGraphRenderer renderer;
    private IGraphLayout layout;

    private AStarDebugRecording lastDisplayedRecording = null;

    private void DrawNodesUsingGraphAPI( Rect canvas ) {

        if( selectedRecording != null && selectedRecording != lastDisplayedRecording ) {

            MultiValueDictionary<ReGoapNode, ReGoapNode> childNodes = new MultiValueDictionary<ReGoapNode, ReGoapNode>();
            ReGoapNode root = null;

            foreach(INode<ReGoapState> inode in selectedRecording.search) {
                if(inode.GetParent() != null)
                    childNodes.Add( inode.GetParent() as ReGoapNode, inode as ReGoapNode );
                else {
                    root = inode as ReGoapNode;
                }
            }

            graph = new AStarDebugGraph( childNodes, root );
            graph.Refresh();
            if(graph.IsEmpty()) {
                ShowMessage( "No graph data" );
                return;
            }

            if(layout == null)
                layout = new ReingoldTilford(false);

            layout.CalculateLayout( graph );

            if(renderer == null)
                renderer = new AStarDebugGraphRenderer();

            renderer.Draw( layout, canvas, new GraphSettings() { maximumNodeSizeInPixels = 200, maximumNormalizedNodeSize = 1f, aspectRatio = 1.61f }, fontSize, offset );
        }
    }

    private static void ShowMessage( string msg ) {
        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        GUILayout.Label( msg );

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
    }

    private static GUIStyle failedCondition = new GUIStyle();
    private static Color failedBackgroundColor = new Color( 0.7f,0.57f,0.57f );

    private void DrawGrid( Rect area, float gridSpacing, float gridOpacity, Color gridColor ) {
        int widthDivs = Mathf.CeilToInt( area.width / gridSpacing );
        int heightDivs = Mathf.CeilToInt( area.height / gridSpacing );

        Handles.BeginGUI();
        Handles.color = new Color( gridColor.r, gridColor.g, gridColor.b, gridOpacity );

        offset += drag * 0.5f;
        Vector3 newOffset = new Vector3( (offset.x % gridSpacing + gridSpacing) % gridSpacing + area.position.x, (offset.y  % gridSpacing + gridSpacing ) % gridSpacing + area.position.y, 0 );

        for(int i = 0; i < widthDivs; i++)
            Handles.DrawLine( new Vector3( gridSpacing * i + newOffset.x, area.yMin, 0 ), new Vector3( gridSpacing * i + newOffset.x, area.yMax, 0f ) );

        for(int j = 0; j < heightDivs; j++)
            Handles.DrawLine( new Vector3( area.xMin, gridSpacing * j + newOffset.y, 0 ), new Vector3( area.xMax, gridSpacing * j + newOffset.y, 0f ) );

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    private void ProcessEvents( Rect area, Event e ) {
        drag = Vector2.zero;

        switch(e.type) {
            case EventType.MouseDrag:
                if(e.button == 0) {
                    OnDrag( e.delta );
                }
                break;
            case EventType.ScrollWheel:
                if(Event.current.delta.y > 0) {
                    if(fontSize > 2) {
                        fontSize--;
                        offset = offset - (Event.current.mousePosition - offset) * (fontSize/(fontSize+1f) - 1);
                    }
                } else {
                    if(fontSize <15) {
                        fontSize++;
                        offset = offset - (Event.current.mousePosition - offset) * (fontSize / (fontSize - 1f) - 1);
                    }
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
