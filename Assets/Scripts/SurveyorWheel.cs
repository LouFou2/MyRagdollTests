using UnityEngine;

public class SurveyorWheel : MonoBehaviour
{
    [SerializeField] private Transform stepController;
    private float wheelRotationSpeed = 360f;
    [SerializeField] private float wheelRadius; // note: the wheelradius is the distance between hip/leg joint and floor (measure height of leg)
    
    [SerializeField] private float stepLiftFactor = 0.25f;
    [SerializeField] private float stepWidthFactor = 0.25f;
    [SerializeField] private Transform stepTarget_L; // Leg IK Targets
    [SerializeField] private Transform stepTarget_R;
    
    [SerializeField] private Transform pelvisTarget; // Target to constrain pelvis/root bone
    [SerializeField] private float pelvisBounceFactor = 0.25f;
    
    [SerializeField] private Transform armTarget_L; // Arm IK Targets
    [SerializeField] private Transform armTarget_R;
    [SerializeField] private float armSwingFactor = 0.25f;
    [SerializeField] private float armLifFactor = 0.1f;


    private float stepTargetY_L;
    private float stepTargetY_R;
    private float armTargetY_L; // Y float values for the Arm IK targets 
    private float armTargetY_R;
    private float initialArmTarget_L_Y;
    private float initialArmTarget_R_Y;
    private float pelvisTargetY;
    private float initialPelvisTargetY;
    private bool withinThreshold = false; //this threshold is used for switching y movement of ste between grounded and lifting
    private bool stepDown = false;
    private Vector3 lastPosition;
    private Vector3 lastStepPosition_L;
    private Vector3 lastStepPosition_R;

    void Start()
    {
        lastPosition = stepController.transform.position; // Use Global Position of parent controller
        initialPelvisTargetY = pelvisTarget.localPosition.y; // Initialize the pelvis target's Y position (need it for offset)
        // *** COULD ALSO INITIALIZE THE Y POSITION of the leg IK targets (if the targets need to be off the ground/ different from parent transform y position
        initialArmTarget_L_Y = armTarget_L.localPosition.y;
        initialArmTarget_R_Y = armTarget_L.localPosition.y; // Initialize the arm targets Y positions, need it for offset
    }

