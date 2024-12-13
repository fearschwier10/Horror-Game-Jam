using UnityEngine;

public class EndGameTrigger : MonoBehaviour
{
    [SerializeField] private GameObject endGamePanel; // Reference to the end game panel
    [SerializeField] private AudioClip endGameSound;  // Sound to play when the end game is triggered
    private AudioSource audioSource;  // AudioSource component to play sound

    // Reference to MonsterAI (assuming the player is in range of the monster)
    [SerializeField] private MonsterAITest monsterAI;

    private void Awake()
    {
        // Ensure the endGamePanel is disabled at the start
        if (endGamePanel != null)
        {
            endGamePanel.SetActive(false); 
        }

        // Ensure the AudioSource component is attached
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();  // Adds an AudioSource if one is not attached
        }

        // Ensure we have a reference to the MonsterAI
        if (monsterAI == null)
        {
            monsterAI = FindObjectOfType<MonsterAITest>();  // Find the MonsterAI if not assigned
        }
    }

    private void Start()
    {
        // Optionally, ensure the endGamePanel is disabled (in case the Awake didn't run correctly)
        if (endGamePanel != null)
        {
            endGamePanel.SetActive(false);
        }
    }

    // Method to reset the trigger (e.g., if needed during gameplay)
    public void ResetTrigger()
    {
        if (endGamePanel != null)
        {
            endGamePanel.SetActive(false); // Disable the end game panel
        }

        // Reset audio source if necessary
        if (audioSource != null && endGameSound != null)
        {
            audioSource.Stop(); // Stop any sound that might be playing
        }

        // Reset time scale to normal (if needed)
        Time.timeScale = 1f;

    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player collides with the trigger
        if (other.CompareTag("Player"))
        {
            // Activate the end game panel
            if (endGamePanel != null)
            {
                endGamePanel.SetActive(true);
            }

            // Play the end game sound (if provided)
            if (endGameSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(endGameSound);
            }

            // Stop chase music if the monster is playing it
            if (monsterAI != null)
            {
                monsterAI.StopChaseMusic();
            }

            // Pause the game (freeze time)
            Time.timeScale = 0f;
        }
    }
}