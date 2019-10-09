using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CameraController camControl;
    [SerializeField] private GratedCircle gratedCircle;
    [SerializeField] private IODevice keyboard;
    [SerializeField] private ArduinoIO arduinoIO;
    [SerializeField] private GameObject coverPanel;
    [SerializeField] private GameObject failPanel;
    [SerializeField] private GameObject environment;
    private IODevice io;
    [SerializeField] private CanvasManager cm;
    [SerializeField] private InputFields welcomeFields;
    [SerializeField] private Text welcomeErrorText;
    private bool selfEnabled = true;
    private int numTrials;
    private int currentTrialNumber;
    private const float controlPauseTime = 1f;
    private const float successPauseTime = 2f;
    private const float failPauseTime = 4f;
    private bool waitingForIR;

    private void Awake()
    {
        failPanel.SetActive(false);
        WelcomeError("");
        cm.Welcome();
        SetState(false);
        selfEnabled = false;
    }
    public void StartGame()
    {
        try
        {

            welcomeFields.Save();
            numTrials = int.Parse(SessionData.numTrials);
            Results.Malloc(numTrials);
            Results.CreateSaveFile(SessionData.saveDirectory,SessionData.mouseID,int.Parse(SessionData.sessionNumber));
            bool natBackground = int.Parse(SessionData.naturalisticBackground) > 0;
            coverPanel.SetActive(!natBackground);
            environment.SetActive(natBackground);
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
            WelcomeError(e);
            return;
        }

        cm.HUD();
        SetState(false);
        WaitForIR();
        selfEnabled = true;
    }
    private void Update()
    {
        if (!selfEnabled) return;
        if (waitingForIR)
        {
            if (io.ReadIR())
            {
                waitingForIR = false;
                StartTrial();
            }
            return;
        }
        if(gratedCircle.AtCenter())
        {
            Success();
        }
        if (gratedCircle.OutOfBounds()!=0)
        {
            Hit(-1*gratedCircle.OutOfBounds());
        }
    }
    private void WaitForIR()
    {
        waitingForIR = true;
        failPanel.SetActive(false);
    }
    private void WelcomeError(System.Exception e)
    {
        welcomeErrorText.text = e.Message;
    }
    private void WelcomeError(string msg)
    {
        welcomeErrorText.text = msg;
    }
    private void StartTrial()
    {
        StopAllCoroutines();
        SetState(true);
        int side = ChooseSide();
        gratedCircle.Reset(side);
        Results.StartTrial(side,1);
    }
    private int ChooseSide()
    {
        float leftBias = Results.LeftProportionOnInterval(6);
        float rand = Random.Range(0f,1f);
        if (rand > leftBias) return Globals.left;
        return Globals.right;
    }

    private void SetState(bool running)
    {
        if (running)
        {
            DisableForSeconds(camControl, controlPauseTime + io.EstimatedServoCloseTime());
        }
        else
        {
            camControl.enabled = false;
        }
        gratedCircle.gameObject.SetActive(running);

    }
    private void Success()
    {
        StopAllCoroutines();
        camControl.SnapTo(gratedCircle.GetWorldPos());
        camControl.enabled = false;
        io.CloseServos();
        Results.LogSuccess(io.ReadIR());
        DisableForSeconds(successPauseTime);
        StartCoroutine(WaitThenEndTrial(successPauseTime));

    }
    private void Hit(int side)
    {
        StopAllCoroutines();
        failPanel.SetActive(true);
        io.CloseServos();
        camControl.enabled = false;
        Results.LogResponse(side,io.ReadIR());
        DisableForSeconds(failPauseTime);
        StartCoroutine(WaitThenEndTrial(failPauseTime));

    }
    IEnumerator WaitThenEndTrial(float time)
    {
        yield return new WaitForSeconds(time);
        EndTrial();
    }
    private void EndTrial()
    {
        StopAllCoroutines();
        SetState(false);
        Results.Save();
        WaitForIR();
    }
    private void DisableForSeconds(MonoBehaviour obj, float time)
    {
        obj.enabled = false;
        StartCoroutine(EnableObject(obj, time));
    }
    IEnumerator EnableObject(MonoBehaviour obj, float time)
    {
        yield return new WaitForSeconds(time);
        obj.enabled = true;
    }
    private void DisableForSeconds(float time)
    {
        selfEnabled = false;
        StartCoroutine(EnableObject(time));
    }
    IEnumerator EnableObject(float time)
    {
        yield return new WaitForSeconds(time);
        selfEnabled = true;
    }
}
