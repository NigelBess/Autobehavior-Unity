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
    public override float EstimatedServoCloseTime()
    {
        return 0;
    }
    public override void CloseServos()
    {
       //pass
    }
    public override void OpenServos()
    {
        //pass
    }
    public override bool ReadIR()
    {
        return Input.GetKey(KeyCode.I);
    }
}
