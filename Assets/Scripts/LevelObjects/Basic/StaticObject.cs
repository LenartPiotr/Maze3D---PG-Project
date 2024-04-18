using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticObject : MonoBehaviour
{
    public virtual bool CanEnter() { return true; }
    public virtual void Enter(Moveable moveable) { }
    public virtual void Leave(Moveable moveable) { }
}
