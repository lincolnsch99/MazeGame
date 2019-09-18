using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControl : MonoBehaviour
{
    public Animator animControl;

    public void SetTransition(int set)
    {
        if (set == 1)
            animControl.SetBool("transitioning", true);
        else
            animControl.SetBool("transitioning", false);
    }
}
