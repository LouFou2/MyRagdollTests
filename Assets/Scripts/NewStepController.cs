using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewStepController : MonoBehaviour
{
    private Vector3 initialPosition = Vector3.zero;
    private Vector3 lastPosition = Vector3.zero;
    private Vector3 currentPosition = Vector3.zero;
    private float HorizontalMoveDistance = 0f;

    [SerializeField] private GameObject horizontalMover;
    [SerializeField] private GameObject verticalMover;

    void Start()
    {
        initialPosition = transform.position; // just in case we need it?
        lastPosition = initialPosition;
    }

    void Update()
    {
        //-- The movement of the controlling object --//
        currentPosition = transform.position;
        Vector2 horizontalLastPosition = new Vector2(lastPosition.z, lastPosition.x);           // only using x z movement for calculating horizontal movement
        Vector2 horizontalCurrentPosition = new Vector2(currentPosition.z, currentPosition.x);  // important: z is forward (same as x axis for 2d), so z is first
        // Calculate HorizontalMoveDistance using magnitude
        HorizontalMoveDistance += (horizontalCurrentPosition - horizontalLastPosition).magnitude;
        Debug.Log(HorizontalMoveDistance);

        /*if (HorizontalMoveDistance >= horizontalMoverAmplitude * 4) // * 4 to allow for full wave cycle (up-down,down-up)
        { HorizontalMoveDistance = 0; }*/
        //***HAVE TO FIND A GOOD WAY TO RESET THE HorizontalMoveDistance WHEN WAVE CYCLES ARE COMPLETED

        //-- Sine Waves: to be referenced by other objects for movement --//
        float verticalSine = Mathf.Sin(HorizontalMoveDistance);
        float horizontalSine = Mathf.Sin(HorizontalMoveDistance);   // sine value starts at the center of the amplitude range.

        //-- Cosine Waves: to be referenced by other objects for movement --//
        float verticalCosine = Mathf.Cos(HorizontalMoveDistance);
        float horizontalCosine = Mathf.Cos(HorizontalMoveDistance);   // cosine value starts at the max of the amplitude range.

        Vector3 verticalMoverVector = verticalMover.transform.localPosition;
        Vector3 addedVerticalSineMovement = new Vector3(verticalMoverVector.x, verticalSine, verticalMoverVector.z); //added the sine value on the y
        verticalMover.transform.localPosition = addedVerticalSineMovement;

        Vector3 horizontalMoverVector = horizontalMover.transform.localPosition;
        Vector3 addedHorizontalSineMovement = new Vector3(horizontalMoverVector.x, horizontalMoverVector.y, horizontalSine); //added cosine value on z (could be x too)
        horizontalMover.transform.localPosition = addedHorizontalSineMovement;

        // To move another object in a horizontal sine movement:
        // float otherObject_H_Sine = horizontalSine * otherObject_H_Amplitude;
        // ...some calculations
        // otherObject.transform.localPosition = ...

        // To move another object in a vertical sine movement:
        // float otherObject_V_Sine = verticalSine * otherObject_V_Amplitude;
        // ...some calculations
        // otherObject.transform.localPosition = ...

        // -- *     THIS HAS TO STAY AT THE END * -- //
        // Update lastPosition for the next frame
        lastPosition = currentPosition;
    }
}
