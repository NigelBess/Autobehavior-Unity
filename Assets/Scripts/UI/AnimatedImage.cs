using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedImage : MonoBehaviour
{
    [SerializeField] private Sprite[] images;
    [SerializeField] private int framesPerSecond = 30;
    private Image image;
    private int index;
    private void Awake()
    {
        index = 0;
        image = GetComponent<Image>();
        image.sprite = images[index];
    }
    private void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(SwapImage());
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
    IEnumerator SwapImage()
    {
        yield return new WaitForSeconds(1f/(float)framesPerSecond);
        index++;
        index %= images.Length;
        image.sprite = images[index];
        StartCoroutine(SwapImage());
    }
}
