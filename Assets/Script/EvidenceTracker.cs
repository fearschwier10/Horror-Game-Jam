using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Collections;

public class EvidenceTracker : MonoBehaviour
{
    public static EvidenceTracker Instance;

    [Header("End Game Trigger Settings")]
    public GameObject endGameTrigger; // Reference to the end game trigger in the scene
    public GameObject endGameScreen; // Reference to the end game screen UI
    public AudioClip endGameSound; // Sound to play when the end game is triggered

    [Header("UI Elements")]
    public GameObject evidenceTrackerPanel; // Reference to the entire evidence tracker panel
    public TextMeshProUGUI evidenceCountText; // Text to display "Evidence Collected: 1/6"
    public AudioSource audioSource; // Reference to AudioSource for playing sounds

    [Header("Evidence Tracking")]
    public List<Evidence> evidenceList = new List<Evidence>();  // List of all available evidence in the game
    private List<Evidence> collectedEvidence = new List<Evidence>(); // List of collected evidence

    [Header("UI Elements for Evidence Count Display")]
    public TextMeshProUGUI evidenceDisplayText; // Text to display the number of collected evidence
    public GameObject evidenceDisplayPanel; // Panel for the evidence display

    [Header("Fade Settings")]
    public float fadeDelay = 2.5f; // Time to wait before starting the fade (can be edited in the Inspector)

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Hide the evidence display and end game trigger initially
        evidenceDisplayPanel.SetActive(false);
        if (endGameTrigger != null)
        {
            endGameTrigger.SetActive(false);
        }
    }

    public void CollectEvidence(Evidence evidence)
    {
        if (!collectedEvidence.Contains(evidence))
        {
            collectedEvidence.Add(evidence);
            UpdateEvidenceUI(evidence);

            // Check if all evidence is collected
            if (collectedEvidence.Count == evidenceList.Count)
            {
                ActivateEndGameTrigger(); // Enable the end game trigger
            }
        }
    }

    private void ActivateEndGameTrigger()
    {
        if (endGameTrigger != null)
        {
            endGameTrigger.SetActive(true);

            // Optionally play a sound for the end game
            if (endGameSound != null)
            {
                audioSource.PlayOneShot(endGameSound);
            }
        }
        else
        {
            Debug.LogError("EndGameTrigger is not assigned in EvidenceTracker!");
        }
    }

    private void UpdateEvidenceUI(Evidence evidence)
    {
        // Display the evidence collected count in the format: "Evidence Collected: 1/6 sjdbasjdasd"
        evidenceCountText.text = $"Evidence Collected: {collectedEvidence.Count}/{evidenceList.Count}";

        // Show the UI panel
        if (evidenceTrackerPanel != null)
        {
            evidenceTrackerPanel.SetActive(true);
        }

        // Play the specific sound for this evidence
        if (evidence != null && evidence.collectSound != null)
        {
            audioSource.PlayOneShot(evidence.collectSound);
        }

        // Hide the UI after a short delay
        StartCoroutine(HideEvidenceUI());
    }

    public IEnumerator HideEvidenceUI()
    {
        // Wait for the fadeDelay time before starting the fade
        yield return new WaitForSeconds(fadeDelay);

        // Fade out the UI panel gradually
        if (evidenceTrackerPanel != null)
        {
            float elapsedTime = 0f;
            CanvasGroup canvasGroup = evidenceTrackerPanel.GetComponent<CanvasGroup>();

            if (canvasGroup == null)
            {
                canvasGroup = evidenceTrackerPanel.AddComponent<CanvasGroup>();
            }

            // Ensure that the panel starts at full opacity (alpha = 1)
            canvasGroup.alpha = 1f;

            // Gradually fade out the panel
            while (elapsedTime < 1.5f) // Fade duration can be adjusted here
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / 1.5f);
                yield return null;
            }

            // Once the fade is complete, set the panel to inactive
            evidenceTrackerPanel.SetActive(false);
        }
    }

     public int GetCollectedEvidenceCount()
    {
        return collectedEvidence.Count;
    }
}