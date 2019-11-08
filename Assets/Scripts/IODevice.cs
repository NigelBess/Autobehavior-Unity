using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IODevice : MonoBehaviour
{
    public abstract float ReadJoystick();
    public abstract float EstimatedServoCloseTime();
    public abstract void CloseServos();
    public abstract void OpenServos();
    public abstract bool ReadIR();
    public abstract void Disconnect();
    public abstract void GiveWater();
    public abstract void CloseSolenoid();
}
