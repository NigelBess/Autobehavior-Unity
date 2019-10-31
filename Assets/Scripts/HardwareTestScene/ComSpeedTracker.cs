using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComSpeedTracker : MonoBehaviour
{
    [SerializeField] private Text speedText;
    [SerializeField] private Text frameRateText;
    [SerializeField] private Text comsPerFrameText;
    [SerializeField] private float updateTime = 0.5f;
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

    void LateUpdate()
    {
        if (Time.time > nextUpdateTime)
        {
            UpdateText();
            nextUpdateTime = Time.time + updateTime;
        }
        coms = 0;
    }
    private void UpdateText()
    {
        speedText.text = "Arduino communications per second: " + ((float)coms / Time.deltaTime).ToString("F2");
        frameRateText.text = "Frames per second: " + Mathf.Floor(1f / Time.deltaTime).ToString("F0");
        comsPerFrameText.text = "Communications per frame: " + coms;
    }
}
