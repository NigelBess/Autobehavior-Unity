using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComSpeedTracker : MonoBehaviour
{
    [SerializeField] private Text speedText;
    [SerializeField] private Text comTimeText;
    [SerializeField] private float updateTime = 1f;
    private float nextUpdateTime = 0f;
    private int coms = 0;
    private void Awake()
    {
        Application.targetFrameRate = -1;
    }
    public void AddCom(int num)
    {
        coms += num;
    }
    public void AddCom()
    {
        coms++;
    }

    void Update()
    {
        if (Time.time > nextUpdateTime)
        {
            UpdateText();
            nextUpdateTime = Time.time + updateTime;
        }
    }
    private void UpdateText()
    {
        speedText.text = "Arduino communications per second: " + ((float)coms / updateTime).ToString("F2");
        comTimeText.text = "Average communication time : " + (updateTime/coms).ToString("F6") + " seconds";
        coms = 0;
    }
}
