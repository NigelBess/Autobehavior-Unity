using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestSceneManager : MonoBehaviour
{
    [SerializeField] private float updateTime = 0.5f;
    private float nextUpdateTime = 0f;
    [SerializeField] private ResponseText responseText;
    [SerializeField] private InputField comField;
    [SerializeField] private GameObject connectButton;
    [SerializeField] private GameObject disconnectButton;
    [SerializeField] private ArduinoIO io;
    [SerializeField] private ComSpeedTracker tracker;
    [SerializeField] private Text lickMeterStateText;
    [SerializeField] private Text lickMeterVoltageText;
    [SerializeField] private Text irStateText;
    [SerializeField] private Button servoButton;
    [SerializeField] private Text servoButtonText;
    [SerializeField] private Text encoderCountText;
    [SerializeField] private GameObject interactables;

    private DataLogger.Path comFieldPath;
    private bool connected = false;
    private void Awake()
    {
        io.Disconnect();
        ResetServoButton(false);
        Connected(false);
        comFieldPath = new DataLogger.Path("testSceneComField");
        try
        {
            comField.text = DataLogger.ReadArray(comFieldPath)[0];
        }
        catch { }
    }
    private void Connected(bool state)
    {
        connected = state;
        comField.interactable = !state;
        connectButton.SetActive(!state);
        disconnectButton.SetActive(state);
        interactables.SetActive(state);
    }
    public void Connect()
    {
        try
        {
            io.Connect(comField.text);
            tracker.AddCom();
            DataLogger.Save(comFieldPath, comField.text, false);
        }
        catch (System.Exception e)
        {
            responseText.SetText(e.Message, Color.red);
            return;
        }
        responseText.SetText("Connected successfully.");
        Connected(true);
        OpenServos();
    }
    public void Disconnect()
    {
        io.Disconnect();
        tracker.AddCom();
        Connected(false);
        responseText.SetText("Disconnected");
    }
    private void Update()
    {
        if (!connected) return;
        if (io.IsLicking())
        {
            lickMeterStateText.text = "Lick detected";
            lickMeterStateText.color = Color.green;
        }
        else
        {
            lickMeterStateText.text = "No lick detected";
            lickMeterStateText.color = Color.white;
        }
        tracker.AddCom();
        if (Time.time > nextUpdateTime)
        {
            lickMeterVoltageText.text = io.LickMeterVoltage().ToString("F2");
            nextUpdateTime = Time.time + updateTime;
        }
        
        tracker.AddCom();

        if (io.ReadIR())
        {
            irStateText.text = "Beam broken";
            irStateText.color = Color.green;
        }
        else
        {
            irStateText.text = "Beam not broken";
            irStateText.color = Color.white;
        }
        tracker.AddCom();
    }
    public void OpenServos()
    {
        PositionServos(true);
    }
    public void CloseServos()
    {
        PositionServos(false);
    }
    public void PositionServos(bool open)
    {
        tracker.AddCom(3);
        try
        {
            if (open)
            {
                io.OpenServos();
                responseText.SetText("Servos opened.");
            }
            else
            {
                io.CloseServos();
                responseText.SetText("Servos closed.");
            }

        }
        catch (System.Exception e)
        {
            responseText.SetText(e.Message, Color.red);
            return;
        }
        ResetServoButton(open);
    }
    private void ResetServoButton(bool open)
    {
        servoButton.onClick.RemoveAllListeners();
        if (open)
        {
            servoButtonText.text = "Close Servos";
            servoButton.onClick.AddListener(delegate { CloseServos(); });
        } else
        {
            servoButtonText.text = "Open Servos";
            servoButton.onClick.AddListener(delegate { OpenServos(); });
        }
    }
}
