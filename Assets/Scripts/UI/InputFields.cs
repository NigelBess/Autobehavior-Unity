using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputFields : MonoBehaviour
{
    protected InputField[] inputFields;
    [SerializeField] private string fileName;
    private DataLogger.Path path;

    void Awake()
    {
        Load();
    }
    public void Load()
    {
        if (path==null) path = new DataLogger.Path(fileName);
        try
        {
            string[] fieldContents = DataLogger.ReadArray(path);
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
        if (path == null) path = new DataLogger.Path(fileName);
        for (int i = 0; i < inputFields.Length; i++)
        {
            DataLogger.Save(path, inputFields[i].text, i > 0);
        }
    }
}
