using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MDAnimationState
{
    public float playbackTime;

    public float speed;

    public MDAnimationState()
    {
        this.playbackTime = 0f;

        this.speed = 1f;
    }

    public MDAnimationState(float time, float speed) 
    {
        this.playbackTime = time;

        this.speed = speed;
    }

    public MDAnimationState(Animator animator) 
    {
        this.playbackTime = animator.playbackTime;

        this.speed = animator.speed;
    }
}
