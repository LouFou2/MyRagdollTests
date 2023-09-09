using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WakeRunManager : MonoBehaviour
{
    [SerializeField] private Animator controlAnimator;
    [SerializeField] private Animator ragAnimator;
    [SerializeField] private AudioLoudnessDetection dbValueScript;
    [SerializeField] private Slider dbFloorSlider;
    [SerializeField] private Slider dbCeilingSlider;
    [SerializeField] private float dB_RangeFloor = -30f;
    [SerializeField] private float dB_RangeCeiling = -3f;
    private float dbValue = -160f;
    public float toRunTransition = 0f;

    void Start()
    {
        dbValue = dbValueScript.dbVal;
        dbFloorSlider.value = dB_RangeFloor;
        dbCeilingSlider.value = dB_RangeCeiling;
    }
    void Update()
    {
        dbValue = dbValueScript.dbVal;
        dB_RangeFloor = dbFloorSlider.value;
        dB_RangeCeiling = dbCeilingSlider.value;

        //--Map dbValue to useful ranges
        if (dbValue < dB_RangeFloor) { 
            toRunTransition = 0f;
        }
        if (dbValue >= dB_RangeFloor && dbValue <= dB_RangeCeiling) {
            toRunTransition = Mathf.InverseLerp(dB_RangeFloor, dB_RangeCeiling, dbValue);
        }
        if (dbValue > dB_RangeCeiling) { 
            toRunTransition = 1f;
        }

        //--Pass mapped values to Animators
        controlAnimator.SetFloat("GetUp", toRunTransition);
        ragAnimator.SetFloat("RagdollWake", toRunTransition);
    }
}
