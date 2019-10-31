using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TestSceneManager : MonoBehaviour
{
    [SerializeField] private float updateTime = 0.25f;
    private float nextUpdateTime = 0f;
    [SerializeField] private ResponseText responseText;
    [SerializeField] private InputField comField;
    [SerializeField] private GameObject connectButton;
    [SerializeField] private GameObject disconnectButton;
    [SerializeField] private ArduinoIO io;
    [SerializeField] private ComSpeedTracker tracker;
    [SerializeField] private Text miscInText;
    [SerializeField] private Text lickMeterStateText;
    [SerializeField] private Text lickMeterVoltageText;
    [SerializeField] private Text irStateText;
    [SerializeField] private Button servoButton;
    [SerializeField] private Text servoButtonText;
    [SerializeField] private Text encoderCountText;
    [SerializeField] private GameObject interactables;
    private const float pauseForMessageSendTime = 0.1f;
    const int comFieldIndex = 4;

    private DataLogger.Path fieldValuesPath;
    private bool connected = false;
    private void Awake()
    {
        io.Disconnect();
        ResetServoButton(false);
        Connected(false);
        fieldValuesPath = new DataLogger.Path(Globals.mainMenuFieldsFileName);
        try
        {
            string[] values = DataLogger.ReadArray(fieldValuesPath);
            comField.text = values[comFieldIndex];
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
            try
            {
                string[] values = DataLogger.ReadArray(fieldValuesPath);
                values[comFieldIndex] = comField.text;
                DataLogger.Save(fieldValuesPath, values, false);
            }
            catch { }
                  
        }
        catch (System.Exception e)
        {
            responseText.SetText(e.Message, Color.red);
            return;
        }
 
        Connected(true);
        OpenServos();
        responseText.SetText("Connected successfully.",Color.green);
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
            tracker.AddCom();

            miscInText.text = io.ReadMiscIn().ToString("F2");
            tracker.AddCom();

            nextUpdateTime = Time.time + updateTime;
        }
        

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

        encoderCountText.text = io.ReadEncoder().ToString();
        tracker.AddCom();

        //stress test
        int stressTestsPerFrame = 5;
        for (int i = 0; i < stressTestsPerFrame; i++)
        {
            io.CheckConnection();
        }
        tracker.AddCom(stressTestsPerFrame);
    }
    public void ResetEncoder()
    {
        try
        {
            io.ResetEncoder();
        }
        catch (System.Exception e)
        {
            responseText.SetText(e.Message);
            return;
        }
        
        responseText.SetText("Encoder reset to zero.");

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
                io.CloseServosNoReset();
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
    public void MainMenu()
    {
        enabled = false;
        StartCoroutine(WaitThenGoToMainMenu());
        
    }
    IEnumerator WaitThenGoToMainMenu()
    {
        yield return new WaitForSeconds(pauseForMessageSendTime);
        io.Disconnect();
        SceneManager.LoadScene("Main", 0);
    }
}
