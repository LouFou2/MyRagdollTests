using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MorphManager : MonoBehaviour
{
    public Animator morphAnimator; // assign the ragdoll animator in inspector
    public Animator controlAnimator; // assign the ragdoll animator in inspector
    public BitcrushManager bitCrushManager; // assign the BitCrush Manager in inspector
    public GameObject morphMeshObject; // assign the mesh object in inspector
    public GameObject floorMeshObject; // assign the floor mesh object in inspector
    [SerializeField] [Range(0f,5f)] private float _morphParam = 0f;
    public Mesh[] morphMeshes;
    public SkinnedMeshRenderer morphRenderer;
    public MeshRenderer floorRenderer;

    // Materials Shader Properties ==//
    public Material characterMaterial;
    public Material floorMaterial;
    public Color characterColor;
    public float colorMixer;
    public Color floorColor;
    public float characterFresnelPower;

    private void Awake()
    {
        morphRenderer = morphMeshObject.GetComponent<SkinnedMeshRenderer>();
        characterMaterial = morphRenderer.sharedMaterial;
        characterColor = characterMaterial.GetColor("_Color"); // getting properties from shader
        characterFresnelPower = characterMaterial.GetFloat("_FresnelPower");
        colorMixer = characterMaterial.GetFloat("_ColorMixer");

        floorRenderer = floorMeshObject.GetComponent<MeshRenderer>();
        floorMaterial = floorRenderer.sharedMaterial;
        floorColor = floorMaterial.GetColor("_Color"); // getting properties from shader
    }
    void Start()
    {
        if (morphAnimator == null )
            Debug.LogError("Assign ragdoll animator in inspector");
        if (controlAnimator == null)
            Debug.LogError("Assign control animator in inspector");
        if (morphRenderer == null)
            Debug.LogError("Assign ragdoll character object that has mesh mesh in inspector");
        if (morphMeshes.Length < 4)
            Debug.LogError("You need 5 morph meshes");
    }

    // Update is called once per frame
    void Update()
    {
        float morphAmountNormalized = Mathf.InverseLerp(0, 5, _morphParam); // morphParam range is 0-5
        if(_morphParam == 0)
            morphRenderer.sharedMesh = morphMeshes[0];

        if (_morphParam > 0f && _morphParam < 1f) 
        {
            morphRenderer.sharedMesh = morphMeshes[1];
            // lerp the "Decimate" BlendShape value between 0-1, using _morphParam (0-1)
            float decimateValue1 = Mathf.InverseLerp(0, 1, _morphParam);
            decimateValue1 *= 100f;
            Debug.Log(decimateValue1);
            morphRenderer.SetBlendShapeWeight(0, 100 - decimateValue1);
        }
        if (_morphParam >= 1f && _morphParam < 2f)
        {
            morphRenderer.sharedMesh = morphMeshes[2];
            // lerp the "Decimate" BlendShape value between 0-1, using _morphParam (1-2)
            float decimateValue2 = Mathf.InverseLerp(1, 2, _morphParam);
            decimateValue2 *= 100f;
            Debug.Log(decimateValue2);
            morphRenderer.SetBlendShapeWeight(0, 100 - decimateValue2);
        }
        if (_morphParam >= 2f && _morphParam < 3f)
        {
            morphRenderer.sharedMesh = morphMeshes[3];
            // lerp the "Decimate" BlendShape value between 0-1, using _morphParam (2-3)
            float decimateValue3 = Mathf.InverseLerp(2, 3, _morphParam);
            decimateValue3 *= 100f;
            Debug.Log(decimateValue3);
            morphRenderer.SetBlendShapeWeight(0, 100 - decimateValue3);
        }
        if (_morphParam >= 3f && _morphParam < 4f)
        {
            morphRenderer.sharedMesh = morphMeshes[4];
            // lerp the "Decimate" BlendShape value between 0-1, using _morphParam (3-4)
            float decimateValue4 = Mathf.InverseLerp(3, 4, _morphParam);
            decimateValue4 *= 100f;
            Debug.Log(decimateValue4);
            morphRenderer.SetBlendShapeWeight(0, 100 - decimateValue4);
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
        characterMaterial.SetColor("_Color", characterColor);
        colorMixer = Mathf.Clamp01( morphAmountNormalized * 4); // times 4 so it transitions within first quarter of transition (can adjust)
        characterMaterial.SetFloat("_ColorMixer", colorMixer);
        characterFresnelPower = 20 - (morphAmountNormalized * 20); // 20 is the max amount currently set in shader for the float property
        characterMaterial.SetFloat("_FresnelPower", characterFresnelPower);
        //== Floor Material (Shader Properties) ==//
        floorMaterial.SetColor("_Color", floorColor);
    }
}
