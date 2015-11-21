using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SnapPiece))]
[CanEditMultipleObjects]
public class SnapPieceEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SnapPiece snapPieceScript = (SnapPiece)target;
        if (GUILayout.Button("Update Connected Local Position"))
        {
            snapPieceScript.UpdateConnectedLocalPosition();
        }
    }
}
