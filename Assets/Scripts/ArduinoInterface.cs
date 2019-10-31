using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Diagnostics;
using System.Threading;

public class ArduinoInterface : MonoBehaviour
{
    private static byte terminator = 254;
    private byte[] incomingMessageBuffer = new byte[512];
    private const byte errorByte = 253;
    private const int readTimeout = 500;//ms

    private const int maxAnalogRead = 1023;
    private const float maxVoltage = 5f;

    private SerialPort port;
    public void SetPort(string nPortName)
    {
        port = new SerialPort();
        port.Close();
        port.PortName = nPortName;
        port.DtrEnable = false;
    }
    private void OnApplicationQuit()
    {
        Disconnect();
    }
    public void Connect()
    {
        port.BaudRate = 250000;
        port.DataBits = 8;
        port.NewLine = ((char)terminator).ToString();
        port.Encoding = System.Text.Encoding.UTF8;
        port.Open();
        port.DiscardOutBuffer();
        port.DiscardInBuffer();
        WaitForConnection();

    }
    public void WaitForConnection()
    {
        bool connected = CheckConnection();
        while (!connected)
        {
            connected = CheckConnection();
        }
    }
    public bool CheckConnection()
    {
        SendMessage(new byte[1] { 253 });
        byte[] response = GetMessage(false);
        return (response[0] == 0);
    }
    public void Disconnect()
    {
        if (port != null)
        {
            port.DtrEnable = true;
            if (port.IsOpen)
            {
                port.Close();
            }
        }
    }
    public byte[] SendMessageReliable(byte[] message)
    {
        SendMessage(message);
        
        byte[] returnMsg =  GetMessage(true);
        return returnMsg;
        
    }
    public void SendMessage(byte[] message)
    {
        byte[] terminatedMessage = new byte[message.Length + 1];
        for (int i = 0; i < message.Length; i++)
        {
            terminatedMessage[i] = message[i];
        }
        terminatedMessage[terminatedMessage.Length - 1] = terminator;
        port.Write(terminatedMessage,0,terminatedMessage.Length);
        port.DiscardOutBuffer();
    }
    public byte[] GetMessage()
    {
        return GetMessage(true);
    }
    public byte[] GetMessage(bool safe)
    {
        int i = 0;
        Stopwatch timer = Stopwatch.StartNew();
        while (i < incomingMessageBuffer.Length)
        {
            while (port.BytesToRead < 1)//wait for next byte to become available
            {
                if (timer.ElapsedMilliseconds > readTimeout)
                {
                    throw new System.TimeoutException("Arduino has timed out or disconnected.");
                }
            }
            byte incoming = (byte)port.ReadByte();
            if (incoming == terminator)
            {
                break;
            }
            incomingMessageBuffer[i] = incoming;
            i++;
        }
        if (safe && incomingMessageBuffer[0] == errorByte)
        {
            string errorMsg = "";
            for (int j = 1; j < i; j++)
            {
                errorMsg += (char)incomingMessageBuffer[j];
            }
            throw new System.InvalidOperationException(errorMsg);
        }
        byte[] output = new byte[i];
        for (int j = 0; j < i; j++)
        {
            output[j] = incomingMessageBuffer[j];
        }
        port.DiscardInBuffer();
        return output;
    }
    public void PinMode(int pin, int mode)
    {
        SendMessageReliable(new byte[3] {0, (byte)pin, (byte)mode });
    }
    public void DigitalWrite(int pin, int state)
    {
        SendMessageReliable(new byte[3] { 1, (byte)pin, (byte)state });
    }
    public void AnalogWrite(int pin, int state)
    {
        SendMessageReliable(new byte[3] { 2, (byte)pin, (byte)state });
    }
    public bool DigitalRead(int pin)
    {
        byte[] response = SendMessageReliable(new byte[2] { 3, (byte)pin});
        return (response[0] > 0);
    }
    public float AnalogRead(int pin)
    {
        byte[] response = SendMessageReliable(new byte[2] { 4, (byte)pin });
        float outVar = maxVoltage*((float)ParseInt(response)) / ((float)maxAnalogRead);
        return outVar;
    }
    public void AttachServo(int pin)
    {
        SendMessageReliable(new byte[2] {5, (byte)pin});
    }
    public void WriteServo(int pin, int angle)
    {
        SendMessageReliable(new byte[3] { 6, (byte)pin, (byte)angle });
    }
    public void DetachServo(int pin)
    {
        SendMessageReliable(new byte[2] { 7, (byte)pin});
    }
    public void AttachEncoder(int interruptPin, int secondaryPin)
    {
        SendMessageReliable(new byte[3] {8, (byte)interruptPin, (byte)secondaryPin});
    }
    public int ReadEncoder(int interruptPin)
    {
        byte[] response = SendMessageReliable(new byte[2] { 9, (byte)interruptPin });
        //DebugBytes(response);
        return ParseInt(response);
    }
    public void ResetEncoder(int interruptPin)
    {
        SendMessageReliable(new byte[2] { 10, (byte)interruptPin });
    }
    private int ParseInt(byte[] response)
    {
        int outVar = 0;
        for (int i = 1; i < response.Length; i++)
        {
            outVar += (int)Mathf.Pow(2, 7*(i - 1)) * response[i];
        }
        if (response[0] > 0) outVar *= -1;
        return outVar;
    }
    private void DebugBytes(byte[] bytes)
    {
        string msg = "";
        for (int i = 0; i < bytes.Length; i++)
        {
            msg += bytes[i].ToString() + " ";
        }
        UnityEngine.Debug.Log(msg);
    }
}
