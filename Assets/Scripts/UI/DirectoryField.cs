using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleFileBrowser;

public class DirectoryField : MonoBehaviour
{
    [SerializeField] private CanvasManager cm;
    [SerializeField] private InputField iField;
    public void Browse()
    {
        cm.Blank();
        StartCoroutine(ShowLoadDialogCoroutine());
    }
    IEnumerator ShowLoadDialogCoroutine()
    {
        yield return FileBrowser.WaitForSaveDialog(true, null, "Choose Save directory for data", "Choose");
        cm.Previous();
        if (FileBrowser.Success) iField.text = FileBrowser.Result;
    }
}