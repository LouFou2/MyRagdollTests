using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorphManager : MonoBehaviour
{
    public Animator morphAnimator; // assign the ragdoll animator in inspector
    public BitcrushManager bitCrushManager; // assign the BitCrush Manager in inspector
    [SerializeField] private GameObject _morphMeshObject; // assign the mesh object in inspector
    [SerializeField] [Range(0f,3f)] private float _morphParam = 0f;
    public Mesh[] _morphMeshes;
    private SkinnedMeshRenderer _morphRenderer;
    void Start()
    {
        _morphRenderer = _morphMeshObject.GetComponent<SkinnedMeshRenderer>();

        if (morphAnimator == null )
            Debug.LogError("Assign ragdoll animator in inspector");
        if (_morphRenderer == null)
            Debug.LogError("Assign ragdoll character object that has mesh mesh in inspector");
        if (_morphMeshes.Length < 2)
            Debug.LogError("You need 3 morph meshes");
    }

    // Update is called once per frame
    void Update()
    {
        float bitCrushNormalized = Mathf.InverseLerp(0, 3, _morphParam); // morphParam range is 0-3
        if(_morphParam < 1f) 
        {
            _morphRenderer.sharedMesh = _morphMeshes[0];
            // lerp the "Decimate" BlendShape value between 0-1, using _morphParam (0-1)
            float decimateValue1 = Mathf.InverseLerp(0, 1, _morphParam);
            decimateValue1 *= 100f;
            Debug.Log(decimateValue1);
            _morphRenderer.SetBlendShapeWeight(0, decimateValue1);
        }
        if (_morphParam >= 1f && _morphParam < 2f)
        {
            _morphRenderer.sharedMesh = _morphMeshes[1];
            // lerp the "Decimate" BlendShape value between 0-1, using _morphParam (1-2)
            float decimateValue2 = Mathf.InverseLerp(1, 2, _morphParam);
            decimateValue2 *= 100f;
            Debug.Log(decimateValue2);
            _morphRenderer.SetBlendShapeWeight(0, decimateValue2);
        }
        if (_morphParam >= 2f && _morphParam < 3f)
        {
            _morphRenderer.sharedMesh = _morphMeshes[2];
            // lerp the "Decimate" BlendShape value between 0-1, using _morphParam (2-3)
            float decimateValue3 = Mathf.InverseLerp(2, 3, _morphParam);
            decimateValue3 *= 100f;
            Debug.Log(decimateValue3);
            //*** the last mesh doesn't have a shape key right now ***//
        }
        morphAnimator.SetFloat("MorphParam", _morphParam);
        bitCrushManager.bitCrushAmount = bitCrushNormalized * 2; // *2 because bitCrushAmount Range is 0-2
    }
}
