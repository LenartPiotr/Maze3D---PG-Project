using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : Moveable
{
    protected override bool CanBePushed() => true;
    protected override bool CanPushOthers() => false;
}
