using UnityEngine;
using UnityEngine.Events;

public class TriggerSpeed {
    public UnityEvent activate;
    public UnityEvent deactivate;

    private bool doActivate = false;

    public void OnTriggerEnter () {
        
        if (doActivate) {

            activate.Invoke();

            return;
        }

        deactivate.Invoke();
    }
}