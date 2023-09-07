using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioLoudnessDetection : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip micClip;

    [SerializeField] private Slider dbSlider;
    [SerializeField] private TextMeshProUGUI dbValueText;

    [SerializeField] private Slider max_dbSlider;
    [SerializeField] private TextMeshProUGUI max_dbValueText;

    [SerializeField] private Slider pitchSlider;
    [SerializeField] private TextMeshProUGUI pitchValueText;

    private float rmsVal;
    public float dbVal;
    private float max_dbVal = -160f; // start at minimum
    public float pitchVal;
    private float Threshold;

    private const int QSamples = 1024;
    private const float RefValue = 0.1f;
    

    private float[] _samples;
    private float[] _spectrum;
    private float _fSample;

    void Start()
    {
        _samples = new float[QSamples];
        _spectrum = new float[QSamples];
        _fSample = AudioSettings.outputSampleRate;

        MicrophoneToAudioClip();

        //check list of mic connected
        foreach (var device in Microphone.devices)
        {
            Debug.Log("Name: " + device);
        }
    }

    void Update()
    {
        AnalyzeSound(source);

        // UI for decibels
        dbSlider.value = dbVal;
        dbValueText.text = "dB Value:\n" + dbVal.ToString("F1") + " dB";
        
        // UI for max decibels
        if (dbVal > max_dbVal) 
        {
            max_dbVal = dbVal;
            max_dbSlider.value = dbVal;
            max_dbValueText.text = "dB Max:\n" + dbVal.ToString("F1") + " dB";
        }

        // UI for pitch 
        pitchSlider.value = pitchVal;
        pitchValueText.text = "Pitch Value:\n" + pitchVal.ToString("F0") + " Hz";
    }


    void AnalyzeSound(AudioSource audio)
    {
        audio.GetComponent<AudioSource>().GetOutputData(_samples, 0); // fill array with samples
        int i;
        float sum = 0;
        for (i = 0; i < QSamples; i++)
        {
            sum += _samples[i] * _samples[i]; // sum squared samples
        }

        rmsVal = Mathf.Sqrt(sum / QSamples); // rms = square root of average
        dbVal = 20 * Mathf.Log10(rmsVal / RefValue); // calculate dB
        if (dbVal < -160) dbVal = -160; // clamp it to -160dB min
        //if (dbVal > 0) dbVal = 0; // clamp it to 0dB max? probably not neccesarry

        // get sound spectrum
        audio.GetComponent<AudioSource>().GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);
        float maxV = 0;
        var maxN = 0;
        for (i = 0; i < QSamples; i++)
        { // find max 
            if (!(_spectrum[i] > maxV) || !(_spectrum[i] > Threshold))
                continue;

            maxV = _spectrum[i];
            maxN = i; // maxN is the index of max
        }
        float freqN = maxN; // pass the index to a float variable
        if (maxN > 0 && maxN < QSamples - 1)
        { // interpolate index using neighbours
            var dL = _spectrum[maxN - 1] / _spectrum[maxN];
            var dR = _spectrum[maxN + 1] / _spectrum[maxN];
            freqN += 0.5f * (dR * dR - dL * dL);
        }
        pitchVal = freqN * (_fSample / 2) / QSamples; // convert index to frequency
    }

    public void MicrophoneToAudioClip()
    {
        string micName = Microphone.devices[0];
        Debug.Log("mic input" + micName);
        micClip = Microphone.Start(micName, true, 20, AudioSettings.outputSampleRate);
        source.clip = micClip;

        /*while (!(Microphone.GetPosition(micName) > 0)); // this should have something to do if there is no mic
        { }*/

        source.Play();
    }

}
