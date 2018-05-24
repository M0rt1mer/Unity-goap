using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GraphVisualizer;
using System;
using SD.Tools.Algorithmia.GeneralDataStructures;
using System.Text;
using System.Linq;

public class AStarDebugNode : Node {

    private static Color colorStart = new Color( 0, 0.8f, 0.8f );
    private static Color colorGoal = new Color( 1, 0.8f, 0 );
    private static Color colorNormal = new Color( 0, 0.8f, 0 );
    bool isGoal;

    public AStarDebugNode( object content, BGoapState goal, float weight = 1, bool active = false ) : base( content, weight, active ) {
        isGoal = (content as BGoapNode).IsGoal(goal);
    }

    public override string ToString() {
        BGoapNode node = content as BGoapNode;
        if( node.action != null )
            return node.GetPathCost() + "+" + node.GetHeuristicCost() +  "     " + node.action + "\n\n" 
                + string.Join( "\n", ((content as BGoapNode).GetState() as IEnumerable<KeyValuePair<IStateVarKey, object>>).Select( x=> (x.Key.Name + ":" + x.Value) ).ToArray() );
        return string.Join( "\n", ((content as BGoapNode).GetState() as IEnumerable<KeyValuePair<IStateVarKey, object>>).Select( x => (x.Key.Name + ":" + x.Value) ).ToArray() );
    }

    public override Color GetColor() {
        if((content as BGoapNode).GetParent() == null)
            return colorStart;
        else if( isGoal )
            return colorGoal;
        else
            return colorNormal;
    }

}


public class AStarDebugGraph : Graph {

    private MultiValueDictionary<BGoapNode, BGoapNode> childLists;
    private BGoapNode root;
    private BGoapState goal;

    public AStarDebugGraph( MultiValueDictionary<BGoapNode, BGoapNode> childLists, BGoapNode root, BGoapState goal ) {
        this.childLists = childLists;
        this.root = root;
        this.goal = goal;
    }

    protected override IEnumerable<Node> GetChildren( Node node ) {
        BGoapNode goapNode = (node as AStarDebugNode).content as BGoapNode;
        if( childLists.ContainsKey( goapNode ) )
            foreach( var BGoapNode in childLists.GetValues( goapNode, true) ) {
                yield return new AStarDebugNode( BGoapNode, goal );
            }
    }
    
    protected override void Populate() {
        AddNodeHierarchy( new AStarDebugNode( root, goal ) );
    }

}

public class AStarDebugGraphRenderer : DefaultGraphRenderer {

    private static Color failedBackgroundColor = new Color( 0.7f, 0.57f, 0.57f );
    private GUIStyle failedCondition;

    public AStarDebugGraphRenderer() {
        failedCondition = new GUIStyle() { normal = { textColor = Color.red } };
    }

    protected override void DrawNode( Rect nodeRect, Node node, bool selected ) {
        DrawRect( nodeRect, node.GetColor(), node.ToString(), node.active, selected );
    }

    private Vertex selectedVertex;

