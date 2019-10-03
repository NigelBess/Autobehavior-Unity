using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keyboard : IODevice
{
    public override int ReadJoystick()
    {
        if (Input.GetKey(KeyCode.LeftArrow)) return -1;
        if (Input.GetKey(KeyCode.RightArrow)) return 1;
        return 0;
    }

}