    private void Update()
    {
        //-- The Movement of the Wheel Itself --//
        Vector3 currPosition = stepController.transform.position; // Use Global Position
        Vector3 movementDirection = currPosition - lastPosition;

        float rotationAngle = movementDirection.magnitude * wheelRotationSpeed;
        transform.Rotate(rotationAngle, 0f, 0f, Space.Self);

        //-- The Movement of the Targets --//
        // Calculate the z position of the stepTarget point on the wheel's circumference. Sine Wave function
        float stepTargetZ_L = transform.localPosition.z + Mathf.Sin(transform.localRotation.eulerAngles.x * Mathf.Deg2Rad) * wheelRadius; // Adding the Sine value
        float stepTargetZ_R = -stepTargetZ_L;

        // The Arm IK targets. *** SINE OR COSINE? CHECK.
        float armTargetZ_R = transform.localPosition.z + Mathf.Sin(transform.localRotation.eulerAngles.x * Mathf.Deg2Rad) * wheelRadius;
        float armTargetZ_L = -armTargetZ_R; // * note: arms R and L Z movement is opposite from legs
        armTargetY_L = transform.localPosition.z + Mathf.Cos(transform.localRotation.eulerAngles.x * Mathf.Deg2Rad) * wheelRadius;
        armTargetY_R = transform.localPosition.z + Mathf.Cos(transform.localRotation.eulerAngles.x * Mathf.Deg2Rad) * wheelRadius;

        // Calculate the Pelvis Target's Y position using a cosine function
        pelvisTargetY = transform.localPosition.y - Mathf.Cos(transform.localRotation.eulerAngles.x * Mathf.Deg2Rad * 2) * wheelRadius; // Subtracting Cosine value, because movement on y is down

        // Calculate the y position of the stepTarget point as the wheel turns
        // The L and the R should switch, when R is stepDown, the L moves up, and vice versa (see toggle logic below)
        // ( when the steptarget reaches each opposite end of the f-b / z movement, switch between lifting/grounded )
        if (!stepDown) 
        {
            stepTargetY_L = transform.localPosition.y + Mathf.Cos(transform.localRotation.eulerAngles.x * Mathf.Deg2Rad * 2) * wheelRadius;
            stepTargetY_R = 0f;
        }

        else if (stepDown)
        {
            stepTargetY_L = 0f;
            stepTargetY_R = transform.localPosition.y + Mathf.Cos(transform.localRotation.eulerAngles.x * Mathf.Deg2Rad * 2) * wheelRadius;
        }

        // Setup needed vars for 'stepDown toggle'
        float adjustedWheelRadius = wheelRadius * stepWidthFactor; //adjust the max/min range based on the actual step width
        float threshold = adjustedWheelRadius * 0.98f; // set up a threshold, because sine/cosine values don't reach max/min predicatably
        Vector2 horizontalVector2 = new Vector2(lastStepPosition_L.x, lastStepPosition_L.z); //only measure horizontal x/z movement
        float prevStepTargetMagnitude = horizontalVector2.magnitude;

        // Toggle to switch 'stepDown'
        if (prevStepTargetMagnitude >= threshold && withinThreshold == false)
        {
            stepDown = !stepDown;

            withinThreshold = true;
        }
        else if (prevStepTargetMagnitude < threshold)
        {
            withinThreshold = false;
        }

        // Set the stepTarget's z position
        Vector3 stepTargetPosition_L = stepTarget_L.localPosition;
        Vector3 stepTargetPosition_R = stepTarget_R.localPosition;

        //-- Update Z Position of the StepTarget
        stepTargetPosition_L.z = stepTargetZ_L * stepWidthFactor;
        stepTargetPosition_R.z = stepTargetZ_R * stepWidthFactor;

        // The Y position is more complicated:
        if (movementDirection.magnitude == 0f)
        {
            stepTargetPosition_L.y = lastStepPosition_L.y;
            stepTargetPosition_R.y = lastStepPosition_R.y;
        }
        else
        {
            stepTargetPosition_L.y = stepTargetY_L * stepLiftFactor;
            stepTargetPosition_R.y = stepTargetY_R * stepLiftFactor;
        }

        stepTarget_L.localPosition = stepTargetPosition_L;
        stepTarget_R.localPosition = stepTargetPosition_R;

        // record the step position for the next update (for calculating y movement of step target)
        lastStepPosition_L = stepTarget_L.localPosition; 
        lastStepPosition_R = stepTarget_R.localPosition;

        // Set the Y position of the pelvis target
        Vector3 pelvisTargetPosition = pelvisTarget.localPosition;
        pelvisTargetPosition.y = initialPelvisTargetY - pelvisTargetY * pelvisBounceFactor; // pay attention to addition vs subtraction for values
        pelvisTarget.localPosition = pelvisTargetPosition;

        // Set the armTargets z AND y positions
        Vector3 armTargetPosition_L = armTarget_L.localPosition;
        Vector3 armTargetPosition_R = armTarget_R.localPosition;
        armTargetPosition_L.z = armTargetZ_L * armSwingFactor; // Z movement
        armTargetPosition_R.z = armTargetZ_R * armSwingFactor;
        armTargetPosition_L.y = initialArmTarget_L_Y + armTargetY_L * armLifFactor; // Y movement 
        armTargetPosition_R.y = initialArmTarget_R_Y + armTargetY_R * armLifFactor; // ***  ADDING OR SUBTRACTING?
        armTarget_L.localPosition = armTargetPosition_L;
        armTarget_R.localPosition = armTargetPosition_R;

        //-- Update Position of the Wheel
        lastPosition = currPosition;
    }
}