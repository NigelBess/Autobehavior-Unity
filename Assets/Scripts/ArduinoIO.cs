using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArduinoVars;

public class ArduinoIO : IODevice
{
    [SerializeField] private ArduinoInterface arduino;

    //pinouts
    private const int miscInPin = (int)Pin.A3;
    private const int irPin = 7;
    private const int lickPin = (int)Pin.A2;
    private const int servoTransistorPin = 6;
    private const int solenoidTransistorPin = 8;
    private const int encoderInterruptPin = 2;
    private const int encoderSecondaryPin = 3;
    private const int leftServoPin = 10;
    private const int rightServoPin = 9;

    private const float waterGiveTime = 0.1f; 

    private const float lickMeterVoltageDelta = 1f;//minimum expected change in voltage on a lick
    private float lickMeterExpectedVoltage;//expected voltage read by lickmeter when mouse is not licking (calibrated on start)

    
    
   
    //servo target positions in degrees
    private const int leftServoOpenPos = 90;
    private const int rightServoOpenPos = 180;
    private const int leftServoClosedPos = 180;
    private const int rightServoClosedPos = 90;

    //after the servos close how much should they back off (in degrees)
    private const int servoBackOff = 1;
    //reason for servoBackOff: when the servos open again, they tend to move a little bit in the wrong direction first. 
    //this causes the joystick to get bumped out of place. Backing off the servos mitigates this

    //how long do we expect it to take the servos to back off
    private const float backOffTime = 0.1f;

    //joystick values below threshold will be ignored. This value should be set to the max expected reading while servos are closed
    private const int joystickThreshold = 5;

    //joystick saturates at this value. This should be set to the max value read by the joystick when servos are open
    private const int maxJoystickValue = 85;

    //how much time do we expect it to take for the servos to close
    private const float servoCloseTime = 0.67f;

    //is the arduino connection established?
    private bool connected = false;

    //used to store the servo power off routine for cancellation. This allows us to cancel the last servo power-off when powering back on
    private IEnumerator servoPowerOffRoutine = null;
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
    public override float ReadJoystick()
    {
        int val = ReadEncoder();
        int absVal = Mathf.Abs(val);
        if (absVal <= joystickThreshold) return 0;
        val -= joystickThreshold * GameFunctions.Sign(val);
        val = Mathf.Clamp(val, -maxJoystickValue, maxJoystickValue);
        return (float)val / (float)maxJoystickValue;
    }
    public int ReadEncoder()
    {
        return arduino.ReadEncoder(encoderInterruptPin);
    }
    private void PositionServos(int leftPos, int rightPos)
    {
        PowerServos();
        arduino.DigitalWrite(servoTransistorPin,1);
        arduino.WriteServo(leftServoPin, leftPos);
        arduino.WriteServo(rightServoPin, rightPos);
    }
    void PowerServos()
    {
        if (servoPowerOffRoutine != null) StopCoroutine(servoPowerOffRoutine);
        arduino.DigitalWrite(servoTransistorPin, 1);
        servoPowerOffRoutine = WaitToPowerOffServos();
        StartCoroutine(servoPowerOffRoutine);
    }
    IEnumerator WaitToPowerOffServos()
    {
        yield return new WaitForSeconds(servoCloseTime);
        arduino.DigitalWrite(servoTransistorPin, 0);
        servoPowerOffRoutine = null;
    }
    IEnumerator WaitToResetEncoder()
    {
        yield return new WaitForSeconds(servoCloseTime);
        ResetEncoder();
    }
    public override void CloseServos()
    {
        CloseServos(true);
    }
    public  void CloseServos(bool resetEnc)
    {
        PositionServos(leftServoClosedPos,rightServoClosedPos);
        StartCoroutine(BackOffServos());
        if(resetEnc)   StartCoroutine(WaitToResetEncoder());
    }
    IEnumerator BackOffServos()
    {
        yield return new WaitForSeconds(servoCloseTime-backOffTime);
        arduino.WriteServo(leftServoPin, leftServoClosedPos-servoBackOff);
        arduino.WriteServo(rightServoPin, rightServoClosedPos+servoBackOff);

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
    public override void GiveWater()
    {
        //stub
    }
    public void OpenSolenoid()
    {
        arduino.DigitalWrite(solenoidTransistorPin,1);
    }
    public void CloseSolenoid()
    {
        arduino.DigitalWrite(solenoidTransistorPin,0);
    }
}
