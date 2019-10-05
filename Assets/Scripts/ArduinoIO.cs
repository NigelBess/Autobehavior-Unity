using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArduinoIO : IODevice
{
    [SerializeField] private ArduinoInterface arduino;
    public void Connect(string port)
    {
        arduino.SetPort(port);
        arduino.Connect();
    }
    public override int ReadJoystick()
    {
        throw new System.NotImplementedException();
    }
}
