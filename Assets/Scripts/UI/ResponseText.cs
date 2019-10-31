using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ResponseText : MonoBehaviour
{
    private Text text;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private float lifeTime = 1f;
    [SerializeField] private float fadeTime = 1f;
    private bool changingAlpha = false;
    private void Awake()
    {
        text = GetComponent<Text>();
        SetAlpha(0);
    }
    private void SetAlpha(float f)
    {
        Color c = text.color;
        c.a = f;
        text.color = c;
    }
    public void SetText(string msg)
    {
        SetText(msg, defaultColor);
    }
    public void SetText(string msg, Color c)
    {
        text.text = msg;
        text.color = c;
        SetAlpha(1f);
        enabled = true;
        changingAlpha = false;
        StartCoroutine(WaitForFade());
    }
    IEnumerator WaitForFade()
    {
        yield return new WaitForSeconds(lifeTime);
        changingAlpha = true;
    }
    private void Update()
    {
        if (!changingAlpha) return;
        float changeInAlpha = Time.deltaTime / fadeTime;
        float newAlpha = text.color.a - changeInAlpha;
        if (newAlpha <= 0)
        {
            newAlpha = 0f;
            enabled = false;
        }
        SetAlpha(newAlpha);
    }
}
