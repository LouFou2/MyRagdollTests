using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetRaycast : MonoBehaviour
{
    private Rigidbody _rB;
    [SerializeField] private float _targetMovementThreshold = 0.001f; // Threshold for when spring force can kick in
    [SerializeField] private float _springTip = 0.01f;
    [SerializeField] private float _springConstant = 100f;
    [SerializeField] private float _dampingFactor = 0.5f;
    private LayerMask _floorLayer;
    private bool _targetMoving = false;
    private Vector3 _previousPosition;
    private Vector3 _currentPosition;


    void Start()
    {
        _rB = GetComponent<Rigidbody>();

        _floorLayer = LayerMask.NameToLayer("Floor");

        _previousPosition = _rB.position;
}

    void FixedUpdate()
    {
        _currentPosition = _rB.position;

        Ray ray = new Ray(transform.position, -transform.up);
        Debug.DrawRay(ray.origin, ray.direction * 1);

        // Calculate velocity manually
        Vector3 objectVelocity = (_currentPosition - _previousPosition) / Time.fixedDeltaTime;
        _targetMoving = objectVelocity.magnitude > 0;// _targetMovementThreshold;

        RaycastHit hitData;

        if (Physics.Raycast(ray, out hitData, 1, _floorLayer))
        {
            float rayHitDistance = hitData.distance;
            Debug.Log(rayHitDistance);
            float overShot = rayHitDistance - _springTip;
            
            if (overShot < 0 && _targetMoving)
            {
                Debug.Log("Apply Spring");

                // Calculate the upward force based on how much the object is below the spring tip
                float springForce = Mathf.Abs(overShot) * _springConstant;

                /*float relativeVelocity = Vector3.Dot(_rB.velocity, -transform.up);
                springForce *= (1 - _dampingFactor * Mathf.Abs(relativeVelocity));*/

                // Apply the upward force to the rigidbody
                //_rB.AddForce(transform.up * springForce);

                // Manually move the object in the upward direction
                Vector3 forceDirection = transform.up; // or any desired direction
                float forceMagnitude = springForce;// * Time.fixedDeltaTime; // Apply force over time

                // Apply the force to move the object manually
                _rB.position += forceDirection * forceMagnitude;
            }
        }

        _previousPosition = _currentPosition;
    }

}
