using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArduinoIO : IODevice
{
    [SerializeField] private ArduinoInterface arduino;
    private const int irPin = 7;
    
    private const int leftServoPin = 10;
    private const int rightServoPin = 9;

    private const int leftServoOpenPos = 180;
    private const int rightServoOpenPos = 90;
    private const int leftServoClosedPos = 90;
    private const int rightServoClosedPos = 180;

    private const float servoCloseTime = 0.3f;

    public void Connect(string port)
    {
        arduino.SetPort(port);
        arduino.Connect();
        arduino.AttachServo(leftServoPin);
        arduino.AttachServo(rightServoPin);
    }
    public override int ReadJoystick()
    {
        throw new System.NotImplementedException();
    }
    public override void CloseServos()
    {
        arduino.WriteServo(leftServoPin,leftServoClosedPos);
        arduino.WriteServo(rightServoPin, rightServoClosedPos);
    }
    public override void OpenServos()
    {
        arduino.WriteServo(leftServoPin, leftServoOpenPos);
        arduino.WriteServo(rightServoPin, rightServoOpenPos);
    }
    public override float EstimatedServoCloseTime()
    {
        return servoCloseTime;
    }
    public override bool ReadIR()
    {
        return arduino.DigitalRead(irPin);
    }
}
