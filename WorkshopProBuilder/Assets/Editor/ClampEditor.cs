using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Clamp))]
public class ClampEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Clamp clampScript = (Clamp)target;
        if (GUILayout.Button("Add Clamp Point"))
        {
            Object clampPointPrefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Tools/Clamp/ClampPoint.prefab", typeof(GameObject));
            GameObject clampPoint = PrefabUtility.InstantiatePrefab(clampPointPrefab) as GameObject;
            clampScript.AddClampPoint(clampPoint);
        }
    }
}
