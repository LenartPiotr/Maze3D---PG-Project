using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Moveable
{
    int selfRotation;

    override protected void Start()
    {
        selfRotation = 0;
        base.Start();
    }

    protected override bool CanBePushed() => false;
    protected override bool CanPushOthers() => true;
    protected override int GetPushStrength() => 1;

    protected override Quaternion GetTargetRotation() => Quaternion.LookRotation(GetPosition().Wall.Front, GetVectorFromRotation(GetPosition().Wall, (rotation + selfRotation) % 4));

    override protected void Update()
    {
        base.Update();

        bool left = Input.GetKeyDown(KeyCode.A);
        bool right = Input.GetKeyDown(KeyCode.D);
        bool up = Input.GetKeyDown(KeyCode.W);
        bool down = Input.GetKeyDown(KeyCode.S);

        if (up)
        {
            selfRotation = 0;
            Move(0);
        }
        if (right)
        {
            selfRotation = 1;
            Move(1);
        }
        if (down)
        {
            selfRotation = 2;
            Move(2);
        }
        if (left)
        {
            selfRotation = 3;
            Move(3);
        }

        // LevelField field = GetPosition().GetField();
        // transform.position = field.transform.position + field.ParentWall.Front;
    }
}
