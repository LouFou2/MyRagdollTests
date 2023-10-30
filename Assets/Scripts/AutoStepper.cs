using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoStepper : MonoBehaviour
{
    private float HorizontalMoveDistance = 0f;
    [SerializeField] private float _moveSpeed = 1f;

    //== The 1-Unit Objects, used to control other Target Objects ==//
    [Header("1-Unit Control Objects")]
    [SerializeField] private GameObject _horizontalSineMover;
    [SerializeField] private GameObject _verticalSineMover;
    [SerializeField] private GameObject _horizontalCosineMover;
    [SerializeField] private GameObject _verticalCosineMover;
    [SerializeField] private GameObject _RotationVisualiser;
    private float fullDegreesRotation = 0f;

    //== Target Objects ==//
    [Header("Target Objects")]
    [SerializeField] private GameObject _coreMover;
    [SerializeField][Range(0f, 1f)] private float _core_X_MoveFactor = 0.5f;
    [SerializeField][Range(0f, 1f)] private float _core_Z_MoveFactor = 0.5f;
    [SerializeField][Range(0f, 1f)] private float _core_Y_MoveFactor = 0.5f;

    [SerializeField] private GameObject _hipAimObject;
    [SerializeField][Range(0f, 1f)] private float _hipAim_X_MoveFactor = 0.5f;
    [SerializeField][Range(0f, 1f)] private float _hipAim_Z_MoveFactor = 0.5f;
    [SerializeField][Range(0f, 1f)] private float _hipAim_Y_MoveFactor = 0.5f;

    [SerializeField] private GameObject _shoulderAimObject;
    [SerializeField][Range(0f, 1f)] private float _shoulderAim_X_MoveFactor = 0.5f;
    [SerializeField][Range(0f, 1f)] private float _shoulderAim_Y_MoveFactor = 0.5f;
    [SerializeField] private float _shoulderAim_Y_Offset = 0.5f;

    [SerializeField] private GameObject _headMover;
    [SerializeField][Range(0f, 1f)] private float _head_X_MoveFactor = 0.5f;
    [SerializeField][Range(0f, 1f)] private float _head_Z_MoveFactor = 0.5f;
    [SerializeField][Range(0f, 1f)] private float _head_Y_MoveFactor = 0.5f;

    [SerializeField] private GameObject _armL_Mover;
    [SerializeField][Range(0f, 1f)] private float _armL_X_MoveFactor = 0.5f;
    [SerializeField][Range(0f, 1f)] private float _armL_Z_MoveFactor = 0.5f;
    [SerializeField][Range(0f, 1f)] private float _armL_Y_MoveFactor = 0.5f;
    [SerializeField] private float _armL_X_Offset = 0f;
    [SerializeField] private float _armL_Y_Offset = 0f;

    [SerializeField] private GameObject _armR_Mover;
    [SerializeField][Range(0f, 1f)] private float _armR_X_MoveFactor = 0.5f;
    [SerializeField][Range(0f, 1f)] private float _armR_Z_MoveFactor = 0.5f;
    [SerializeField][Range(0f, 1f)] private float _armR_Y_MoveFactor = 0.5f;
    [SerializeField] private float _armR_X_Offset = 0f;
    [SerializeField] private float _armR_Y_Offset = 0f;

    [SerializeField] private GameObject _handL_Mover;
    [SerializeField][Range(0f, 1f)] private float _handL_X_MoveFactor = 0.5f;
    [SerializeField][Range(0f, 1f)] private float _handL_Z_MoveFactor = 0.5f;
    [SerializeField][Range(0f, 1f)] private float _handL_Y_MoveFactor = 0.5f;
    [SerializeField] private float _handL_X_Offset = 0f;

    [SerializeField] private GameObject _handR_Mover;
    [SerializeField][Range(0f, 1f)] private float _handR_X_MoveFactor = 0.5f;
    [SerializeField][Range(0f, 1f)] private float _handR_Z_MoveFactor = 0.5f;
    [SerializeField][Range(0f, 1f)] private float _handR_Y_MoveFactor = 0.5f;
    [SerializeField] private float _handR_X_Offset = 0f;

    [SerializeField] private GameObject _legL_Mover;
    [SerializeField][Range(0f, 1f)] private float _legL_X_MoveFactor = 0.5f;
    [SerializeField][Range(0f, 1f)] private float _legL_Z_MoveFactor = 0.5f;
    [SerializeField][Range(0f, 1f)] private float _legL_Y_MoveFactor = 0.5f;
    private bool legFrozenX_L = false;

    [SerializeField] private GameObject _legR_Mover;
    [SerializeField][Range(0f, 1f)] private float _legR_X_MoveFactor = 0.5f;
    [SerializeField][Range(0f, 1f)] private float _legR_Z_MoveFactor = 0.5f;
    [SerializeField][Range(0f, 1f)] private float _legR_Y_MoveFactor = 0.5f;
    private bool legFrozenX_R = false;

    [SerializeField] private GameObject _toeL_Mover;
    [SerializeField][Range(0f, 1f)] private float _toeL_X_MoveFactor = 0.5f;
    [SerializeField][Range(0f, 1f)] private float _toeL_Z_MoveFactor = 0.5f;
    [SerializeField][Range(0f, 1f)] private float _toeL_Y_MoveFactor = 0.5f;
    [SerializeField] private float _toeL_X_Offset = 0f;
    [SerializeField] private float _toeL_Z_Offset = 0f;

    [SerializeField] private GameObject _toeR_Mover;
    [SerializeField][Range(0f, 1f)] private float _toeR_X_MoveFactor = 0.5f;
    [SerializeField][Range(0f, 1f)] private float _toeR_Z_MoveFactor = 0.5f;
    [SerializeField][Range(0f, 1f)] private float _toeR_Y_MoveFactor = 0.5f;
    [SerializeField] private float _toeR_X_Offset = 0f;
    [SerializeField] private float _toeR_Z_Offset = 0f;


    void Update()
    {
         // Calculate HorizontalMoveDistance using magnitude
        float horizontalDistanceMovedInUpdate = Time.deltaTime * _moveSpeed;
        HorizontalMoveDistance += horizontalDistanceMovedInUpdate;
        Debug.Log(HorizontalMoveDistance);

        //-- Sine Waves: to be referenced by other objects for movement --//
        float movementSineValue = Mathf.Sin(HorizontalMoveDistance);    // Sine value starts at the middle of the amplitude range.
        float movementSineDoubleFreq = Mathf.Sin(2* HorizontalMoveDistance);

        //-- Cosine Waves: to be referenced by other objects for movement --//
        float movementCosineValue = Mathf.Cos(HorizontalMoveDistance);  // cosine value starts at the max of the amplitude range.
        float movementCosineDoubleFreq = Mathf.Cos(2 * HorizontalMoveDistance);

        //-- Rotation Visualisation --//
        float degreesRotation = horizontalDistanceMovedInUpdate * Mathf.Rad2Deg;
        fullDegreesRotation += degreesRotation;
        _RotationVisualiser.transform.Rotate(degreesRotation, 0f, 0f, Space.Self);
        Debug.Log("degreesRotation: "+ fullDegreesRotation);
        if (fullDegreesRotation >= 360)
        {
            HorizontalMoveDistance = 0;
            fullDegreesRotation = 0f;
        }

        //-- Examples: Sine / Cosine Movement
        Vector3 ySineMoverVector = _verticalSineMover.transform.localPosition;
        Vector3 added_Y_SineMovement = new Vector3(ySineMoverVector.x, movementSineValue, ySineMoverVector.z); //added the sine value on the y
        Vector3 addedDouble_Y_SineMovement = new Vector3(ySineMoverVector.x, movementSineDoubleFreq, ySineMoverVector.z);
        _verticalSineMover.transform.localPosition = added_Y_SineMovement;

        Vector3 zSineMoverVector = _horizontalSineMover.transform.localPosition;
        Vector3 added_Z_SineMovement = new Vector3(zSineMoverVector.x, zSineMoverVector.y, movementSineValue); //added cosine value on z
        Vector3 addedDouble_Z_SineMovement = new Vector3(zSineMoverVector.x, zSineMoverVector.y, movementSineDoubleFreq);
        _horizontalSineMover.transform.localPosition = added_Z_SineMovement;

        Vector3 xSineMoverVector = _horizontalSineMover.transform.localPosition;
        Vector3 added_X_SineMovement = new Vector3(movementSineValue, xSineMoverVector.y, xSineMoverVector.z); //added cosine value on x
        Vector3 addedDouble_X_SineMovement = new Vector3(movementSineDoubleFreq, xSineMoverVector.y, xSineMoverVector.z);
        _horizontalSineMover.transform.localPosition = added_X_SineMovement;

        Vector3 yCosineMoverVector = _verticalCosineMover.transform.localPosition;
        Vector3 added_Y_CosineMovement = new Vector3(yCosineMoverVector.x, movementCosineValue, yCosineMoverVector.z); //added the cosine value on the y
        Vector3 addedDouble_Y_CosineMovement = new Vector3(yCosineMoverVector.x, movementCosineDoubleFreq, yCosineMoverVector.z);
        _verticalCosineMover.transform.localPosition = added_Y_CosineMovement;

        Vector3 zCosineMoverVector = _horizontalCosineMover.transform.localPosition;
        Vector3 added_Z_CosineMovement = new Vector3(zCosineMoverVector.x, zCosineMoverVector.y, movementCosineValue); //added cosine value on z (could be x too)
        Vector3 addedDouble_Z_CosineMovement = new Vector3(zCosineMoverVector.x, zCosineMoverVector.y, movementCosineDoubleFreq);
        _horizontalCosineMover.transform.localPosition = added_Z_CosineMovement;

        Vector3 xCosineMoverVector = _horizontalCosineMover.transform.localPosition;
        Vector3 added_X_CosineMovement = new Vector3(movementCosineValue, xCosineMoverVector.y, xCosineMoverVector.z); //added cosine value on z (could be x too)
        Vector3 addedDouble_X_CosineMovement = new Vector3(movementCosineDoubleFreq, xCosineMoverVector.y, xCosineMoverVector.z);
        _horizontalCosineMover.transform.localPosition = added_X_CosineMovement;

        //== Calling Methods to Move and Rotate Target Objects ==//
        MoveCore(_coreMover, added_X_SineMovement, added_Z_SineMovement, addedDouble_Y_CosineMovement, _core_X_MoveFactor, _core_Z_MoveFactor, _core_Y_MoveFactor);
        RotateCore(_hipAimObject, added_X_SineMovement, added_Z_SineMovement, addedDouble_Y_CosineMovement, _hipAim_X_MoveFactor, _hipAim_Z_MoveFactor, _hipAim_Y_MoveFactor);
        RotateShoulders(_shoulderAimObject, added_X_SineMovement, addedDouble_Y_CosineMovement, _shoulderAim_X_MoveFactor, _shoulderAim_Y_MoveFactor);
        MoveHead(_headMover, added_X_SineMovement, added_Z_SineMovement, addedDouble_Y_CosineMovement, _head_X_MoveFactor, _head_Z_MoveFactor, _head_Y_MoveFactor);
        MoveArmL(_armL_Mover, addedDouble_X_CosineMovement, added_Z_SineMovement, addedDouble_Y_CosineMovement, _armL_X_MoveFactor, _armL_Z_MoveFactor, _armL_Y_MoveFactor);
        MoveArmR(_armR_Mover, addedDouble_X_CosineMovement, added_Z_SineMovement, addedDouble_Y_CosineMovement, _armR_X_MoveFactor, _armR_Z_MoveFactor, _armR_Y_MoveFactor);
        MoveHandL(_handL_Mover, addedDouble_X_CosineMovement, added_Z_SineMovement, addedDouble_Y_CosineMovement, _handL_X_MoveFactor, _handL_Z_MoveFactor, _handL_Y_MoveFactor);
        MoveHandR(_handR_Mover, addedDouble_X_CosineMovement, added_Z_SineMovement, addedDouble_Y_CosineMovement, _handR_X_MoveFactor, _handR_Z_MoveFactor, _handR_Y_MoveFactor);
        MoveLegL(_legL_Mover, addedDouble_X_CosineMovement, added_Z_SineMovement, added_Y_CosineMovement, _legL_X_MoveFactor, _legL_Z_MoveFactor, _legL_Y_MoveFactor);
        MoveLegR(_legR_Mover, addedDouble_X_CosineMovement, added_Z_SineMovement, added_Y_CosineMovement, _legR_X_MoveFactor, _legR_Z_MoveFactor, _legR_Y_MoveFactor);
        MoveToeL(_toeL_Mover, addedDouble_X_CosineMovement, added_Z_SineMovement, added_Y_CosineMovement, _toeL_X_MoveFactor, _toeL_Z_MoveFactor, _toeL_Y_MoveFactor);
        MoveToeR(_toeR_Mover, addedDouble_X_CosineMovement, added_Z_SineMovement, added_Y_CosineMovement, _toeR_X_MoveFactor, _toeR_Z_MoveFactor, _toeR_Y_MoveFactor);
    }
    void MoveCore(GameObject moverObject, Vector3 xMovement, Vector3 zMovement, Vector3 yMovement, float moveAmountX, float moveAmount_Z, float moveAmount_Y)
    {
        Vector3 position = moverObject.transform.localPosition;
        moverObject.transform.localPosition = new Vector3(-(xMovement.x * moveAmountX), yMovement.y * moveAmount_Y, zMovement.z * moveAmount_Z);
    }
    void RotateCore(GameObject moverObject, Vector3 xMovement, Vector3 zMovement, Vector3 yMovement, float moveAmountX, float moveAmount_Z, float moveAmount_Y) 
    {
        Vector3 position = moverObject.transform.localPosition;
        moverObject.transform.localPosition = new Vector3((xMovement.x * moveAmountX), yMovement.y * moveAmount_Y, zMovement.z * moveAmount_Z);
    }
    void RotateShoulders(GameObject moverObject, Vector3 xMovement, Vector3 yMovement, float moveAmountX, float moveAmount_Y)
    {
        Vector3 position = moverObject.transform.localPosition;
        moverObject.transform.localPosition = new Vector3(-(xMovement.x * moveAmountX), (yMovement.y * moveAmount_Y) + _shoulderAim_Y_Offset, position.z);
    }
    
    void MoveHead(GameObject moverObject, Vector3 xMovement, Vector3 zMovement, Vector3 verticalMovement, float moveAmountX, float moveAmount_Z, float moveAmount_Y)
    {
        Vector3 position = moverObject.transform.localPosition;
        moverObject.transform.localPosition = new Vector3(-(xMovement.x * moveAmountX), verticalMovement.y * moveAmount_Y, zMovement.z * moveAmount_Z);
    }
    void MoveArmL(GameObject moverObject, Vector3 xMovement, Vector3 zMovement, Vector3 verticalMovement, float moveAmountX, float moveAmount_Z, float moveAmount_Y)
    {
        
        Vector3 position = moverObject.transform.localPosition;
        moverObject.transform.localPosition = new Vector3(-(xMovement.x * moveAmountX) + _armL_X_Offset, -(verticalMovement.y * moveAmount_Y) + _armL_Y_Offset, -(zMovement.z * moveAmount_Z));

    }
    void MoveArmR(GameObject moverObject, Vector3 xMovement, Vector3 zMovement, Vector3 verticalMovement, float moveAmountX, float moveAmount_Z, float moveAmount_Y)
    {
        Vector3 position = moverObject.transform.localPosition;
        moverObject.transform.localPosition = new Vector3((xMovement.x * moveAmountX) + _armR_X_Offset, -(verticalMovement.y * moveAmount_Y) + _armR_Y_Offset, zMovement.z * moveAmount_Z);

    }
    void MoveHandL(GameObject moverObject, Vector3 xMovement, Vector3 zMovement, Vector3 verticalMovement, float moveAmountX, float moveAmount_Z, float moveAmount_Y)
    {
        Vector3 position = moverObject.transform.localPosition;
        moverObject.transform.localPosition = new Vector3(-(xMovement.x * moveAmountX) + _handL_X_Offset, -(verticalMovement.y * moveAmount_Y), -(zMovement.z * moveAmount_Z));
    }
    void MoveHandR(GameObject moverObject, Vector3 xMovement, Vector3 zMovement, Vector3 verticalMovement, float moveAmountX, float moveAmount_Z, float moveAmount_Y)
    {
        Vector3 position = moverObject.transform.localPosition;
        moverObject.transform.localPosition = new Vector3((xMovement.x * moveAmountX) + _handR_X_Offset, -(verticalMovement.y * moveAmount_Y), zMovement.z * moveAmount_Z);
    }
    void MoveLegL(GameObject moverObject, Vector3 xMovement, Vector3 zMovement, Vector3 verticalMovement, float moveAmountX, float moveAmount_Z, float moveAmount_Y)
    {
        Vector3 position = moverObject.transform.localPosition;
        float actualX = position.x;
        if (!legFrozenX_L)
            actualX = -(xMovement.x * moveAmountX);
        moverObject.transform.localPosition = new Vector3(actualX, verticalMovement.y * moveAmount_Y, zMovement.z * moveAmount_Z);
        if (moverObject.transform.localPosition.y <= 0f) 
        { 
            legFrozenX_L = true;
            moverObject.transform.localPosition = new Vector3(moverObject.transform.localPosition.x, 0f, moverObject.transform.localPosition.z); 
        }
        else
            legFrozenX_L = false;
            
    }
    void MoveLegR(GameObject moverObject, Vector3 xMovement, Vector3 zMovement, Vector3 verticalMovement, float moveAmountX, float moveAmount_Z, float moveAmount_Y)
    {
        Vector3 position = moverObject.transform.localPosition;
        float actualX = position.x;
        if (!legFrozenX_R)
            actualX = xMovement.x * moveAmountX;
        moverObject.transform.localPosition = new Vector3(actualX, -(verticalMovement.y * moveAmount_Y), -(zMovement.z * moveAmount_Z));
        if (moverObject.transform.localPosition.y <= 0f)
        {
            legFrozenX_R = true;
            moverObject.transform.localPosition = new Vector3(moverObject.transform.localPosition.x, 0f, moverObject.transform.localPosition.z);
        }
        else
            legFrozenX_R = false;
    }
    void MoveToeL(GameObject moverObject, Vector3 xMovement, Vector3 zMovement, Vector3 verticalMovement, float moveAmountX, float moveAmount_Z, float moveAmount_Y)
    {
        Vector3 position = moverObject.transform.localPosition;
        float actualX = position.x;
        if (!legFrozenX_L)
            actualX = -(xMovement.x * moveAmountX) + _toeL_X_Offset;
        float actualZ = (zMovement.z * moveAmount_Z) + _toeL_Z_Offset;
        moverObject.transform.localPosition = new Vector3(actualX, verticalMovement.y * moveAmount_Y, actualZ);
        if (moverObject.transform.localPosition.y <= 0f)
            moverObject.transform.localPosition = new Vector3(moverObject.transform.localPosition.x, 0f, moverObject.transform.localPosition.z);
    }
    void MoveToeR(GameObject moverObject, Vector3 xMovement, Vector3 zMovement, Vector3 verticalMovement, float moveAmountX, float moveAmount_Z, float moveAmount_Y)
    {
        Vector3 position = moverObject.transform.localPosition;
        float actualX = position.x;
        if (!legFrozenX_R)
            actualX = (xMovement.x * moveAmountX) + _toeR_X_Offset;
        float actualZ = -(zMovement.z * moveAmount_Z) + _toeR_Z_Offset;
        moverObject.transform.localPosition = new Vector3(actualX, -(verticalMovement.y * moveAmount_Y), actualZ);
        if (moverObject.transform.localPosition.y <= 0f)
            moverObject.transform.localPosition = new Vector3(moverObject.transform.localPosition.x, 0f, moverObject.transform.localPosition.z);
    }


}