    protected override void DrawGraph( IGraphLayout graphLayout, Rect drawingArea, GraphSettings graphSettings, int fontSize, Vector2 offset ) {
        // add border, except on right-hand side where the legend will provide necessary padding
        /*drawingArea = new Rect( drawingArea.x + s_BorderSize,
            drawingArea.y + s_BorderSize,
            drawingArea.width - s_BorderSize * 2,
            drawingArea.height - s_BorderSize * 2 );*/

        var b = new Bounds( Vector3.zero, Vector3.zero );
        foreach(Vertex v in graphLayout.vertices) {
            b.Encapsulate( new Vector3( v.position.x, v.position.y, 0.0f ) );
        }

        // Increase b by maximum node size (since b is measured between node centers)
        b.Expand( new Vector3( graphSettings.maximumNormalizedNodeSize, graphSettings.maximumNormalizedNodeSize, 0 ) );

        //var scale = new Vector2( drawingArea.width / b.size.x, drawingArea.height / b.size.y );
        //var offset = new Vector2( -b.min.x, -b.min.y );

        var scale = new Vector2( fontSize*10, fontSize*10 );
        Vector2 nodeSize = ComputeNodeSize( scale, graphSettings );

        GUI.BeginGroup( drawingArea );

        foreach(var e in graphLayout.edges) {
            Vector2 v0 = ScaleVertex( e.source.position, offset, scale );
            Vector2 v1 = ScaleVertex( e.destination.position, offset, scale );
            Node node = e.source.node;

            if(graphLayout.leftToRight)
                DrawEdge( v1, v0, node.weight );
            else
                DrawEdge( v0, v1, node.weight );
        }

        Event currentEvent = Event.current;

        bool oldSelectionFound = false;
        Vertex newSelectedVertex = null;

        //clear selection
        if (currentEvent.type == EventType.MouseUp && currentEvent.button == 0)
        {
            selectedVertex = null;
        }

        foreach (Vertex v in graphLayout.vertices) {
            Vector2 nodeCenter = ScaleVertex( v.position, offset, scale ) - nodeSize / 2;
            var nodeRect = new Rect( nodeCenter.x, nodeCenter.y, nodeSize.x, nodeSize.y );

            bool clicked = false;
            if(currentEvent.type == EventType.MouseUp && currentEvent.button == 0) {
                Vector2 mousePos = currentEvent.mousePosition;
                if(nodeRect.Contains( mousePos )) {
                    clicked = true;
                    currentEvent.Use();
                }
            }

            bool currentSelection = (selectedVertex != null)
                && v.node.content.Equals( selectedVertex.node.content ); // Make sure to use Equals() and not == to call any overriden comparison operator in the content type.

            DrawNode( nodeRect, v.node, currentSelection || clicked );

            if(currentSelection) {
                // Previous selection still there.
                oldSelectionFound = true;
            } else if(clicked) {
                // Just Selected a new node.
                newSelectedVertex = v;
            }
        }

        if(newSelectedVertex != null) {
            selectedVertex = newSelectedVertex;
        } else if(!oldSelectionFound) {
            selectedVertex = null;
        }

        GUI.EndGroup();

        if(selectedVertex != null) {
            DrawPossibleActions( ScaleVertex( selectedVertex.position, offset, scale ) );
        }

    }

    private void DrawPossibleActions( Vector2 position ) {
        List<ReGoapActionState> actions = new List<ReGoapActionState>();
        var enumerator = (selectedVertex.node.content as BGoapNode).GetPossibleActionsEnumerator( true );
        while(enumerator.MoveNext())
            actions.Add( enumerator.Current );

        GUILayout.BeginArea( new Rect( position - new Vector2( 310, 10 + actions.Count * 55 ), new Vector2( 320, 1000 ) ) );
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
                        GUILayout.Label(PlaceholderIfEmpty(actions[i].preconditions.ToString()), 
                            (!actions[i].isValid && actions[i].reason == ReGoapActionState.InvalidReason.CONFLICT) ? failedCondition : GUIStyle.none );
                        GUILayout.Label(PlaceholderIfEmpty(actions[i].effects.ToString()), 
                            (!actions[i].isValid && actions[i].reason == ReGoapActionState.InvalidReason.EFFECTS_DONT_HELP) ? failedCondition : GUIStyle.none );
                        if(!actions[i].isValid && actions[i].reason == ReGoapActionState.InvalidReason.PROCEDURAL_CONDITION)
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

    private string PlaceholderIfEmpty(string str) {
        if (str.Length == 0)
            return "*****";
        else
            return str;
    }

    protected new Vector2 ScaleVertex( Vector2 v, Vector2 offset, Vector2 scaleFactor ) {
        return new Vector2( v.x * scaleFactor.x, v.y * scaleFactor.y ) + offset;
    }
}