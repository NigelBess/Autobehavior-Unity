using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Results
{
    private static TrialData[] trials;
    private static int currentTrial = 0;
    private static string directory;
    private static string fileName;

    public static void Malloc(int numTrials, string ndirectory)
    {
        trials = new TrialData[numTrials];
        currentTrial = 0;
        directory = ndirectory;
    }
    public static void SetFileName(string nfileName)
    {
        fileName = nfileName;
        DataLogger.Save(fileName, "Response, Response Time, Correct?, IRSensorState", true, directory);
    }
    public static void Save()
    {
        TrialData t = trials[currentTrial];
        string data = t.response + ", " + t.responseTime + ", " + GameFunctions.BoolToInt(t.correct) + ", " + GameFunctions.BoolToInt(t.irSensorState);
        DataLogger.Save(fileName,data,true,directory);
    }
}
public class TrialData
{
    public int response = 0;//-1 left, 1 right, 0 no response
    public float responseTime = -1;
    public bool correct = false;
    public bool irSensorState = true;
}

