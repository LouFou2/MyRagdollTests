using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WakeRunManager : MonoBehaviour
{
    [SerializeField] private Animator controlAnimator;
    [SerializeField] private Animator ragAnimator;
    [SerializeField] private AudioLoudnessDetection dbValueScript;
    [SerializeField] private float dB_RangeFloor = -50f;
    [SerializeField] private float dB_RangeCeiling = 0f;
    private float dbValue = -160f;
    private float toRunTransition = 0f;

    void Start()
    {
        dbValue = dbValueScript.dbVal;
    }
    void Update()
    {
        dbValue = dbValueScript.dbVal;

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
