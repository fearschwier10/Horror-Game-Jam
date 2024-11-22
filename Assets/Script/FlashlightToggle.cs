using UnityEngine;

public class FlashlightToggle : MonoBehaviour
{
    private Light flashlight;  // Reference to the Light component
    private AudioSource audioSource;  // Reference to the AudioSource component
    public AudioClip flashlightOnSound;  // Assign the flashlight on sound in the Inspector
    public AudioClip flashlightOffSound;  // Assign the flashlight off sound in the Inspector

    private bool isOn = false;

    private void Start()
    {
        // Get the Light and AudioSource components attached to the Main Camera
        flashlight = GetComponentInChildren<Light>();
        audioSource = GetComponent<AudioSource>();

        if (flashlight != null)
        {
            flashlight.enabled = false;  // Start with flashlight off
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isOn = !isOn;  // Toggle the boolean value
            if (flashlight != null)
            {
                flashlight.enabled = isOn;  // Toggle the flashlight based on the value of isOn

                // Play the corresponding sound
                if (audioSource != null)
                {
                    if (isOn && flashlightOnSound != null)
                    {
                        audioSource.PlayOneShot(flashlightOnSound);
                    }
                    else if (!isOn && flashlightOffSound != null)
                    {
                        audioSource.PlayOneShot(flashlightOffSound);
                    }
                }
            }
        }
    }
}
