using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DesignCube))]
public class DesignCubeInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DesignCube cube = (DesignCube) target;

        if (GUILayout.Button("Fit Position")) cube.FitToGrid();
        if (GUILayout.Button("Fit Rotation & Scale")) cube.FitScaleAndRotation();
    }
}
