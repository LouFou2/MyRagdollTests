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
    [SerializeField] private GameObject _morphPointObject;
    [SerializeField] private GameObject _playerObject;
    [SerializeField] [Range(0f,5f)] private float _morphParam = 0f;
    [HideInInspector] public Mesh[] morphMeshes;
    [HideInInspector] public SkinnedMeshRenderer morphRenderer;
    [HideInInspector] public MeshRenderer floorRenderer;

    // Materials Shader Properties ==//
    public Material opaqueCharacterMaterial;
    public Material transparentCharacterMaterial;
    public Material opaqueFloorMaterial;
    public Material transparentFloorMaterial;
    [HideInInspector] public Material characterMaterial;
    [HideInInspector] public Material floorMaterial;
    [HideInInspector] public float characterColorMixer;
    [HideInInspector] public float characterFresnelPower; // Character Material Shader Properties
    [HideInInspector] public float characterColorAlpha;
    [HideInInspector] public float charEmissionAmount;
    [HideInInspector] public float floorAlpha; // Floor Material Shader Properties
    [HideInInspector] public float floorEmission;
    [HideInInspector] public float floorColorAmount;

    private void Awake()
    {
        morphRenderer = morphMeshObject.GetComponent<SkinnedMeshRenderer>();
        characterMaterial = morphRenderer.sharedMaterial;
        characterFresnelPower = characterMaterial.GetFloat("_FresnelPower");
        characterColorMixer = characterMaterial.GetFloat("_ColorMixer");
        characterColorAlpha = characterMaterial.GetFloat("_AlphaFactor");
        charEmissionAmount = characterMaterial.GetFloat("_EmissionMultiplier");

        floorRenderer = floorMeshObject.GetComponent<MeshRenderer>();
        floorMaterial = floorRenderer.sharedMaterial;
        floorAlpha = floorMaterial.GetFloat("_FloorAlpha");
        floorEmission = floorMaterial.GetFloat("_FloorEmission");
        floorColorAmount = floorMaterial.GetFloat("_FloorColor");
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
        //== Calculate distance between player and "morph point" (proximity will affect morph transition) ==//
        Vector3 morphpoint = _morphPointObject.transform.position;
        Vector3 playerPosition = _playerObject.transform.position;
        Vector3 playerDistanceFromMorphPoint = morphpoint - playerPosition;
        Vector2 distance2D = new Vector2(playerDistanceFromMorphPoint.x, playerDistanceFromMorphPoint.z);
        float distanceFromMorphPoint = distance2D.magnitude;
        float morphProximityNormalised = Mathf.InverseLerp(10,0, distanceFromMorphPoint); //can adjust 10 for range of the proximity
        //Debug.Log(morphProximityNormalised);
        float morphAmountNormalized = morphProximityNormalised;
        _morphParam = morphAmountNormalized * 5; // because the range of _morphParam is 0-5

        //float morphAmountNormalized = Mathf.InverseLerp(0, 5, _morphParam); // morphParam range is 0-5

        //== Switch Character Material from opaque to transparent ==//
        if (morphAmountNormalized < 0.1)
            morphRenderer.sharedMaterial = opaqueCharacterMaterial;
        if (morphAmountNormalized >= 0.1)
            morphRenderer.sharedMaterial = transparentCharacterMaterial;

        //== Switch Floor MAterial from opaque to transparent ==//
        if (morphAmountNormalized < 0.1)
            floorRenderer.sharedMaterial = opaqueFloorMaterial;
        if (morphAmountNormalized >= 0.1)
            floorRenderer.sharedMaterial = transparentFloorMaterial;

        //== Other Transitions ==//
        if (_morphParam <= 0.01)
            morphRenderer.sharedMesh = morphMeshes[0];

        if (_morphParam > 0.01f && _morphParam < 1f) 
        {
            morphRenderer.sharedMesh = morphMeshes[1];
            // lerp the "Decimate" BlendShape value between 0-1, using _morphParam (0-1)
            float decimateValue1 = Mathf.InverseLerp(0.01f, 1, _morphParam);
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
            morphRenderer.SetBlendShapeWeight(0, 100 - decimateValue2);
        }
        if (_morphParam >= 2f && _morphParam < 3f)
        {
            morphRenderer.sharedMesh = morphMeshes[3];
            // lerp the "Decimate" BlendShape value between 0-1, using _morphParam (2-3)
            float decimateValue3 = Mathf.InverseLerp(2, 3, _morphParam);
            decimateValue3 *= 100f;
            morphRenderer.SetBlendShapeWeight(0, 100 - decimateValue3);
        }
        if (_morphParam >= 3f && _morphParam < 4f)
        {
            morphRenderer.sharedMesh = morphMeshes[4];
            // lerp the "Decimate" BlendShape value between 0-1, using _morphParam (3-4)
            float decimateValue4 = Mathf.InverseLerp(3, 4, _morphParam);
            decimateValue4 *= 100f;
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
        characterMaterial = morphRenderer.sharedMaterial;
        //characterMaterial.SetColor("_Color", characterColor);
        characterColorMixer = Mathf.Clamp01( morphAmountNormalized * 4); // times 4 so it transitions within first quarter of transition (can adjust)
        characterMaterial.SetFloat("_ColorMixer", characterColorMixer);
        float fresnelAmount = Mathf.Clamp((morphAmountNormalized * 40), 0, 20); // 40 = 20 * 2 (can adjust 2 for the multiplier, to increase speed of transition)
        characterFresnelPower = 20 - fresnelAmount; // 20 is the max amount currently set in shader for the float property
        characterMaterial.SetFloat("_FresnelPower", characterFresnelPower);
        float alphaFader = Mathf.Clamp01(morphAmountNormalized * 2); // can adjust this value, it just multiplies the speed of the transition
        characterMaterial.SetFloat("_AlphaFactor", 1 - alphaFader);
        float emissionAmount = morphAmountNormalized * 5;
        characterMaterial.SetFloat("_EmissionMultiplier", emissionAmount);

        //== Floor Material (Shader Properties) ==//
        floorMaterial = floorRenderer.sharedMaterial;
        floorColorAmount = Mathf.Clamp01(morphAmountNormalized);
        floorAlpha = Mathf.Clamp01(1 - morphAmountNormalized);
        floorEmission = Mathf.Clamp01(morphAmountNormalized);
        floorMaterial.SetFloat("_FloorColor", floorColorAmount);
        floorMaterial.SetFloat("_FloorAlpha", floorAlpha);
        floorMaterial.SetFloat("_FloorEmission", floorEmission);
    }
}
