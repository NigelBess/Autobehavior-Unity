using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Results
{
    private const string extension = "csv";
    private static TrialData[] trials;
    private static int currentTrial = 0;
    private static DataLogger.Path path;
    public static void Malloc(int numTrials)
    {
        trials = new TrialData[numTrials];
        for (int i = 0; i < numTrials; i++)
        {
            trials[i] = new TrialData();
        }
        currentTrial = -1;
    }
    public static void CreateSaveFile(string directory, string mouseID, int sessionNum)
    {
        System.DateTime date = System.DateTime.Now;
        string dateStr = date.Year + "-" + date.Month + "-" + date.Day;
        string name = "Unity_" + mouseID + "_"+ dateStr+ "_"+sessionNum;
        CreateSaveFile(directory,name);
    }
    public static void CreateSaveFile(string directory, string fileName)
    {
        path = new DataLogger.Path(directory, fileName, extension);
        if(!path.Exists()) DataLogger.Save(path, "Response, Stimulus Position, Trial Start Time, Response Time, Correct?, IRSensorState, Opacity",true);
    }
    public static void Save()
    {
        DataLogger.Save(path,ThisTrial().ToString(),true);
    }
    private static TrialData ThisTrial()
    {
        return trials[currentTrial];
    }
    public static void StartTrial(int stimPosition, float opacity)
    {
        currentTrial++;
        if (currentTrial < 0) currentTrial = 0;
        TrialData t = ThisTrial();
        t.startTime = Time.time;
        t.stimPosition = stimPosition;
        t.opacity = opacity;
    }
    public static void CancelTrial()
    {
        //should only be called if the game is paused mid trial

        //do nothing if we havent started any trials
        if (currentTrial < 0) return;

        trials[currentTrial] = new TrialData();//reset info about current trial
        currentTrial--;
    }
    public static void LogSuccess(bool irSensorState)
    {
        LogResponse(ThisTrial().stimPosition, irSensorState);
    }
    public static void LogFail(bool irSensorState)
    {
        LogResponse(ThisTrial().stimPosition * -1, irSensorState);
    }
    public static void LogResponse(int side,bool irSensorState)
    {
        TrialData t = ThisTrial();
        if (t.response != 0) return;
        t.response = side;
        t.responseTime = Time.time;
        t.correct = t.response == t.stimPosition;
        t.irSensorState = irSensorState;
    }
    
    public static float TotalCorrectRate()
    {
        return CorrectRate(CompletedTrials());
    }
    public static float RespondedCorrectRate()
    {
        return CorrectRate(TrialsWithResponse(CompletedTrials()));
    }
    public static float RespondedWithIRCorrectRate()
    {
        return CorrectRate(TrialsWithResponse(TrialsWithIR(CompletedTrials())));
    }
    public static float TotalLeftBias()
    {
        return LeftBias(TrialsWithResponse(CompletedTrials()));
    }
    public static float LeftBiasWithIR()
    {
        return LeftBias(TrialsWithIR(CompletedTrials()));
    }
    public static float LeftProportionOnInterval(int numTrials)
    {
        if (currentTrial < 0) return Random.Range(0f, 1f);
        return LeftBias(LastNTrials(numTrials,TrialsWithResponse(CompletedTrialsPlus1())));
    }
    private static TrialData[] LastNTrials(int n,TrialData[] selectedTrials)
    {
        if (selectedTrials.Length < n) n = selectedTrials.Length;
        TrialData[] outVar = new TrialData[n];
        if (n == 0) return outVar;
        for (int i = 0; i < n; i++)
        {
            outVar[n - 1- i] = selectedTrials[selectedTrials.Length - 1 - i];
        }
        return outVar;
    }
    private static TrialData[] CompletedTrialsPlus1()
    {
        if (currentTrial < 0)
        {
            return new TrialData[0];
        }
        TrialData[] outVar = new TrialData[currentTrial+1];
        for (int i = 0; i < currentTrial+1; i++)
        {
            outVar[i] = trials[i];
        }
        return outVar;
    }
    private static TrialData[] CompletedTrials()
    {
        if (currentTrial <= 0)
        {
            return new TrialData[0];
        }
        TrialData[] outVar = new TrialData[currentTrial];
        for (int i = 0; i < currentTrial; i++)
        {
            outVar[i] = trials[i];
        }
        return outVar;
    }
    private static TrialData[] CorrectTrials(TrialData[] selectedTrials)
    {
        int correct = 0;
        for (int i = 0; i < selectedTrials.Length; i++)
        {
            if (selectedTrials[i].correct) correct++;
        }
        TrialData[] outVar = new TrialData[correct];
        int j = 0;
        for (int i = 0; i < selectedTrials.Length; i++)
        {
            if (selectedTrials[i].response != 0)
            {
                outVar[j] = selectedTrials[i];
                j++;
            }
        }
        return outVar;
    }
    private static TrialData[] TrialsWithResponse(TrialData[] selectedTrials)
    {
        int responded = 0;
        for (int i = 0; i < selectedTrials.Length; i++)
        {
            if (selectedTrials[i].response != 0) responded++;
        }
        TrialData[] outVar = new TrialData[responded];
        int j = 0;
        for (int i = 0; i < selectedTrials.Length; i++)
        {
            if (selectedTrials[i].response!=0)
            {
                outVar[j] = trials[i];
                j++;
            }
        }
        return outVar;
    }
    private static TrialData[] TrialsWithIR(TrialData[] selectedTrials)
    {
        int withIR = 0;
        for (int i = 0; i < selectedTrials.Length; i++)
        {
            if (selectedTrials[i].irSensorState) withIR++;
        }
        TrialData[] outVar = new TrialData[withIR];
        int j = 0;
        for (int i = 0; i < selectedTrials.Length; i++)
        {
            if (selectedTrials[i].irSensorState)
            {
                outVar[j] = trials[i];
                j++;
            }
        }
        return outVar;
    }
    private static float CorrectRate(TrialData[] selectedTrials)
    {
        int correct = 0;
        for (int i = 0; i < selectedTrials.Length; i++)
        {
            if (selectedTrials[i].correct) correct++;
        }
        return (float)correct / (float)(currentTrial + 1);
    }
    private static float LeftBias(TrialData[] selectedTrials)
    {
        int left = 0;
        for (int i = 0; i < selectedTrials.Length; i++)
        {
            Debug.Log(selectedTrials[i].response);
            if (selectedTrials[i].response == Globals.left) left++;
        }
        float outVar = ((float)left) / ((float)selectedTrials.Length);
        Debug.Log("left bias over " + selectedTrials.Length + " trials: " + outVar);
        return outVar;
    }
    private class TrialData
    {
        public int response = 0;//-1 left, 1 right, 0 no response
        public int stimPosition = 0;//-1 left, 1 right
        public float startTime = -1;
        public float responseTime = -1;
        public bool correct = false;
        public bool irSensorState = false;
        public float opacity = -1f;
        public override string ToString()
        {
            return response + ", " +
                stimPosition + ", " +
                startTime + ", " +
                responseTime + ", " +
                GameFunctions.BoolToInt(correct) + ", " +
                GameFunctions.BoolToInt(irSensorState) + ", " +
                opacity;
        }
    }
}


