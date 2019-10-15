using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMaker : MonoBehaviour
{
    [SerializeField] private AudioSource audio;
    [SerializeField] private AudioClip rewardNoise;
    [SerializeField] private AudioClip badNoise;
    public void Success()
    {
        PlayClip(rewardNoise);
    }
    public void Fail()
    {
        PlayClip(badNoise);
    }
    private void PlayClip(AudioClip clip)
    {
        audio.clip = clip;
        audio.Play();
    }
}
