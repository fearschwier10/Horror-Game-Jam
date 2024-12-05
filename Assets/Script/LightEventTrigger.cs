using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LightEventTrigger : MonoBehaviour
{
    public string tag = "Player"; // Tag to identify the player
    public UnityEvent TriggerEvent; // Event to trigger when player enters the zone

    public List<GameObject> lightSources = new List<GameObject>(); // List to hold references to the light sources with FlickeringLight script
    public float flickerDuration = 5f; // Duration for the flickering effect

    private void OnTriggerEnter(Collider other)
    {
        // If the object entering the trigger zone has the tag "Player"
        if (other.gameObject.CompareTag(tag))
        {
            // Trigger the event that starts the flickering light behavior
            TriggerEvent.Invoke();

            // Loop through the list of light sources and trigger the flickering on each
            foreach (GameObject lightSource in lightSources)
            {
                FlickeringLight flickeringLightScript = lightSource.GetComponent<FlickeringLight>();
                if (flickeringLightScript != null)
                {
                    // Set the flicker duration dynamically for each light source
                    flickeringLightScript.flickerDuration = flickerDuration;

                    // Start flickering on the light source
                    flickeringLightScript.StartFlickering();
                }
            }
        }
    }
}

