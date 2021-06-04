using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Komodo.Runtime;

public class NetworkedAnimator : MonoBehaviour
{
    public Animation localAnimation;

    private MDAnimationState _localState;

    private float _savedSpeed;
    
    public bool isCanonicallyPlaying;

    // NOTE(brandon): Changing most properties for an Animation object will 
    // change all child animators. 

    private string _messageName = "mdAnimationState";

    void OnReset ()
    {
        if (!localAnimation) 
        {
            throw new System.Exception("You must assign a localAnimator in NetworkedAnimator.");
        }
    }

    void Awake ()
    {
        if (!localAnimation) 
        {
            throw new System.Exception("You must assign a localAnimator in NetworkedAnimator.");
        }
        
        GlobalMessageManager.Instance.Subscribe(_messageName, ReceiveAnimationState);
    }

    void Start ()
    {
        localAnimation = GetComponent<Animation>();

        StartCoroutine(SendLocalAnimationState());
    }

    void OnDestroy () 
    {
        StopCoroutine(SendLocalAnimationState());
    }

    /*
    *
    * Automatically generates an animation state object from the state of the local animator.
    *
    */
    IEnumerator SendLocalAnimationState ()
    {
        var waitTime = new WaitForSeconds(1f);

        while (isCanonicallyPlaying) 
        {
            var state = new MDAnimationState(localAnimation);

            SendMessage(state);

            yield return waitTime;
        }
    }

    void SendMessage (MDAnimationState state) 
    {
        var stateObj = JsonUtility.ToJson(state);

        var stateMessage = new KomodoMessage(_messageName, stateObj);

        stateMessage.Send();
    }

    /* Begin public local animation functions. These can be chained. */

    public Animation Pause ()
    {
        foreach (var childAnim in localAnimation)
        {
            _savedSpeed = childAnim.speed;

            childAnim.speed = 0f;
        }

        isCanonicallyPlaying = false;

        return localAnimation;
    }

    private void _RestoreSpeed () 
    {
        foreach (var childAnim in localAnimation)
        {
            childAnim.speed = _savedSpeed;
        }
    }

    public Animation Play ()
    {
        _RestoreSpeed();

        localAnimation.Play();

        SetIsCanonicallyPlaying(true);

        return localAnimation;
    }

    public Animation Rewind () 
    {
        localAnimation.Rewind();

        return localAnimation;
    }

    public Animation Stop () 
    {
        // this will also rewind it to the start.

        localAnimation.Stop();

        return localAnimation;
    }

    public Animation ToggleLoop () 
    {
        if (localAnimation.wrapMode == WrapMode.Loop) 
        {
            localAnimation.wrapMode = ClampForever;

            return localAnimation;
        }

        localAnimation.wrapMode = WrapMode.Loop;

        return localAnimation;
    }

    public void Scrub (float time) 
    {
        localAnimation.playbackTime = time;

        var state = new MDAnimationState(localAnimation);

        SendMessage(state);
    }

    public Animation ChangeSpeed (float speed) 
    {
        localAnimation.speed = speed;

        var state = new MDAnimationState(localAnimation);

        SendMessage(state);
    }

    /* Receiving functions */

    public void SetIsCanonicallyPlaying (bool isPlaying) 
    {
        isCanonicallyPlaying = isPlaying;
    }

    public Animation SetIsPlaying (bool isPlaying)
    {
        localAnimation.isPlaying = isPlaying;

        return localAnimation;
    }

    public Animation SetTime (float time)
    {
        localAnimation.playbackTime = time;

        return localAnimation;
    }

    public Animation SetSpeed (float speed)
    {
        foreach (AnimationState state in localAnimation)
        {
            state.speed = speed;
        }

        return localAnimation;
    }

    public Animation SetWrapMode (WrapMode mode) 
    {
        localAnimation.wrapMode = mode;

        return localAnimation;
    }

    void ReceiveAnimationState (string stateAsString) {

        var state = JsonUtility.FromJSON<MDAnimationState>(stateAsString);

        SetIsPlaying(state.isPlaying);

        SetIsCanonicallyPlaying(state.isPlaying);

        SetTime(state.playbackTime);

        SetSpeed(state.speed);

        SetWrapMode(state.wrapMode);

    }
}
