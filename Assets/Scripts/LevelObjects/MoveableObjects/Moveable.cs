using Palmmedia.ReportGenerator.Core;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
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

    protected void Move(Movement movement)
    {
        Movement nextMovement = GetNextFieldInDirection(movement);
        if (!CanBeMovedInDirection(movement, GetPushStrength())) return;
        rotation = (rotation - movement.Rotation + nextMovement.Rotation + 4) % 4;
        MoveFromTo(movement, nextMovement);
        onMove.Invoke();
    }

    protected virtual bool CanBePushed() { return true; }
    protected virtual bool CanPushOthers() { return true; }
    protected virtual int GetPushStrength() { return 1; }

    protected Movement GetNextFieldInDirection(Movement movement)
    {
        Position newPosition = movement.position;
        Position position = movement.position;
        int wallSize = position.Wall.GetWallSize();
        bool needCross = false;
        switch (movement.Rotation)
        {
            case 0: if (position.Y > 0)             newPosition.Y--; else needCross = true; break;
            case 1: if (position.X < wallSize - 1)  newPosition.X++; else needCross = true; break;
            case 2: if (position.Y < wallSize - 1)  newPosition.Y++; else needCross = true; break;
            case 3: if (position.X > 0)             newPosition.X--; else needCross = true; break;
        }
        if (needCross) return HaveToCross(movement);
        return new Movement(newPosition, movement.Rotation);
    }

    protected bool CanBeMovedInDirection(Movement movement, int strength)
    {
        if (strength <= -10) return false;
        Movement nextMovement = GetNextFieldInDirection(movement);

        if (nextMovement.position.Wall[nextMovement.position.X, nextMovement.position.Y].TryGetStaticObject(out StaticObject nextStaticObject))
        {
            if (!nextStaticObject.CanEnter()) return false;
        }

        Moveable nextMoveableObject;
        nextMoveableObject = nextMovement.position.Wall[nextMovement.position.X, nextMovement.position.Y].MoveableObject;
        
        if (!nextMovement.position.Wall[nextMovement.position.X, nextMovement.position.Y].CanEnter) return false;
        if (nextMoveableObject != null)
        {
            if (!nextMoveableObject.CanBePushed()) return false;
            if (!nextMoveableObject.CanBeMovedInDirection(nextMovement, strength - 1)) return false;
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

    Movement HaveToCross(Movement movement)
    {
        int wallSize = movement.position.Wall.GetWallSize();

        int nX = 0, nY = 0;
        int lastPos = wallSize - 1;
        int x = movement.position.X, y = movement.position.Y;

        int A = 0;
        switch (movement.Rotation)
        {
            case 0: A = x; break;
            case 1: A = y; break;
            case 2: A = lastPos - x; break;
            case 3: A = lastPos - y; break;
        }

        switch (movement.position.Wall.GetSideOutDirection(movement.Rotation))
        {
            case 0: nX = lastPos - A; nY = 0; break;
            case 1: nX = lastPos; nY = lastPos - A; break;
            case 2: nX = A; nY = lastPos; break;
            case 3: nX = 0; nY = A; break;
        }

        return new Movement(new Position()
        {
            Wall = movement.position.Wall.GetWallOnSide(movement.Rotation),
            X = nX,
            Y = nY
        }, movement.position.Wall.GetSideOutDirection(movement.Rotation) + 2);
    }
    void MoveFromTo(Movement movementFrom, Movement movementTo)
    {
        Moveable nextMoveableObject;

        movementFrom.position.Wall[movementFrom.position.X, movementFrom.position.Y].TryGetStaticObject(out StaticObject currentStaticObject);
        movementTo.position.Wall[movementTo.position.X, movementTo.position.Y].TryGetStaticObject(out StaticObject nextStaticObject);
        nextMoveableObject = movementTo.position.Wall[movementTo.position.X, movementTo.position.Y].MoveableObject;

        if (nextMoveableObject != null) nextMoveableObject.Move(movementTo);
        if (currentStaticObject != null) currentStaticObject.Leave(this);
        if (nextStaticObject != null) nextStaticObject.Enter(this);

        movementFrom.position.GetField().MoveableObject = null;
        movementTo.position.GetField().MoveableObject = this;

        currentWall = movementTo.position.Wall;
        x = movementTo.position.X;
        y = movementTo.position.Y;

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
    protected Movement MovementFromCameraDirection(int direction)
    {
        return new Movement(GetPosition(), direction + rotation);
    }

    public struct Position
    {
        public LevelWall Wall;
        public int X;
        public int Y;
        public readonly LevelField GetField() => Wall[X, Y];
    }

    protected struct Movement
    {
        public Position position;
        private int rotation;

        public Movement(Position position, int rotation)
        {
            this.position = position;
            this.rotation = rotation % 4;
        }

        public int Rotation
        {
            readonly get => rotation;
            set => rotation = value % 4;
        }
    }
}
