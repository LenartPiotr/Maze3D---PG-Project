using Palmmedia.ReportGenerator.Core;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class Moveable : MonoBehaviour
{
    protected int rotation = 0;
    public int Rotation => rotation;

    [SerializeField] private LevelWall currentWall;
    [SerializeField] private int x;
    [SerializeField] private int y;
    [Range(0f, 2f)]
    [SerializeField] private float moveY = 0.5f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotSpeed = 800f;

    [SerializeField]
    private UnityEvent onTeleport;

    [SerializeField]
    private UnityEvent onMove;

    public LevelField Field {
        get {
            return currentWall != null ? currentWall[x, y] : null;
        }
    }

    protected Vector3 targetPosition;
    protected Quaternion targetRotation;

    protected virtual void Start()
    {
        targetPosition = GetTargetPosition();
        targetRotation = GetTargetRotation();
        transform.position = targetPosition;
        transform.rotation = targetRotation;
    }

    protected virtual void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.fixedDeltaTime * moveSpeed);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.fixedDeltaTime * rotSpeed);
    }

    protected virtual void Update()
    {
        //
    }

    public bool IsMoving() => Vector3.Distance(transform.position, targetPosition) < 0.01;
    public bool IsRotating() => Quaternion.Angle(transform.rotation, targetRotation) < 0.01;

    public void SetPos(LevelWall targetWall, int x, int y)
    {
        currentWall = targetWall;
        this.x = x;
        this.y = y;
        targetPosition = GetTargetPosition();
        targetRotation = GetTargetRotation();
        transform.position = targetPosition;
        transform.rotation = targetRotation;
        onTeleport.Invoke();
    }

    protected virtual Vector3 GetTargetPosition() => GetPosition().GetField().transform.position + currentWall.Front * moveY;
    protected virtual Quaternion GetTargetRotation() => Quaternion.LookRotation(currentWall.Front, GetVectorFromRotation(currentWall, rotation));

    public void Move(int direction)
    {
        int addRot = 0;
        direction += rotation;
        direction %= 4;
        Position oldPosition = GetPosition();
        Position newPosition = GetNextFieldInDirection(direction, out bool needCross, ref addRot);
        if (!CanBeMovedInDirection(direction, GetPushStrength())) return;
        if (needCross) {
            direction += addRot;
            rotation = addRot;
        }
        else direction += rotation;
        direction %= 4;
        MoveFromTo(oldPosition, newPosition, direction);
        onMove.Invoke();
    }

    protected virtual bool CanBePushed() { return true; }
    protected virtual bool CanPushOthers() { return true; }
    protected virtual int GetPushStrength() { return 1; }

    protected Position GetNextFieldInDirection(int direction, out bool needCross, ref int addRot)
    {
        Position newPosition = GetPosition();
        Position position = GetPosition();
        int wallSize = position.Wall.GetWallSize();
        needCross = false;
        switch (direction)
        {
            case 0: if (position.Y > 0)             newPosition.Y--; else needCross = true; break;
            case 1: if (position.X < wallSize - 1)  newPosition.X++; else needCross = true; break;
            case 2: if (position.Y < wallSize - 1)  newPosition.Y++; else needCross = true; break;
            case 3: if (position.X > 0)             newPosition.X--; else needCross = true; break;
        }
        if (needCross) newPosition = HaveToCross(position, direction, ref addRot);
        return newPosition;
    }

    protected bool CanBeMovedInDirection(int direction, int strength)
    {
        if (strength < -10) return false;
        int addRot = 0;
        Position newPosition = GetNextFieldInDirection(direction, out bool needCross, ref addRot);

        int newDir = (direction + addRot) % 4;

        StaticObject nextStaticObject;
        Moveable nextMoveableObject;

        newPosition.Wall[newPosition.X, newPosition.Y].TryGetStaticObject(out nextStaticObject);
        nextMoveableObject = newPosition.Wall[newPosition.X, newPosition.Y].MoveableObject;

        if (nextStaticObject != null && !nextStaticObject.CanEnter()) return false;
        if (!newPosition.Wall[newPosition.X, newPosition.Y].CanEnter) return false;
        if (nextMoveableObject != null)
        {
            if (!nextMoveableObject.CanBePushed()) return false;
            if (!CanBeMovedInDirection(newDir, strength - 1)) return false;
        }
        
        return true;
    }
    public Position GetPosition()
    {
        return new Position()
        {
            Wall = currentWall,
            X = x,
            Y = y,
        }; 
    }

    Position HaveToCross(Position pos, int dir, ref int addRot)
    {
        int wallSize = pos.Wall.GetWallSize();

        int nX = 0, nY = 0;
        int lastPos = wallSize - 1;

        int A = 0;
        switch (dir)
        {
            case 0: A = pos.X; break;
            case 1: A = pos.Y; break;
            case 2: A = lastPos - pos.X; break;
            case 3: A = lastPos - pos.Y; break;
        }

        switch (pos.Wall.GetSideOutDirection(dir))
        {
            case 0: nX = lastPos - A; nY = 0; break;
            case 1: nX = lastPos; nY = lastPos - A; break;
            case 2: nX = A; nY = lastPos; break;
            case 3: nX = 0; nY = A; break;
        }

        addRot = pos.Wall.GetSideOutDirection(dir) - dir;
        if (addRot < 0) addRot += 4;
        addRot += 2 + rotation;
        addRot %= 4;

        return new Position()
        {
            Wall = pos.Wall.GetWallOnSide(dir),
            X = nX,
            Y = nY
        };
    }
    void MoveFromTo(Position fromPosition, Position targetPosition, int dir)
    {
        Moveable nextMoveableObject;

        fromPosition.Wall[fromPosition.X, fromPosition.Y].TryGetStaticObject(out StaticObject currentStaticObject);
        targetPosition.Wall[targetPosition.X, targetPosition.Y].TryGetStaticObject(out StaticObject nextStaticObject);
        nextMoveableObject = targetPosition.Wall[targetPosition.X, targetPosition.Y].MoveableObject;

        if (nextMoveableObject != null) nextMoveableObject.Move(dir);
        if (currentStaticObject != null) currentStaticObject.Leave(this);
        if (nextStaticObject != null) nextStaticObject.Enter(this);

        fromPosition.GetField().MoveableObject = null;
        targetPosition.GetField().MoveableObject = this;

        currentWall = targetPosition.Wall;
        x = targetPosition.X;
        y = targetPosition.Y;

        this.targetPosition = GetTargetPosition();
        targetRotation = GetTargetRotation();
    }

    protected Vector3 GetVectorFromRotation(LevelWall wall, int rotation)
    {
        return rotation switch
        {
            0 => wall.Up,
            1 => wall.Right,
            2 => -wall.Up,
            3 => -wall.Right,
            _ => wall.Front,
        };
    }

    public class Position
    {
        public LevelWall Wall;
        public int X;
        public int Y;
        public LevelField GetField() => Wall[X, Y];
    }
}
