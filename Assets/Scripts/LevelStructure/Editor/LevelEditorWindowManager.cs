using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

public class LevelEditorWindowManager : EditorWindow
{
    [MenuItem("Window/LevelEditor")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditorWindowManager>("Level Editor");
    }

    int levelWallSize = 5;

    private void OnGUI()
    {
        var redButtonStyle = new GUIStyle(GUI.skin.button);
        redButtonStyle.normal.textColor = Color.red;

        if (GUILayout.Button("Generate empty design cube")) CreateEmptyDesignCube();
        if (GUILayout.Button("Fix positions all cubes")) FixPositionsAllCubes();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Size of single wall: " + levelWallSize);
        levelWallSize = Mathf.RoundToInt(GUILayout.HorizontalSlider(levelWallSize, 3, 10));
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Create level from design cubes", redButtonStyle)) CreateLevelFromDesignCubes();
    }

    void CreateEmptyDesignCube()
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = Vector3.zero;
        cube.name = "Design Cube";
        cube.AddComponent<DesignCube>();
    }

    void FixPositionsAllCubes()
    {
        DesignCube[] cubes = FindObjectsOfType<DesignCube>();
        foreach (DesignCube c in cubes)
        {
            c.FitScaleAndRotation();
            c.FitToGrid();
        }
    }

    void CreateLevelFromDesignCubes()
    {
        if (!EditorUtility.DisplayDialog("Create level from design cubes", "Do you want remove all cubes and create level walls from them? This operation is irreversible!", "Yes", "No"))
            return;

        HashSet<Vector3Int> cubesPositions = new();
        foreach (DesignCube cube in FindObjectsOfType<DesignCube>())
        {
            cubesPositions.Add(cube.FitToGrid());
            DestroyImmediate(cube.gameObject);
        }

        var levelData = EditorCubesAlgorithm.GetWallsFromCubes(cubesPositions.ToArray());

        GameObject generatorObject = new GameObject("Level Root");
        var generator = generatorObject.AddComponent<LevelGenerator>();
        generator.SetLevelData(levelData);
        generator.CreateLevelFromData(levelWallSize);
    }
}
