using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleIfield : MonoBehaviour
{
    [SerializeField] private InputField iField;
    [SerializeField] private Toggle toggle;

    private void Awake()
    {
        iField.onValueChanged.AddListener(delegate { iFieldChanged(); });
        toggle.onValueChanged.AddListener(delegate { toggleChanged(); });
        iFieldChanged();
        toggleChanged();
    }
    private void iFieldChanged()
    {
        if (iField.text == "") iField.text = "0";
        int val = int.Parse(iField.text);
        if (val > 0) iField.text = "1";
        toggle.isOn = val > 0;
    }
    private void toggleChanged()
    {
        if (toggle.isOn)
        {
            iField.text = "1";
        }
        else
        {
            iField.text = "0";
        }
    }
}
