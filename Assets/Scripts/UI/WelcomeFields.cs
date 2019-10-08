using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WelcomeFields : InputFields
{
    [SerializeField] private InputField mouseIDField;
    [SerializeField] private InputField rigField;
    [SerializeField] private InputField sessionField;
    [SerializeField] private InputField numTrialsField;
    [SerializeField] private InputField portField;
    [SerializeField] private InputField naturalisticBackgroundField;
    [SerializeField] private InputField rewardOnIncorrectField;
    [SerializeField] private InputField saveDirectoryField;
    private void Awake()
    {
        inputFields = new InputField[8] { mouseIDField, rigField, sessionField, numTrialsField, portField, naturalisticBackgroundField, rewardOnIncorrectField, saveDirectoryField };
        Load();
    }
    public override void Save()
    {
        SessionData.mouseID = mouseIDField.text;
        SessionData.rig = rigField.text;
        SessionData.numTrials = numTrialsField.text;
        SessionData.sessionNumber = sessionField.text;
        SessionData.port = portField.text;
        SessionData.naturalisticBackground = naturalisticBackgroundField.text;
        SessionData.rewardOnIncorrect = rewardOnIncorrectField.text;
        SessionData.saveDirectory = saveDirectoryField.text;
        base.Save();
    }
}
