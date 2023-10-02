using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootRaycast : MonoBehaviour
{
    private CapsuleCollider _legCapsule = null;
    private Rigidbody _rB;
    private float _capsuleHeight = 0f;
    [SerializeField] private Transform _targetObject; // Reference to the target limb's transform (spring force is only used when limb moves)
    [SerializeField] private float _targetMovementThreshold = 0.001f; // Threshold for when spring force can kick in
    [SerializeField] private float _springTip = 0.03f;
    [SerializeField] private float _springConstant = 100f;
    [SerializeField] private float _dampingFactor = 0.5f;
    private LayerMask _floorLayer;
    private Vector3 _targetCurrentPosition = Vector3.zero;
    private Vector3 _targetPreviousPosition = Vector3.zero;

    
    void Start()
    {
        _legCapsule = GetComponent<CapsuleCollider>();
        _rB = GetComponent<Rigidbody>();

        _targetPreviousPosition = _targetObject.position;

        if (_legCapsule != null)
        {
            _capsuleHeight = _legCapsule.height;
        }
        _floorLayer = LayerMask.NameToLayer("Floor");
    }
    private void Update()
    {

    }
    void FixedUpdate()
    {
        _targetCurrentPosition = _targetObject.position;

        Ray ray = new Ray(transform.position, transform.up);
        Debug.DrawRay(ray.origin, ray.direction * (_capsuleHeight + 0.1f));
        
        RaycastHit hitData;
        
        if (Physics.Raycast(ray, out hitData, (_capsuleHeight + 1), _floorLayer)) 
        {
            float rayHitDistance = hitData.distance;
            float overShot = rayHitDistance - (_capsuleHeight + _springTip);
            bool targetMoving = Vector3.Distance(_targetCurrentPosition, _targetPreviousPosition) > 0; // You can adjust the threshold value as needed
            if (targetMoving)
            {
                Debug.Log("Target moving");
            }

            if (overShot < 0 && targetMoving) 
            {
                // Calculate the upward force based on how much the object is below the spring tip
                float springForce = Mathf.Abs(overShot) * _springConstant;

                /*float relativeVelocity = Vector3.Dot(_rB.velocity, -transform.up);
                springForce *= (1 - _dampingFactor * Mathf.Abs(relativeVelocity));*/

                // Apply the upward force to the rigidbody
                _rB.AddForce(-transform.up * springForce); // -transfrom.up because the object is actually upside-down in this case
            }
        }
        _targetPreviousPosition = _targetCurrentPosition;
    }

}
