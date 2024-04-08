using Palmmedia.ReportGenerator.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelWall : MonoBehaviour
{
    [SerializeField] int id;
    [SerializeField] Vector3 front;
    [SerializeField] Vector3 right;
    [SerializeField] Vector3 up;

    public int ID => id;
    public Vector3 Front => front;
    public Vector3 Right => right;
    public Vector3 Up => up;

    public void SetWallVectors(int id, Vector3 front, Vector3 right, Vector3 up)
    {
        this.id = id;
        this.front = front;
        this.right = right;
        this.up = up;
    }

    [SerializeField]
    private SideConnectInfo[] sideWallReferences;

    [SerializeField]
    private Serializable2DArray<LevelField> fields;

    public LevelField this[int x, int y]
    {
        get
        {
            return fields[x, y];
        }
    }

    public LevelWall GetWallOnSide(int side) => sideWallReferences[side].Wall;
    public int GetSideAngle(int side) => sideWallReferences[side].Angle;
    public int GetSideOutDirection(int side) => sideWallReferences[side].DirOut;
    public int GetWallSize() => fields.Width;

    public void SetSideWall(SideConnectInfo wall, int side)
    {
        sideWallReferences ??= new SideConnectInfo[4];
        sideWallReferences[side] = wall;
    }

    public void BuildPlates(int size)
    {
        fields = new Serializable2DArray<LevelField>(size, size);
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                GameObject fieldObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                LevelField field = fieldObj.AddComponent<LevelField>();
                fields[x, y] = field;
                field.SetData(this, x, y);
                fieldObj.name = "Field " + x + " " + y;
                fieldObj.transform.parent = transform;
                fieldObj.transform.localScale = new Vector3(1, 1, 0.1f);
                fieldObj.transform.localPosition = (Vector3.up + Vector3.right) * size / 2f + x * Vector3.left + y * Vector3.down + Vector3.left / 2 + Vector3.down / 2;
                fieldObj.transform.rotation = transform.rotation;
            }
        }
    }

    [Serializable]
    public class SideConnectInfo
    {
        [SerializeField] private LevelWall wall;
        [SerializeField] private int direction;
        [SerializeField] private int angle;

        public LevelWall Wall => wall;
        public int DirOut => direction;
        public int Angle => angle;

        public SideConnectInfo(LevelWall wall, int dir, int angle)
        {
            this.wall = wall;
            direction = dir;
            this.angle = angle;
        }
    }
}
