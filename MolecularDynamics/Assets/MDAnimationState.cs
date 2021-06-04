using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MDAnimationState
{

    public float playbackTime;

    public float speed;

    public bool isPlaying;

    public WrapMode wrapMode;

    public MDAnimationState()
    {
        this.playbackTime = 0f;

        this.speed = 1f;

        this.isPlaying = true;

        this.wrapMode = WrapMode.Loop;
    }

    public MDAnimationState(float time, float speed, bool isPlaying, WrapMode wrapMode) 
    {
        this.playbackTime = time;

        this.speed = speed;

        this.isPlaying = isPlaying;

        this.wrapMode = wrapMode;
    }

    public MDAnimationState(UnityEngine.Animation animation) 
    {
        this.playbackTime = animation.playbackTime;

        foreach (var childAnim in animation)
        {
            this.speed = childAnim.speed;
        }

        this.isPlaying = animation.isPlaying;

        this.wrapMode = animation.wrapMode;
    }

    public MDAnimationState Pause () 
    {
        this.isPlaying = false;

        return this;
    }

    public MDAnimationState Play () 
    {
        this.isPlaying = true;

        return this;
    }   

    public MDAnimationState Scrub (float time) 
    {
        this.playbackTime = time;

        return this;
    }

    public MDAnimationState Rewind () 
    {
        this.playbackTime = 0f;

        return this;
    }

    public MDAnimationState Stop () 
    {
        this.playbackTime = 0f;

        this.isPlaying = false;

        return this;
    }

    public MDAnimationState SetSpeed (float speed) 
    {
        this.speed = speed;

        return this;
    }

    public MDAnimationState SetWrapMode (WrapMode mode) 
    {
        this.wrapMode = mode;

        return this;
    }
}
