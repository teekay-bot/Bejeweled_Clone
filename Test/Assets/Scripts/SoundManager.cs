using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public AudioSource destroyNoise;
    public AudioSource backgroundSound;
    public Button backgroundLoop;
    public Button backgroundLoopRed;
    public Button noise;
    public Button noiseRed;

    public void PlayDestroyNoise()
    { 
        destroyNoise.Play();  
    }

    public void TurnOnDestroyNoise()
    {
        destroyNoise.gameObject.SetActive(true);
        noiseRed.gameObject.SetActive(false);
        noise.gameObject.SetActive(true);
    }

    public void PlayBackgroundLoop()
    {
        backgroundSound.Play();
        backgroundLoopRed.gameObject.SetActive(false);
        backgroundLoop.gameObject.SetActive(true);
    }

    public void TurnOffDestroyNoise()
    {
        destroyNoise.gameObject.SetActive(false);
        noise.gameObject.SetActive(false);
        noiseRed.gameObject.SetActive(true);
    }

    public void TurnoffBackgroundLoop()
    {
        backgroundSound.Stop();
        backgroundLoop.gameObject.SetActive(false);
        backgroundLoopRed.gameObject.SetActive(true); 
    }
}
