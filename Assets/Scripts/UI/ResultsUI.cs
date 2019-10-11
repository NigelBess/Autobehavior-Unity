using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultsUI : MonoBehaviour
{
    [SerializeField] private Text numTrialsText;
    [SerializeField] private Text totalCorrectRateText;
    [SerializeField] private Text activeCorrectRateText;
    [SerializeField] private Text totalResponseRateText;
    [SerializeField] private Text activeResponseRateText;
    [SerializeField] private Text leftBiasText;

    private void OnEnable()
    {
        Refresh();
    }
    public void Refresh()
    {
        if (Results.isNull()) return;
        numTrialsText.text = "Completed trials: " + Results.CurrentTrialNumber();
        totalCorrectRateText.text = MakeString("Total correct rate", Results.TotalCorrectRate());
        activeCorrectRateText.text = MakeString("Active correct rate", Results.RespondedWithIRCorrectRate());
        totalResponseRateText.text = MakeString("Total response rate",Results.TotalResponseRate());
        activeResponseRateText.text = MakeString("Active response rate", Results.ActiveResponseRate());
        leftBiasText.text = MakeString("Left bias", Results.TotalLeftBias());
    }
    private string MakeString(string name,float value)
    {
        return name + ": " + (value * 100f).ToString("F2") + "%";
    }
}
