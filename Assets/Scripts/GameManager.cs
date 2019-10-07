﻿using System.Collections;
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
    private bool waitingForIR;

    private void Awake()
    {
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
            Results.Malloc(numTrials,SessionData.saveDirectory);
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
            WelcomeError(e.Message);
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
    }
    private void WaitForIR()
    {
        waitingForIR = true;
    }
    private void WelcomeError(string message)
    {
        welcomeErrorText.text = message;
    }
    private void StartTrial()
    {
        StopAllCoroutines();
        SetState(true);
        int side = ChooseSide();
        gratedCircle.Reset(side);
    }
    private int ChooseSide()
    {
        return 1;
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
        Debug.Log("setstate " + running);
    }
    private void Success()
    {
        StopAllCoroutines();
        camControl.SnapTo(gratedCircle.GetWorldPos());
        camControl.enabled = false;
        io.CloseServos();
        DisableForSeconds(successPauseTime);
        StartCoroutine(WaitThenEndTrial(successPauseTime));

    }
    IEnumerator WaitThenEndTrial(float time)
    {
        yield return new WaitForSeconds(time);
        EndTrial();
    }
    private void EndTrial()
    {
        StopAllCoroutines();
        Debug.Log("trial ended");
        SetState(false);
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
