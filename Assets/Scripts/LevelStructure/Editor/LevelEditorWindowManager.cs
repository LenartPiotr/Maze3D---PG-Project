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

    Color[] fieldsColors = new Color[] {
        new(0.2f, 0.8f, 0.2f),
        new(0f, 0.6f, 0.2f),
        new(1f, 0.6f, 0f),
        new(0.8f, 0.2f, 0f)
    };

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

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();

        for (int i = 0; i < 4; i++) fieldsColors[i] = EditorGUILayout.ColorField(fieldsColors[i]);

        GUILayout.EndHorizontal();

        if (GUILayout.Button("Paint")) PaintFields();

        if (GUILayout.Button("Toogle Enter On")) ToogleEnter(true);
        if (GUILayout.Button("Toogle Enter Off")) ToogleEnter(false);
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

    void PaintFields()
    {
        foreach (LevelField field in FindObjectsOfType<LevelField>())
        {
            int colorIndex = (field.CanEnter ? 0 : 2) + (field.X + field.Y) % 2;
            Renderer r = field.GetComponent<Renderer>();
            r.sharedMaterial.color = fieldsColors[colorIndex];
        }
    }

    void ToogleEnter(bool value)
    {
        foreach(var obj in Selection.gameObjects)
        {
            if (obj.TryGetComponent(out LevelField field))
            {
                field.CanEnter = value;
            }
        }
        PaintFields();
    }
}
