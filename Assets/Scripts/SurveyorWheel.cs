using UnityEngine;

public class SurveyorWheel : MonoBehaviour
{
    [SerializeField] private Transform stepController;
    [SerializeField] [Range(0,2)] private float wheelRadius; // note: the wheelradius is the distance between hip/leg joint and floor (measure height of leg)
    private Vector3 wheelScale = Vector3.zero;
    private Vector3 wheelPosition = Vector3.zero;

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
        // Set wheel scale(z and y), wheelRadius AND y position = wheelRadius
        wheelScale = transform.localScale;
        wheelScale.z = wheelRadius;
        wheelScale.y = wheelRadius;
        transform.localScale = wheelScale;

        // Set the y position to the desired value
        wheelPosition = transform.position;
        wheelPosition.y = wheelRadius;
        transform.position = wheelPosition;


        lastPosition = stepController.transform.position; // Use Global Position of parent controller
        initialPelvisTargetY = pelvisTarget.localPosition.y; // Initialize the pelvis target's Y position (need it for offset)
        // *** COULD ALSO INITIALIZE THE Y POSITION of the leg IK targets (if the targets need to be off the ground/ different from parent transform y position
        initialArmTarget_L_Y = armTarget_L.localPosition.y;
        initialArmTarget_R_Y = armTarget_L.localPosition.y; // Initialize the arm targets Y positions, need it for offset
    }

    private void Update()
    {
        //-- The Movement of the Parent Controller Object --//
        Vector3 currPosition = stepController.transform.position; // Use Global Position
        Vector2 x_z_LastPosition = new Vector2(lastPosition.x, lastPosition.z); // only x and z movement is used
        Vector2 x_z_CurrentPosition = new Vector2(currPosition.x, currPosition.z);
        Vector3 movementDirection = currPosition - lastPosition;
        Vector2 x_z_MovementDirection = x_z_CurrentPosition - x_z_LastPosition;
        float x_z_MovementAmount =  x_z_MovementDirection.magnitude;

        // Update wheel scale / radius changes (will affect speed etc)
        wheelScale = transform.localScale;
        wheelScale.z = wheelRadius;
        wheelScale.y = wheelRadius;
        transform.localScale = wheelScale;
        wheelPosition = transform.position;
        wheelPosition.y = wheelRadius;
        transform.position = wheelPosition;

        //-- The Rotation of the Surveyor Wheel --// (the idea of a surveyor wheel is: if wheel radius is 1 unit & wheel moves 1 unit, arc length = 1 unit, radians = 1 unit)
        // Radian Value = Arc Length / Radius
        float rotationAngle = (x_z_MovementAmount / wheelRadius) * Mathf.Rad2Deg;
        
        Debug.Log("Movement Amount: " + x_z_MovementAmount + ", Rads Rotation Angle: " + (x_z_MovementAmount / wheelRadius));
         
        transform.Rotate(rotationAngle, 0f, 0f, Space.Self);

        //-- The Movement of the Targets --//
        // Calculate the z position of the stepTarget point on the wheel's circumference. Sine Wave function

        float wheelLocalRotationX = transform.localRotation.eulerAngles.x * Mathf.Deg2Rad;

        float stepTargetZ_L = Mathf.Sin(wheelLocalRotationX) * wheelRadius; // Adding the Sine value
        // *** BREAK DOWN THE "FREQUENCY", "AMPLITUDE" AND "OFFSET" VALUES FOR THE SINE WAVE.
        //      frequency: should it be double-time, etc.
        //      amplitude: e.g. wheelRadius * stepWidthFactor
        //      offset: the initial position of the object (* any desired phasing factor, if needed)
        float stepTargetZ_R = -stepTargetZ_L;

        // The Arm IK targets. *** SINE OR COSINE? CHECK.
        float armTargetZ_R = Mathf.Sin(wheelLocalRotationX) * wheelRadius;
        float armTargetZ_L = -armTargetZ_R; // * note: arms R and L Z movement is opposite from legs
        armTargetY_L =  Mathf.Cos(wheelLocalRotationX) * wheelRadius;
        armTargetY_R = Mathf.Cos(wheelLocalRotationX) * wheelRadius;

        // Calculate the Pelvis Target's Y position using a cosine function
        pelvisTargetY = transform.localPosition.y - Mathf.Cos(wheelLocalRotationX * 2) * wheelRadius; // Subtracting Cosine value, because movement on y is down
        
        // Calculate the y position of the stepTarget point as the wheel turns
        // The L and the R should switch, when R is stepDown, the L moves up, and vice versa (see toggle logic below)
        // ( when the steptarget reaches each opposite end of the f-b / z movement, switch between lifting/grounded )
        if (!stepDown)
        {
            stepTargetY_L = transform.localPosition.y + Mathf.Cos(wheelLocalRotationX * 2) * wheelRadius;
            stepTargetY_R = 0f;
        }

        else if (stepDown)
        {
            stepTargetY_L = 0f;
            stepTargetY_R = transform.localPosition.y + Mathf.Cos(wheelLocalRotationX * 2) * wheelRadius;
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
        stepTargetPosition_L.z = stepTargetZ_L * stepWidthFactor; // *** TRY TO USE THE FACTOS VALUES IN THE SINE/COSINE CALCULATIONS ABOVE INSTEAD
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