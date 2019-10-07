using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IOInteractor : MonoBehaviour
{
    protected IODevice io;
    public void SetIODevice(IODevice newio)
    {
        io = newio;
    }
}
