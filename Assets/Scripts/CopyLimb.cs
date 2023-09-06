using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class CopyLimb : MonoBehaviour
{
    [SerializeField] private Transform targetLimb;
    private ConfigurableJoint m_ConfigurableJoint;
    private Quaternion targetInitialRotation;
    private Vector3 targetInitialPosition;

    private JointDrive originalXDrive;
    private JointDrive originalYDrive;
    private JointDrive originalZDrive;
    private JointDrive originalAngularXDrive;
    private JointDrive originalAngularYZDrive; // joint drive values to be scaled by 'ragdollPowerFactor'
    private JointDrive scaledXDrive;
    private JointDrive scaledYDrive;
    private JointDrive scaledZDrive;
    private JointDrive scaledAngularXDrive;
    private JointDrive scaledAngularYZDrive; // the scaled drive values

    [SerializeField] 
    private bool ragdollSwitchEnabled = true; // New boolean for switching ragdoll behavior
    [SerializeField] private float ragdollPowerMin = 0f;
    private float ragdollPowerMax = 1f;
    [SerializeField]
    [Range(0f, 1f)] private float ragdollPowerFactor = 1f; 
    [SerializeField] private bool powerX = false;
    [SerializeField] private bool powerY = false;
    [SerializeField] private bool powerZ = false;
    [SerializeField] private bool powerAngularX = true;
    [SerializeField] private bool powerAngularYZ = true;
    [SerializeField]
    [Range(0f,1f)] private float positionCopyAmount = 0f;
    void Start()
    {
        // Set up the configurable joint that will copy the target rotation
        this.m_ConfigurableJoint = GetComponent<ConfigurableJoint>();
        
        // Record the target's intitial rotation
        this.targetInitialRotation = this.targetLimb.transform.localRotation;

        // Record the target's intitial position
        this.targetInitialPosition = this.targetLimb.localPosition;

        // Store the original driving values
        originalXDrive = m_ConfigurableJoint.xDrive;
        originalYDrive = m_ConfigurableJoint.yDrive;
        originalZDrive = m_ConfigurableJoint.zDrive;
        originalAngularXDrive = m_ConfigurableJoint.angularXDrive;
        originalAngularYZDrive = m_ConfigurableJoint.angularYZDrive;

        // Initialize the scaled driving values
        scaledXDrive = ScaleDrive(originalXDrive, ragdollPowerFactor);
        scaledYDrive = ScaleDrive(originalYDrive, ragdollPowerFactor);
        scaledZDrive = ScaleDrive(originalZDrive, ragdollPowerFactor);
        scaledAngularXDrive = ScaleDrive(originalAngularXDrive, ragdollPowerFactor);
        scaledAngularYZDrive = ScaleDrive(originalAngularYZDrive, ragdollPowerFactor);
    }

    private void FixedUpdate()
    {
        // Map ragdollPowerFactor to the ragdollPowerMin - ragdollPowerMax range
        float ragdollPowerRange = ragdollPowerMax - ragdollPowerMin;
        float mappedRagdollPower = ragdollPowerFactor * ragdollPowerRange + ragdollPowerMin;

        if (ragdollSwitchEnabled)
        {
            this.m_ConfigurableJoint.targetRotation = copyRotation();
            this.m_ConfigurableJoint.targetPosition = copyPosition();

            // Update scaled driving values based on current ragdollPowerFactor and individual power booleans
            scaledXDrive = powerX ? ScaleDrive(originalXDrive, mappedRagdollPower) : originalXDrive;
            scaledYDrive = powerY ? ScaleDrive(originalYDrive, mappedRagdollPower) : originalYDrive;
            scaledZDrive = powerZ ? ScaleDrive(originalZDrive, mappedRagdollPower) : originalZDrive;
            scaledAngularXDrive = powerAngularX ? ScaleDrive(originalAngularXDrive, mappedRagdollPower) : originalAngularXDrive;
            scaledAngularYZDrive = powerAngularYZ ? ScaleDrive(originalAngularYZDrive, mappedRagdollPower) : originalAngularYZDrive;

            // Apply the scaled driving values to the configurable joint
            this.m_ConfigurableJoint.xDrive = scaledXDrive;
            this.m_ConfigurableJoint.yDrive = scaledYDrive;
            this.m_ConfigurableJoint.zDrive = scaledZDrive;
            this.m_ConfigurableJoint.angularXDrive = scaledAngularXDrive;
            this.m_ConfigurableJoint.angularYZDrive = scaledAngularYZDrive;
        }

        else
        {
            // Copy transforms to GameObject directly
            transform.localPosition = targetLimb.localPosition;
            transform.localRotation = targetLimb.localRotation;
        }
    }

    private Quaternion copyRotation() 
    {
        return Quaternion.Inverse(this.targetLimb.localRotation) * this.targetInitialRotation;
    }

    private Vector3 copyPosition()
    {
        Vector3 positionDifference = this.targetInitialPosition -this.targetLimb.localPosition;
        return positionDifference * positionCopyAmount;
    }
    private JointDrive ScaleDrive(JointDrive originalDrive, float ragdollPowerFactor)
    {
        JointDrive scaledDrive = originalDrive;
        scaledDrive.positionSpring *= ragdollPowerFactor;
        //scaledDrive.positionDamper *= ragdollPowerFactor;
        //scaledDrive.maximumForce *= ragdollPowerFactor;
        return scaledDrive;
    }
}
