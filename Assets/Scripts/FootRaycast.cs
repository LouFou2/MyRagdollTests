using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootRaycast : MonoBehaviour
{
    private CapsuleCollider _legCapsule = null;
    private Rigidbody _rB;
    private float _capsuleHeight = 0f;
    [SerializeField] private float _springTip = 0.03f;
    [SerializeField] private float springConstant = 100f;
    private LayerMask _floorLayer;
    void Start()
    {
        _legCapsule = GetComponent<CapsuleCollider>();
        _rB = GetComponent<Rigidbody>();

        if (_legCapsule != null)
        {
            _capsuleHeight = _legCapsule.height;
        }
        _floorLayer = LayerMask.NameToLayer("Floor");
    }
    void Update()
    {
        Ray ray = new Ray(transform.position, transform.up);
        Debug.DrawRay(ray.origin, ray.direction * (_capsuleHeight + 0.1f));
        
        RaycastHit hitData;
        
        if (Physics.Raycast(ray, out hitData, (_capsuleHeight + 1), _floorLayer)) 
        {
            float _rayHitDistance = hitData.distance;
            float _overShot = _rayHitDistance - (_capsuleHeight + _springTip);

            if (_overShot < 0) 
            {
                // Calculate the upward force based on how much the object is below the spring tip
                float springForce = Mathf.Abs(_overShot) * springConstant;

                // Apply the upward force to the rigidbody
                _rB.AddForce(-transform.up * springForce);
            }
        }  
    }
}
