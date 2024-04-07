using Palmmedia.ReportGenerator.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelWall : MonoBehaviour
{
    public int ID { get; set; }
    public Vector3 Front { get; set; }
    public Vector3 Right { get; set; }
    public Vector3 Up { get; set; }

    private readonly SideConnectInfo[] sideWallReferences = new SideConnectInfo[4];
    private GameObject[,] fields;

    public void SetSideWall(SideConnectInfo wall, int side)
    {
        sideWallReferences[side] = wall;
    }

    public void BuildPlates(int size)
    {
        fields = new GameObject[size, size];
        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
            {
                GameObject field = GameObject.CreatePrimitive(PrimitiveType.Cube);
                fields[x, y] = field;
                field.name = "Field " + x + " " + y;
                field.transform.parent = transform;
                field.transform.localScale = new Vector3(1, 1, 0.1f);
                field.transform.localPosition = (Vector3.up + Vector3.right) * size / 2f + x * Vector3.left + y * Vector3.down + Vector3.left / 2 + Vector3.down / 2;
                field.transform.rotation = transform.rotation;
            }
    }

    public class SideConnectInfo
    {
        public LevelWall Wall { get; set; }
        public int DirOut { get; set; }
        public int Angle { get; set; }
    }
}
