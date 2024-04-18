using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Moveable target;
    [SerializeField] private float distance = 5f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotSpeed = 8f;
    [SerializeField] private float devitationMultiplier = 1f;

    Vector3 vecUp;
    Vector3 targetPosition;
    Quaternion targetRotation;
    
    void Start()
    {
        targetPosition = GetTargetPosition();
        transform.position = targetPosition;
        targetRotation = GetTargetRotation();
        transform.rotation = targetRotation;
    }

    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.fixedDeltaTime * moveSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, GetTargetRotation(), Time.fixedDeltaTime * rotSpeed);
    }

    public void UpdatePosition()
    {
        targetPosition = GetTargetPosition();
        targetRotation = GetTargetRotation();
    }

    public void UpdatePositionInstantly()
    {
        targetPosition = GetTargetPosition();
        transform.position = targetPosition;
        targetRotation = GetTargetRotation();
        transform.rotation = targetRotation;
    }

    Vector3 GetTargetPosition()
    {
        Moveable.Position tPos = target.GetPosition();
        int wallSize = tPos.Wall.Size;

        Vector3 position = tPos.Wall.transform.position + tPos.Wall.Front * distance;
        vecUp = GetVectorFromRotation(tPos.Wall, target.Rotation);
        
        position += -(tPos.Wall.GetConnectAngle((tPos.X >= wallSize / 2f) ? 1 : 3) - 1) * (tPos.X - (wallSize / 2f - 0.5f)) * devitationMultiplier * tPos.Wall.Right;
        position += (tPos.Wall.GetConnectAngle((tPos.Y >= wallSize / 2f) ? 2 : 0) - 1) * (tPos.Y - (wallSize / 2f - 0.5f)) * devitationMultiplier * tPos.Wall.Up;

        return position;
    }
    Quaternion GetTargetRotation() => Quaternion.LookRotation(target.transform.position - transform.position, vecUp);

    Vector3 GetVectorFromRotation(LevelWall wall, int rotation)
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
}
