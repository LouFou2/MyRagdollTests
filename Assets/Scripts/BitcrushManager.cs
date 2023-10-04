using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BitcrushManager : MonoBehaviour
{
    [SerializeField] private GameObject[] audioObjects;
    public BitCrusher[] bitCrushers;
    public AudioHighPassFilter[] highPassFilters; // high pass filter makes the bit crush sound good

    [SerializeField] [Range(0,2)] private float _bitCrushAmount = 0f;
    [HideInInspector] public float fadeValue1;
    [HideInInspector] public float fadeValue2;
    void Start()
    {
        bitCrushers = new BitCrusher[audioObjects.Length];
        highPassFilters = new AudioHighPassFilter[audioObjects.Length];

        for(int i = 0; i < bitCrushers.Length; i++) 
        {
            bitCrushers[i] = audioObjects[i].GetComponent<BitCrusher>();
            highPassFilters[i] = audioObjects[i].GetComponent<AudioHighPassFilter>();
        }
    }

    void Update()
    {
        if(_bitCrushAmount < 1) 
        {
            //== putting bitcrushers in a cue ==//
            bitCrushers[0].bitDepthIndex = 0; // first position in the cue
            bitCrushers[1].bitDepthIndex = 1; // second position in the cue

            //== cross-fade between the two bit crushers ==//
            float fadevalue = Mathf.InverseLerp(0, 1, _bitCrushAmount);
            fadeValue1 = 1 - fadevalue;
            fadeValue2 = fadevalue;
            bitCrushers[0].decibelFader = fadeValue1;
            bitCrushers[1].decibelFader = fadeValue2;
        }
        if (_bitCrushAmount >= 1)
        {
            bitCrushers[0].bitDepthIndex = 2; // the first bitcrusher takes the next position in the cue
            bitCrushers[1].bitDepthIndex = 1;

            //== cross-fade between the two bit crushers ==//
            float fadevalue = Mathf.InverseLerp(1, 2, _bitCrushAmount);
            fadeValue1 = 1 - fadevalue;
            fadeValue2 = fadevalue;

            bitCrushers[0].decibelFader = fadeValue2;
            bitCrushers[1].decibelFader = fadeValue1;
        }
        
        float filterFactor = Mathf.InverseLerp(0, 2, _bitCrushAmount);
        float filterAmount = 10 + (500 * filterFactor);
        highPassFilters[0].cutoffFrequency = filterAmount;
        highPassFilters[1].cutoffFrequency = filterAmount;
    }
}
