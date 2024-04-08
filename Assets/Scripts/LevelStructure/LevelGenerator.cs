using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    List<WallSideInfo[]> levelData;
    LevelWall[] levelWalls;
    bool[] done;
    int levelWallSize;

    public void SetLevelData(List<WallSideInfo[]> newLevelData)
    {
        levelData = newLevelData;
    }

    public void CreateLevelFromData(int levelWallSize)
    {
        this.levelWallSize = levelWallSize;
        levelWalls = new LevelWall[levelData.Count];
        done = new bool[levelData.Count];
        
        GenerateWall(Vector3.zero, Vector3.forward, Vector3.up, -Vector3.right, 0);

        for (int i = 0; i < levelWalls.Length; i++)
        {
            var wall = levelWalls[i];
            for (int side = 0; side < 4; side++)
            {
                wall.SetSideWall(new LevelWall.SideConnectInfo(levelWalls[levelData[i][side].ID], levelData[i][side].Dir, levelData[i][side].Angle), side);
            }
            wall.BuildPlates(levelWallSize);
        }
    }

    void GenerateWall(Vector3 position, Vector3 front, Vector3 up, Vector3 right, int id)
    {
        if (done[id]) return;
        done[id] = true;

        var wall = new GameObject("Wall " + id);
        wall.transform.parent = transform;
        wall.transform.position = position;
        wall.transform.LookAt(position + front, up);
        
        var levelWall = wall.AddComponent<LevelWall>();
        levelWalls[id] = levelWall;

        levelWall.SetWallVectors(id, front, right, up);
        
        for (int side = 0; side < 4; side++)
        {
            if (levelData[id][side].ID == -1) continue;
            if (done[levelData[id][side].ID]) continue;

            Vector3 newPosition;
            Vector3 newRight;
            Vector3 newFront = Vector3.zero;
            Vector3 newUp = Vector3.zero;

            int angle = levelData[id][side].Angle;
            int direction = levelData[id][side].Dir;
            Vector3 a = Vector3.zero;
            Vector3 b = Vector3.zero;
            
            switch (side)
            {
                case 0: a = up; b = -right; break;
                case 1: a = right; b = up; break;
                case 2: a = -up; b = right; break;
                case 3: a = -right; b = -up; break;
            }
            switch (angle)
            {
                case -1: newFront = a; newUp = front; break;
                case 0: newFront = front; newUp = -a; break;
                case 1: newFront = -a; newUp = -front; break;
            }

            newRight = b;
            newPosition = position + levelWallSize * a - a * Mathf.Abs(angle) * levelWallSize / 2 + front * angle * levelWallSize / 2;

            Vector3 oldUp;
            Vector3 oldRight;
            for (int j = 0; j < direction; j++)
            {
                oldUp = newUp;
                oldRight = newRight;
                newUp = -oldRight;
                newRight = oldUp;
            }
            GenerateWall(newPosition, newFront, newUp, newRight, levelData[id][side].ID);
        }
    }
}
