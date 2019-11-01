using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keyboard : IODevice
{
    public override float ReadJoystick()
    {
        if (Input.GetKey(KeyCode.LeftArrow)) return -1f;
        if (Input.GetKey(KeyCode.RightArrow)) return 1f;
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
    public override void Disconnect()
    {
        //pass
    }
    public override void GiveWater()
    {
        //pass
    }
}
