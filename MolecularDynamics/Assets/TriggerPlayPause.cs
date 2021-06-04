using UnityEngine;
using UnityEngine.Events;

public class TriggerPlayPause : MonoBehaviour {
    public UnityEvent activate;
    public UnityEvent deactivate;

    private bool doActivate = false;

    public void OnTriggerEnter () {
        
        if (doActivate) {

            activate.Invoke();

            doActivate = false;

            return;
        }

        deactivate.Invoke();

        doActivate = true;
    }
}