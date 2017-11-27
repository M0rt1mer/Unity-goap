using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GraphVisualizer;
using System;
using SD.Tools.Algorithmia.GeneralDataStructures;
using System.Text;
using System.Linq;

public class AStarDebugNode : Node {

    public AStarDebugNode( object content, float weight = 1, bool active = false ) : base( content, weight, active ) {
    }

    public override string ToString() {
        return string.Join( "\n", ((content as ReGoapNode).GetState() as IEnumerable<KeyValuePair<IWorldState, object>>).Select( x=> (x.Key.name + ":" + x.Value) ).ToArray() );
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

}