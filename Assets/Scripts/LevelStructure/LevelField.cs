using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelField : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private Moveable moveable;

    [SerializeField, HideInInspector]
    private StaticObject staticObject;

    public Moveable MoveableObject
    {
        get => moveable;
        set => moveable = value;
    }
    public StaticObject StaticObject
    {
        get => staticObject;
        set => staticObject = value;
    }

    [SerializeField] private LevelWall parentWall;
    [SerializeField] private int x;
    [SerializeField] private int y;
    [SerializeField] private bool canEnter = true;

    public LevelWall ParentWall => parentWall;
    public int X => x;
    public int Y => y;
    public bool CanEnter { get => canEnter; set => canEnter = value; }

    public bool TryGetStaticObject(out StaticObject staticObject)
    {
        staticObject = this.staticObject;
        return staticObject != null;
    }

    public void SetData(LevelWall wall, int x, int y)
    {
        parentWall = wall;
        this.x = x;
        this.y = y;
    }
}
