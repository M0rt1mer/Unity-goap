using SD.Tools.Algorithmia.GeneralDataStructures;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AStarDebugRecorder {

    public static MultiValueDictionary<IReGoapAgent, AStarDebugRecording> recordings = new MultiValueDictionary<IReGoapAgent, AStarDebugRecording>();

    public static void AddRecording( AStarDebugRecording recording ) {
        recordings.Add( recording.search.First().planner.GetCurrentAgent(), recording );
    }

}

public class AStarDebugRecording {

    public BGoapNode[] search;

    public AStarDebugRecording( IEnumerable<BGoapNode> search ) {
        this.search = search.ToArray();
    }

}