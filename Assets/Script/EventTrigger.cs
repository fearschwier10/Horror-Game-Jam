using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour
{
    public string tag = "Player";
    public UnityEvent TriggerEvent;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(tag))
        {
            TriggerEvent.Invoke();
        }
    }

}
