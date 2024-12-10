using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;  // Add reference for TextMeshPro

public class IntroScreen : MonoBehaviour
{
    public GameObject introPanel;  // Reference to the intro screen UI panel
    public GameObject player;  // Reference to the player object
    public TMP_Text introText;  // Reference to the TextMeshPro text component
    public AudioSource typingSound;  // Reference to the typing sound effect
    public float typingSpeed = 0.05f;  // Speed at which the text is typed

    private bool isIntroActive = true;  // Flag to track if the intro is active

    void Start()
    {
        // Ensure the intro screen is shown at the start of the game
        if (introPanel != null)
        {
            introPanel.SetActive(true);  // Show the intro panel
            Time.timeScale = 0f;  // Pause the game time
            AudioListener.pause = false;  // Ensure audio keeps playing

            // Disable player interactions
            if (player != null)
            {
                player.SetActive(false);  // Deactivate the player so they can't interact with the world
            }

            // Start typing the text
            if (introText != null)
            {
                introText.text = "";  // Clear the text initially
                StartCoroutine(TypeText("Welcome to the haunted house..."));  // Start typing the message
            }
            else
            {
                Debug.LogWarning("Intro Text is not assigned!");
            }
        }
        else
        {
            Debug.LogWarning("Intro Panel is not assigned!");
        }
    }

    void Update()
    {
        // Check for any input (mouse click or key press)
        if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
        {
            // Hide the intro screen and start the game
            if (introPanel != null)
            {
                introPanel.SetActive(false);  // Hide the intro panel
                Time.timeScale = 1f;  // Resume the game time
                AudioListener.pause = false;  // Ensure audio continues

                // Reactivate the player to allow interaction
                if (player != null)
                {
                    player.SetActive(true);  // Activate the player after the intro screen is closed
                }
            }
        }
    }

    // Coroutine to simulate the typing effect
    private IEnumerator TypeText(string message)
    {
        // Loop through each character in the message
        foreach (char letter in message)
        {
            introText.text += letter;  // Add each letter one by one
            if (typingSound != null)
            {
                typingSound.Play();  // Play typing sound
            }
            yield return new WaitForSeconds(typingSpeed);  // Wait between each character
        }
    }
}
