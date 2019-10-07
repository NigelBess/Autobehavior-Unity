using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputFields : MonoBehaviour
{
    protected InputField[] inputFields;
    [SerializeField] private string fileName;

    void Awake()
    {
        Load();
    }
    public void Load()
    {
        try
        {
            string[] fieldContents = DataLogger.ReadArray(fileName);
            for (int i = 0; i < inputFields.Length; i++)
            {
                if (i < fieldContents.Length) inputFields[i].text = fieldContents[i];
            }
        }
        catch
        {
            //do nothing
        }
    }
    public virtual void Save()
    {
        for (int i = 0; i < inputFields.Length; i++)
        {
            DataLogger.Save(fileName, inputFields[i].text, i > 0);
        }
    }
}
