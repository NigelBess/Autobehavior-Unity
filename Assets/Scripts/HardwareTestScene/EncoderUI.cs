using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncoderUI : MonoBehaviour
{
    [SerializeField] private ArduinoIO io;
    [SerializeField] private ComSpeedTracker tracker;
    [SerializeField] private int countsPerRev = 800;
    private void Update()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, io.ReadEncoder() * 360 / countsPerRev));
        tracker.AddCom();
    }
}
