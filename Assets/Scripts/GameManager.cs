﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CameraController camControl;
    [SerializeField] private GratedCircle gratedCircle;
    [SerializeField] private IODevice keyboard;
    [SerializeField] private ArduinoIO arduinoIO;
    [SerializeField] private GameObject coverPanel;
    [SerializeField] private GameObject failPanel;
    [SerializeField] private GameObject environment;
    [SerializeField] private Text devModeText;
    private IODevice io;
    [SerializeField] private CanvasManager cm;
    [SerializeField] private InputFields welcomeFields;
    [SerializeField] private Text welcomeErrorText;
    [SerializeField] private SoundMaker sound;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private Text pauseMenuText;
    private bool selfEnabled = true;
    private int numTrials;
    private const float controlPauseTime = 1f;
    private const float successPauseTime = 2f;
    private const float failPauseTime = 4f;
    private const float timeOutTime = 10f;
    private bool waitingForIR;
    private float startTime;

    private void Awake()
    {
        failPanel.SetActive(false);
        WelcomeError("");
        cm.Welcome();
        SetState(false);
        selfEnabled = false;
    }
    public void TryToStartGame()
    {
        try
        {

            welcomeFields.Save();
            numTrials = int.Parse(SessionData.numTrials);
            if (numTrials < 1) throw new System.Exception("Invalid number of trials.");
            Results.Malloc(numTrials);
            Results.CreateSaveFile(SessionData.saveDirectory,SessionData.mouseID,int.Parse(SessionData.sessionNumber));
            bool natBackground = int.Parse(SessionData.naturalisticBackground) > 0;
            coverPanel.SetActive(!natBackground);
            environment.SetActive(natBackground);
            if (SessionData.mouseID.Equals("dev",System.StringComparison.OrdinalIgnoreCase))
            {
                io = keyboard;
                camControl.SetIODevice(io);
                devModeText.text = "You have entered developer mode by setting the mouse ID to " + SessionData.mouseID + ". Do you want to continue?";
                cm.DevMode();
                return;
            }
            else
            {
                io = arduinoIO as IODevice;
                camControl.SetIODevice(io);
                arduinoIO.Connect(SessionData.port);
            }


            StartGame();
        }
        catch (System.Exception e)
        {
            WelcomeError(e);
            return;
        }
        StartGame();
        
    }
    public void StartGame()
    {
        startTime = Time.time;
        io.CloseServos();
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
        if (!gratedCircle.gameObject.activeSelf) return;
        if(gratedCircle.AtCenter())
        {
            Success();
        }
        if (gratedCircle.OutOfBounds())
        {
            Hit();
        }
    }
    private void WaitForIR()
    {
        waitingForIR = true;
        
    }
    private void WelcomeError(System.Exception e)
    {
        welcomeErrorText.text = e.Message;
    }
    private void WelcomeError(string msg)
    {
        welcomeErrorText.text = msg;
    }
    public void Pause()
    {
        Pause(true);
    }
    public void Pause(bool cancel)
    {
        io.CloseServos();
        SetState(false);
        StopAllCoroutines();
        if(cancel) Results.CancelTrial();
        WaitForIR();
        cm.Pause();
        selfEnabled = false;
    }
    public void Resume()
    {
        StopAllCoroutines();
        cm.HUD();
        selfEnabled = true;
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void MainMenu()
    {
        io.Disconnect();
        SceneManager.LoadScene(0);
    }
    private void StartTrial()
    {
        StopAllCoroutines();
        SetState(true);
        int side = ChooseSide();
        gratedCircle.Reset(side);
        Results.StartTrial(side,1);
        StartCoroutine(WaitForTimeOut());
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
            StartCoroutine(WaitToOpenServos(controlPauseTime));
        }
        else
        {
            camControl.enabled = false;
        }
        gratedCircle.gameObject.SetActive(running);

    }
    IEnumerator WaitToOpenServos(float time)
    {
        yield return new WaitForSeconds(time);
        io.OpenServos();
    }
    private void Success()
    {
        StopAllCoroutines();
        camControl.SnapTo(gratedCircle.GetWorldPos());
        camControl.enabled = false;
        io.GiveWater();
        io.CloseServos();
        Results.LogSuccess(io.ReadIR());
        DisableForSeconds(successPauseTime);
        StartCoroutine(WaitThenEndTrial(successPauseTime));
        sound.Success();

    }
    private void Hit()
    {
        Results.LogHit(io.ReadIR());
        Fail();
    }
    private void TimeOut()
    {
        bool irState = io.ReadIR();
        Results.LogTimeOut(irState);
        if (irState)
        {
            Fail();
        }
        else
        {
            io.CloseServos();
            EndTrial();
        }
    }
    public void TroubleshootHardware()
    {
        if(io!=null)   io.Disconnect();
        welcomeFields.Save();
        SceneManager.LoadScene(1,0);
    }
    private void Fail()
    {
        StopAllCoroutines();
        sound.Fail();
        failPanel.SetActive(true);
        io.CloseServos();
        camControl.enabled = false;
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
        if (Results.CurrentTrialNumber() >= numTrials)
        {
            EndGame();
            return;
        }
        failPanel.SetActive(false);
        WaitForIR();
    }
    private void EndGame()
    {
        int time = (int)(Time.time - startTime);
        int hours = time / 3600;
        time -= hours * 3600;
        int minutes = time / 60;
        int seconds = time - minutes * 60;
        Pause(false);
        pauseMenuText.text = "Mouse " + SessionData.mouseID + " completed all trials in " + hours.ToString("00")+ ":" + minutes.ToString("00") + ":" +seconds.ToString("00");
        continueButton.SetActive(false);
    }
    IEnumerator WaitForTimeOut()
    {
        yield return new WaitForSeconds(timeOutTime);
        TimeOut();
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
