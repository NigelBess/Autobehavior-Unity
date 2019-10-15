using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaler : MonoBehaviour
{
    public static float gameStartTime = 0;
    public static float time
    {
        get
        {
            return gameStartTime + Time.time;
        }
    }
    private void Awake()
    {
        System.DateTime date = System.DateTime.Now;
        gameStartTime = 3600 * date.Hour + 60 * date.Minute + date.Second + ((float)date.Millisecond) / 1000f;
    }
}
