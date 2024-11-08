using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueText : MonoBehaviour
{
    CubeMovement PlayerMovement;
    public TextAsset text;
    public TMP_Text TMP_Text;
    public Image panel;
    public float typingSpeed = 0.05f; // Speed of typing effect

    private Coroutine typingCoroutine;
    private bool lockPlayer;

    public bool IsActive => TMP_Text.enabled;
    public UnityEvent OnDialogueStart;
    public UnityEvent OnDialogueEnd;

    private void Awake()
    {
        PlayerMovement = FindObjectOfType<CubeMovement>();
    }

    void Start()
    {
        DisableText();
    }

    public void SetText(TextAsset newText, bool LockPlayer)
    {
        text = newText;
        lockPlayer = LockPlayer; // Store whether to lock the player

        // Start typing effect
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine); // Stop any ongoing typing coroutine
        }
        TMP_Text.text = ""; // Clear the current text
        TMP_Text.enabled = true;
        panel.enabled = true;
        typingCoroutine = StartCoroutine(TypeText()); // Start typing the new text

        OnDialogueStart.Invoke();
        if (lockPlayer)
        {
            LockPlayerMovement(); // Lock the player's movement
        }

        Debug.Log("Triggered Dialogue " + newText.name);
    }

    IEnumerator TypeText()
    {
        foreach (char letter in text.text.ToCharArray())
        {
            TMP_Text.text += letter;
            yield return new WaitForSeconds(typingSpeed); // Wait between each letter
        }

        // Once the typing is done, unlock the player's movement if needed
        if (lockPlayer)
        {
            UnlockPlayerMovement();
        }
    }

    public void DisableText()
    {
        TMP_Text.enabled = false;
        panel.enabled = false;
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine); // Stop typing effect if active
        }
        OnDialogueEnd.Invoke();

        if (lockPlayer)
        {
            UnlockPlayerMovement(); // Ensure player movement is unlocked when the text is closed
        }
    }

    private void LockPlayerMovement()
    {
        if (PlayerMovement != null)
        {
            PlayerMovement.enabled = false; // Disable movement script
            Debug.Log("Player movement locked.");
        }
    }

    private void UnlockPlayerMovement()
    {
        if (PlayerMovement != null)
        {
            PlayerMovement.enabled = true; // Enable movement script
            Debug.Log("Player movement unlocked.");
        }
    }

    private int clickCount = 0; // Track the number of clicks

    private void Update()
    {
        if (TMP_Text.enabled)
        {
            if (Input.GetMouseButtonDown(0)) // Check for mouse click
            {
                clickCount++; // Increment click count

                if (clickCount == 1 && typingCoroutine != null)
                {
                    // Finish typing immediately on the first click
                    StopCoroutine(typingCoroutine);
                    TMP_Text.text = text.text; // Show full text
                }
                else if (clickCount == 2)
                {
                    // On the second click, disable text
                    DisableText();
                    clickCount = 0; // Reset click count after closing dialogue
                }
            }
        }
    }
}