using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewRagdollManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _ragdollLimbs; //*** put limbs in order of loat list below!!!
    [Header("Add array objects in order of the list below")]
    
    private CopyLimb[] _copyLimbScript;
    [SerializeField][Range(0f, 1f)] private float _headPower = 1f; // *** e.g. Head = 0
    [SerializeField][Range(0f, 1f)] private float _neckPower = 1f; // *** neck = 1 ...
    [SerializeField][Range(0f, 1f)] private float _bustPower = 1f;
    [SerializeField][Range(0f, 1f)] private float _torsoPower = 1f;
    [SerializeField][Range(0f, 1f)] private float _tumPower = 1f;
    [SerializeField][Range(0f, 1f)] private float _pelvisPower = 1f;
    [SerializeField][Range(0f, 1f)] private float _shoulder_L_Power = 1f;
    [SerializeField][Range(0f, 1f)] private float _uprArm_L_Power = 1f;
    [SerializeField][Range(0f, 1f)] private float _lwrArm_L_Power = 1f;
    [SerializeField][Range(0f, 1f)] private float _shoulder_R_Power = 1f;
    [SerializeField][Range(0f, 1f)] private float _uprArm_R_Power = 1f;
    [SerializeField][Range(0f, 1f)] private float _lwrArm_R_Power = 1f;
    [SerializeField][Range(0f, 1f)] private float _hip_L_Power = 1f;
    [SerializeField][Range(0f, 1f)] private float _uprLeg_L_Power = 1f;
    [SerializeField][Range(0f, 1f)] private float _lwrLeg_L_Power = 1f;
    [SerializeField][Range(0f, 1f)] private float _hip_R_Power = 1f;
    [SerializeField][Range(0f, 1f)] private float _uprLeg_R_Power = 1f;
    [SerializeField][Range(0f, 1f)] private float _lwrLeg_R_Power = 1f;
    private float[] _limbPowers;

    void Start()
    {
        _copyLimbScript = new CopyLimb[_ragdollLimbs.Length];
        _limbPowers = new float[_ragdollLimbs.Length];

        for (int i = 0; i < _ragdollLimbs.Length; i++)
        {
            _copyLimbScript[i] = _ragdollLimbs[i].GetComponent<CopyLimb>();
            _limbPowers[i] = _copyLimbScript[i].ragdollPowerFactor;
        }
    }

    void Update()
    {
        _limbPowers[0] = _headPower; // *** indexes has to be in the right order for this to work
        _limbPowers[1] = _neckPower;
        _limbPowers[2] = _bustPower;
        _limbPowers[3] = _torsoPower;
        _limbPowers[4] = _tumPower;
        _limbPowers[5] = _pelvisPower;
        _limbPowers[6] = _shoulder_L_Power;
        _limbPowers[7] = _uprArm_L_Power;
        _limbPowers[8] = _lwrArm_L_Power;
        _limbPowers[9] = _shoulder_R_Power;
        _limbPowers[10] = _uprArm_R_Power;
        _limbPowers[11] = _lwrArm_R_Power;
        _limbPowers[12] = _hip_L_Power;
        _limbPowers[13] = _uprLeg_L_Power;
        _limbPowers[14] = _lwrLeg_L_Power;
        _limbPowers[15] = _hip_R_Power;
        _limbPowers[16] = _uprLeg_R_Power;
        _limbPowers[17] = _lwrLeg_R_Power;

        for (int i = 0; i < _ragdollLimbs.Length; i++)
        {
            _copyLimbScript[i].ragdollPowerFactor = _limbPowers[i];
        }
    }
}
