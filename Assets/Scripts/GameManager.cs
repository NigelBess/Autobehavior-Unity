using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CameraController camControl;
    [SerializeField] private IODevice keyboard;
    [SerializeField] private ArduinoIO arduinoIO;
    private IODevice io;
    [SerializeField] private CanvasManager cm;
    [SerializeField] private InputFields welcomeFields;
    [SerializeField] private Text welcomeErrorText;
    private void Awake()
    {
        camControl.enabled = false;
        WelcomeError("");
        cm.Welcome();
    }
    public void StartGame()
    {
        try
        {

            welcomeFields.Save();
            if (SessionData.mouseID == "0")
            {
                io = keyboard;
            }
            else
            {
                io = arduinoIO as IODevice;
                arduinoIO.Connect(SessionData.port);
            }

            camControl.SetIODevice(io);
        }
        catch (System.Exception e)
        {
            WelcomeError(e.Message);
            return;
        }

        cm.HUD();
        }
    private void WelcomeError(string message)
    {
        welcomeErrorText.text = message;
    }
}
