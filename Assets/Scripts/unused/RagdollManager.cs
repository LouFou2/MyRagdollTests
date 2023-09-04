using System;
using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

// This script is to control all the limbs in the ragdoll
// To control when each limb loses drive/power (so it "collapses")
// This script will use the ragdollPowerFactor variable from the script attached to each individual limb
// And it will collapse limbs in a sequence, depending on the order they are placed in an array
public class RagdollManager : MonoBehaviour
{
    // need an array for each limb that is gonna be "collapsed"
    // so it can be added in the inspector, in the order it will collapse
    
    // set up a main "slider" (float) that will collapse each limb in the index order of the array
    //[SerializeField] 
    //[Range(0f,1f)] private float collapseSlider = 1f;
    //private float fullCollapseValue;

    void Start()
    {
        // map the range of the collapseSlider to the total nr of items in the array
        // fullCollapseValue = (array size);
    }


    void Update()
    {
        // as the collapse slider lowers, it will also count down/switch from the highest int to the lowest int in the array
        // fullCollapseValue = (array size) * collapseSlider;
        // currentItem = Mathf.?(fullCollapseValue);
        
        // So for example:
        // if the array has 4 items, and the collapse slider is at 0.75f,
        // the fullCollapseValue will be 3, and we will switch to item nr 3
        // when the slider is at 0.5, the fullCollapseValue will be 2, switch to item nr 2...

        // So when the collapseSlider gets lowered, as it passes the next int value, it will start affecting the next item's ragDollPowerFactor value
        // float mappedCollapseValue = fullCollapseValue - (current int value + 1);
        // So for example, if our array has 4 items, the slider is at 0.5, fullCollapseValue is 2: the mappedCollapseValue is 1
        // now, as the slider lowers, it will go from 1 to 0, until current int becomes the next int

        // make sure to only affect the ragdollPowerFactor of the CURRENT ITEM
        // currentItem's ragdollPowerFactor = mappedCollapseValue
    }
}
