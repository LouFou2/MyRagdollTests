using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MorphManager : MonoBehaviour
{
    public Animator morphAnimator; // assign the ragdoll animator in inspector
    public Animator controlAnimator; // assign the ragdoll animator in inspector
    public BitcrushManager bitCrushManager; // assign the BitCrush Manager in inspector
    public GameObject _morphMeshObject; // assign the mesh object in inspector
    public GameObject _floorMeshObject; // assign the floor mesh object in inspector
    [SerializeField] [Range(0f,5f)] private float _morphParam = 0f;
    public Mesh[] _morphMeshes;
    public SkinnedMeshRenderer _morphRenderer;
    public MeshRenderer _floorRenderer;

    // Materials Shader Properties ==//
    public Material characterMaterial;
    public Material floorMaterial;
    public Color _characterColor;
    public Color _floorColor;
    public float _characterFresnelPower;
    void Start()
    {
        _morphRenderer = _morphMeshObject.GetComponent<SkinnedMeshRenderer>();
        characterMaterial = _morphRenderer.sharedMaterial;
        _characterColor = characterMaterial.GetColor("_Color"); // getting properties from shader
        _characterFresnelPower = characterMaterial.GetFloat("_FresnelPower"); 

        _floorRenderer = _floorMeshObject.GetComponent<MeshRenderer>();
        floorMaterial = _floorRenderer.sharedMaterial;
        _floorColor = floorMaterial.GetColor("_Color"); // getting properties from shader

        if (morphAnimator == null )
            Debug.LogError("Assign ragdoll animator in inspector");
        if (controlAnimator == null)
            Debug.LogError("Assign control animator in inspector");
        if (_morphRenderer == null)
            Debug.LogError("Assign ragdoll character object that has mesh mesh in inspector");
        if (_morphMeshes.Length < 4)
            Debug.LogError("You need 5 morph meshes");
    }

    // Update is called once per frame
    void Update()
    {
        float morphAmountNormalized = Mathf.InverseLerp(0, 5, _morphParam); // morphParam range is 0-5
        if(_morphParam == 0)
            _morphRenderer.sharedMesh = _morphMeshes[0];

        if (_morphParam > 0f && _morphParam < 1f) 
        {
            _morphRenderer.sharedMesh = _morphMeshes[1];
            // lerp the "Decimate" BlendShape value between 0-1, using _morphParam (0-1)
            float decimateValue1 = Mathf.InverseLerp(0, 1, _morphParam);
            decimateValue1 *= 100f;
            Debug.Log(decimateValue1);
            _morphRenderer.SetBlendShapeWeight(0, 100 - decimateValue1);
        }
        if (_morphParam >= 1f && _morphParam < 2f)
        {
            _morphRenderer.sharedMesh = _morphMeshes[2];
            // lerp the "Decimate" BlendShape value between 0-1, using _morphParam (1-2)
            float decimateValue2 = Mathf.InverseLerp(1, 2, _morphParam);
            decimateValue2 *= 100f;
            Debug.Log(decimateValue2);
            _morphRenderer.SetBlendShapeWeight(0, 100 - decimateValue2);
        }
        if (_morphParam >= 2f && _morphParam < 3f)
        {
            _morphRenderer.sharedMesh = _morphMeshes[3];
            // lerp the "Decimate" BlendShape value between 0-1, using _morphParam (2-3)
            float decimateValue3 = Mathf.InverseLerp(2, 3, _morphParam);
            decimateValue3 *= 100f;
            Debug.Log(decimateValue3);
            _morphRenderer.SetBlendShapeWeight(0, 100 - decimateValue3);
        }
        if (_morphParam >= 3f && _morphParam < 4f)
        {
            _morphRenderer.sharedMesh = _morphMeshes[4];
            // lerp the "Decimate" BlendShape value between 0-1, using _morphParam (3-4)
            float decimateValue4 = Mathf.InverseLerp(3, 4, _morphParam);
            decimateValue4 *= 100f;
            Debug.Log(decimateValue4);
            _morphRenderer.SetBlendShapeWeight(0, 100 - decimateValue4);
        }
        float controlAnimatorParam = Mathf.InverseLerp(0, 5, _morphParam); // calculation to pass param to Controller Animator
        float reverseAnimParam = 1 - controlAnimatorParam; // (reverse it because animation slows opposite relation to the decimation)

        //== The Ragdoll Animator ==//
        morphAnimator.SetFloat("MorphParam", _morphParam);
        //== The Controller Animator ==//
        controlAnimator.SetFloat("WalkSpeed", reverseAnimParam);
        //== The Bitcrush Audio Effect ==//
        bitCrushManager.bitCrushAmount = morphAmountNormalized * 2; // *2 because bitCrushAmount Range is 0-2
        //== Character Material (Shader Properties) ==//
        characterMaterial.SetColor("_Color", _characterColor);
        _characterFresnelPower = 20 - (morphAmountNormalized * 20); // 20 is the max amount currently set in shader for the float property
        characterMaterial.SetFloat("_FresnelPower", _characterFresnelPower);
        //== Floor Material (Shader Properties) ==//
        floorMaterial.SetColor("_Color", _floorColor);
    }
}
