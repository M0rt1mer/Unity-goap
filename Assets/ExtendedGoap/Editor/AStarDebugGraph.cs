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

    public AStarDebugNode( object content, float weight = 1, bool active = false ) : base( content, weight, active ) {
    }

    public override string ToString() {
        ReGoapNode node = content as ReGoapNode;
        if( node.action != null )
            return node.action + "\n\n" + string.Join( "\n", ((content as ReGoapNode).GetState() as IEnumerable<KeyValuePair<IWorldState, object>>).Select( x=> (x.Key.name + ":" + x.Value) ).ToArray() );
        return string.Join( "\n", ((content as ReGoapNode).GetState() as IEnumerable<KeyValuePair<IWorldState, object>>).Select( x => (x.Key.name + ":" + x.Value) ).ToArray() );
    }

    public override Color GetColor() {
        if((content as ReGoapNode).GetParent() == null)
            return colorStart;
        else if((content as ReGoapNode).IsGoal( null ))
            return colorGoal;
        else
            return colorNormal;
    }

}


public class AStarDebugGraph : Graph {

    private MultiValueDictionary<ReGoapNode, ReGoapNode> childLists;
    private ReGoapNode root;

    public AStarDebugGraph( MultiValueDictionary<ReGoapNode, ReGoapNode> childLists, ReGoapNode root ) {
        this.childLists = childLists;
        this.root = root;
    }

    protected override IEnumerable<Node> GetChildren( Node node ) {
        ReGoapNode goapNode = (node as AStarDebugNode).content as ReGoapNode;
        if( childLists.ContainsKey( goapNode ) )
            foreach( var reGoapNode in childLists.GetValues( goapNode, true) ) {
                yield return new AStarDebugNode( reGoapNode );
            }
    }

    protected override void Populate() {
        AddNodeHierarchy( new AStarDebugNode( root ) );
    }

}

public class AStarDebugGraphRenderer : DefaultGraphRenderer {

    protected override void DrawNode( Rect nodeRect, Node node, bool selected ) {
        DrawRect( nodeRect, node.GetColor(), node.ToString(), node.active, selected );
    }

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
        Node newSelectedNode = null;

        foreach(Vertex v in graphLayout.vertices) {
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

            bool currentSelection = (m_SelectedNode != null)
                && v.node.content.Equals( m_SelectedNode.content ); // Make sure to use Equals() and not == to call any overriden comparison operator in the content type.

            DrawNode( nodeRect, v.node, currentSelection || clicked );

            if(currentSelection) {
                // Previous selection still there.
                oldSelectionFound = true;
            } else if(clicked) {
                // Just Selected a new node.
                newSelectedNode = v.node;
            }
        }

        if(newSelectedNode != null) {
            m_SelectedNode = newSelectedNode;
        } else if(!oldSelectionFound) {
            m_SelectedNode = null;
        }

        GUI.EndGroup();
    }
    protected new Vector2 ScaleVertex( Vector2 v, Vector2 offset, Vector2 scaleFactor ) {
        return new Vector2( v.x * scaleFactor.x, v.y * scaleFactor.y ) + offset;
    }
}