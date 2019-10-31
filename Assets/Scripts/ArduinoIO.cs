using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArduinoVars;

public class ArduinoIO : IODevice
{
    [SerializeField] private ArduinoInterface arduino;
    private const int miscInPin = (int)Pin.A3;
    private const int irPin = (int)Pin.D7;
    private const int lickPin = (int)Pin.A2;
    private const float lickMeterVoltageDelta = 1f;
    private float lickMeterExpectedVoltage;

    private const int servoTransistorPin = (int)Pin.D6;
    private const int solenoidTransistorPin = (int)Pin.D8;
    
    private const int leftServoPin = 10;
    private const int rightServoPin = 9;

    private const int leftServoOpenPos = 90;
    private const int rightServoOpenPos = 180;
    private const int leftServoClosedPos = 180;
    private const int rightServoClosedPos = 90;

    private const float servoCloseTime = 0.3f;

    private const int encoderInterruptPin = 2;
    private const int encoderSecondaryPin = 3;
    private bool connected = false;
    private void Awake()
    {
        connected = false;
    }
    public void Connect(string port)
    {
        if (connected)
        {
            throw new System.Exception("Arduino is already connected");
        }
        arduino.SetPort(port);
        arduino.Connect();
        connected = true;
        arduino.AttachServo(leftServoPin);
        arduino.AttachServo(rightServoPin);
        arduino.PinMode(miscInPin, (int)Mode.Input);
        arduino.PinMode(irPin, (int)Mode.Input_Pullup);
        arduino.PinMode(lickPin, (int)Mode.Input);
        arduino.PinMode(servoTransistorPin, (int)Mode.Output);
        arduino.PinMode(solenoidTransistorPin, (int)Mode.Output);
        arduino.AttachEncoder(encoderInterruptPin, encoderSecondaryPin);
        CalibrateLickMeter();
    }
    public override void Disconnect()
    {
        if (!connected) return;
        arduino.Disconnect();
        connected = false;
    }
    private void CalibrateLickMeter()
    {
        lickMeterExpectedVoltage = arduino.AnalogRead(lickPin);
    }
    public override int ReadJoystick()
    {
        throw new System.NotImplementedException();
    }
    public int ReadEncoder()
    {
        return arduino.ReadEncoder(encoderInterruptPin);
    }
    private void PositionServos(int leftPos, int rightPos)
    {
        arduino.DigitalWrite(servoTransistorPin,1);
        arduino.WriteServo(leftServoPin, leftPos);
        arduino.WriteServo(rightServoPin, rightPos);
        StartCoroutine(WaitToPowerOffServos());
    }
    IEnumerator WaitToPowerOffServos()
    {
        yield return new WaitForSeconds(servoCloseTime);
        arduino.DigitalWrite(servoTransistorPin, 0);
    }
    IEnumerator WaitToResetEncoder()
    {
        yield return new WaitForSeconds(servoCloseTime);
        ResetEncoder();
    }
    public override void CloseServos()
    {
        CloseServosNoReset();
        StartCoroutine(WaitToResetEncoder());
    }
    public void CloseServosNoReset()
    {
        PositionServos(leftServoClosedPos, rightServoClosedPos);
    }
    public override void OpenServos()
    {
        PositionServos(leftServoOpenPos,rightServoOpenPos);
    }
    public override float EstimatedServoCloseTime()
    {
        return servoCloseTime;
    }
    public override bool ReadIR()
    {
        return !arduino.DigitalRead(irPin);
    }
    public bool IsLicking()
    {
        return (lickMeterExpectedVoltage - LickMeterVoltage()) > lickMeterVoltageDelta;
    }
    public float LickMeterVoltage()
    {
        return arduino.AnalogRead(lickPin);
    }
    public void CheckConnection()
    {
        arduino.CheckConnection();
    }
    public void ResetEncoder()
    {
        arduino.ResetEncoder(encoderInterruptPin);
    }
    public float ReadMiscIn()
    {
        return arduino.AnalogRead(miscInPin);
    }
}
