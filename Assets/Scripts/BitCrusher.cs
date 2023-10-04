using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BitCrusher : MonoBehaviour
{
    private readonly int[] allowedBitDepths = { 8, 4, 2}; // Allowed bit depths
    [Range(0, 2)]
    public int bitDepthIndex = 0; // Index of the selected bit depth from the allowedBitDepths array
    private float _decibelFactor = 1f;
    [Range(0f , 1f)] public float decibelFader = 1f;

    private void OnAudioFilterRead(float[] data, int channels)
    {
        int bitDepthMaxValue = (int)Mathf.Pow(2, allowedBitDepths[bitDepthIndex]);

        if (bitDepthIndex == 0)
            _decibelFactor = 1;

        if (bitDepthIndex == 1)
            _decibelFactor = 0.8f;

        if (bitDepthIndex == 2)
            _decibelFactor = 0.3f;

        for (int i = 0; i < data.Length; i++)
        {
            data[i] = Mathf.Floor(data[i] * bitDepthMaxValue) / bitDepthMaxValue;
            data[i] *= _decibelFactor;
            data[i] *= decibelFader;
        }
    }
}

