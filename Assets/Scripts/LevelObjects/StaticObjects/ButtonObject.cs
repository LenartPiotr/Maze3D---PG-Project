using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonObject : StaticObject
{
    [SerializeField] private bool permanent = true;
    [SerializeField] private Gate targetGate;

    public override bool CanEnter() => true;
    public override void Enter(Moveable moveable)
    {
        if (permanent) targetGate.OpenPermanently();
        else targetGate.Open();
    }
    public override void Leave(Moveable moveable) => targetGate.Close();
}
