using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlay : MonoBehaviour
{
    private AudioSource woofAudio;
    [SerializeField] private WakeRunManager runTransitionScript;
    [SerializeField] private float woofPitchMin;
    [SerializeField] private float woofPitchMax;
    [SerializeField] private float transitionValueBuffer = 0.6f;
    private float moddedPitchValue = 0f;
    void Start()
    {
        woofAudio = GetComponent<AudioSource>();
        woofAudio.pitch = woofPitchMin;
    }
    private void Update()
    {
        float runTransitionValue = runTransitionScript.toRunTransition;
        float woofPitchRange = woofPitchMax - woofPitchMin;
        if (woofAudio.pitch <= transitionValueBuffer) 
        { 
            moddedPitchValue = woofPitchMin;
        }
        if (woofAudio.pitch > transitionValueBuffer)
        {
            moddedPitchValue = Mathf.Lerp(woofPitchMin, woofPitchMax, runTransitionValue);
        }
            
        woofAudio.pitch = moddedPitchValue;
    }
    public void PlayWoof() 
    {
        //woofAudio.Stop();
        woofAudio.Play();
    }
    public void StopWoof() //In case I need it.
    {
        woofAudio.Stop();
    }
}
