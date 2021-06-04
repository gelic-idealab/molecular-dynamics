using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using Komodo.Runtime;

public class NetworkedAnimator : MonoBehaviour
{
    public Animator localAnimator;

    private MDAnimationState _localState;

    private float _savedSpeed;
    
    public bool isCanonicallyPlaying;

    // NOTE(brandon): Changing most properties for an Animation object will 
    // change all child animators. 

    private string _messageName = "mdAnimationState";

    void OnReset ()
    {
        if (!localAnimator) 
        {
            throw new System.Exception("You must assign a localAnimator in NetworkedAnimator.");
        }
    }

    void Awake ()
    {
        
        GlobalMessageManager.Instance.Subscribe(_messageName, ReceiveAnimationState);
    }

    void Start ()
    {
        localAnimator = GetComponent<Animator>();

        if (!localAnimator) 
        {
            throw new System.Exception("You must assign a localAnimator in NetworkedAnimator.");
        }

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
            var state = new MDAnimationState(localAnimator);

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

    public void Pause ()
    {
        _savedSpeed = localAnimator.speed;

        localAnimator.speed = 0f;

        SetIsCanonicallyPlaying(false);
    }

    private void _RestoreSpeed () 
    {
        localAnimator.speed = _savedSpeed;
    }

    public void Play ()
    {
        _RestoreSpeed();

        SetIsCanonicallyPlaying(true);
    }

    public Animator ChangeSpeed (float speed) 
    {
        localAnimator.speed = speed;

        var state = new MDAnimationState(localAnimator);

        SendMessage(state);

        return localAnimator;
    }

    /* Receiving functions */

    public void SetIsCanonicallyPlaying (bool isPlaying) 
    {
        isCanonicallyPlaying = isPlaying;
    }

    public Animator SetTime (float time)
    {
        localAnimator.playbackTime = time;

        return localAnimator;
    }

    public Animator SetSpeed (float speed)
    {
        localAnimator.speed = speed;

        return localAnimator;
    }

    void ReceiveAnimationState (string stateAsString) {

        var state = JsonUtility.FromJson<MDAnimationState>(stateAsString);

        SetIsCanonicallyPlaying(state.speed == 0f);

        SetTime(state.playbackTime);

        SetSpeed(state.speed);

    }
}
