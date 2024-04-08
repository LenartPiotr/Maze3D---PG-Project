using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Moveable
{
    void Start()
    {
        
    }

    public override bool CanBePushed() => false;
    public override bool CanPushOthers() => true;
    public override int GetPushStrength() => 1;

    void Update()
    {
        bool left = Input.GetKeyDown(KeyCode.A);
        bool right = Input.GetKeyDown(KeyCode.D);
        bool up = Input.GetKeyDown(KeyCode.W);
        bool down = Input.GetKeyDown(KeyCode.S);

        if (up) Move(0);
        if (right) Move(1);
        if (down) Move(2);
        if (left) Move(3);

        LevelField field = GetPosition().GetField();
        transform.position = field.transform.position + field.ParentWall.Front;
    }
}
