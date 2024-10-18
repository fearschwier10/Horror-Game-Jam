using UnityEngine;

public class PlaySoundOnPlayerTrigger : MonoBehaviour
{
    public AudioClip soundClip; // Assign your audio clip in the inspector
    private AudioSource audioSource;

    private void Start()
    {
        // Create an AudioSource component and assign it to the object
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = soundClip;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the trigger has the tag "Player"
        if (other.CompareTag("Player"))
        {
            // Play the sound
            audioSource.Play();
        }
    }
}
