using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : StaticObject
{
    int buttonsPressed = 0;
    bool openedPernamently = false;

    public void OpenPermanently()
    {
        buttonsPressed++;
        openedPernamently = true;
    }
    public void Open() => buttonsPressed++;
    public void Close() => buttonsPressed--;

    public override bool CanEnter() => buttonsPressed > 0 || openedPernamently;
}
