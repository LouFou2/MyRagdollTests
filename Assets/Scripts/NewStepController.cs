using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewStepController : MonoBehaviour
{
    private Vector3 initialPosition = Vector3.zero;
    private Vector3 lastPosition = Vector3.zero;
    private Vector3 currentPosition = Vector3.zero;
    private float HorizontalMoveDistance = 0f;

    [SerializeField] private GameObject horizontalSineMover;
    [SerializeField] private GameObject verticalSineMover;
    [SerializeField] private GameObject horizontalCosineMover;
    [SerializeField] private GameObject verticalCosineMover;

    [SerializeField] private GameObject RotationVisualiser;
    private float fullDegreesRotaion = 0f;

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
        float horizontalDistanceMovedInUpdate = (horizontalCurrentPosition - horizontalLastPosition).magnitude; ;
        HorizontalMoveDistance += horizontalDistanceMovedInUpdate;
        Debug.Log(HorizontalMoveDistance);

        //-- Sine Waves: to be referenced by other objects for movement --//
        float movementSineValue = Mathf.Sin(HorizontalMoveDistance);    // Sine value starts at the middle of the amplitude range.

        //-- Cosine Waves: to be referenced by other objects for movement --//
        float movementCosineValue = Mathf.Cos(HorizontalMoveDistance);  // cosine value starts at the max of the amplitude range.

        //-- Rotation Visualisation --//
        float degreesRotation = horizontalDistanceMovedInUpdate * Mathf.Rad2Deg;
        fullDegreesRotaion += degreesRotation;
        RotationVisualiser.transform.Rotate(degreesRotation, 0f, 0f, Space.Self);
        Debug.Log(fullDegreesRotaion);
        if ( fullDegreesRotaion >= 360 ) { 
            HorizontalMoveDistance = 0;
            fullDegreesRotaion = 0f;
        }

        //-- Examples: Sine / Cosine Movement
        Vector3 verticalSineMoverVector = verticalSineMover.transform.localPosition;
        Vector3 addedVerticalSineMovement = new Vector3(verticalSineMoverVector.x, movementSineValue, verticalSineMoverVector.z); //added the sine value on the y
        verticalSineMover.transform.localPosition = addedVerticalSineMovement;

        Vector3 horizontalSineMoverVector = horizontalSineMover.transform.localPosition;
        Vector3 addedHorizontalSineMovement = new Vector3(horizontalSineMoverVector.x, horizontalSineMoverVector.y, movementSineValue); //added cosine value on z (could be x too)
        horizontalSineMover.transform.localPosition = addedHorizontalSineMovement;

        Vector3 verticalCosineMoverVector = verticalCosineMover.transform.localPosition;
        Vector3 addedVerticalCosineMovement = new Vector3(verticalCosineMoverVector.x, movementCosineValue, verticalCosineMoverVector.z); //added the sine value on the y
        verticalCosineMover.transform.localPosition = addedVerticalCosineMovement;

        Vector3 horizontalCosineMoverVector = horizontalCosineMover.transform.localPosition;
        Vector3 addedHorizontalCosineMovement = new Vector3(horizontalCosineMoverVector.x, horizontalCosineMoverVector.y, movementCosineValue); //added cosine value on z (could be x too)
        horizontalCosineMover.transform.localPosition = addedHorizontalCosineMovement;

        // To move another object using sine/cosine movement:
        // e.g:
        // float otherObject_H_Sine = horizontalSine * otherObject_H_Amplitude; // use a specific amplitude appropriate for object's movement
        // ...some calculations
        // otherObject.transform.localPosition = ...



        // -- *     THIS HAS TO STAY AT THE END * -- //
        // Update lastPosition for the next frame
        lastPosition = currentPosition;
    }
}
