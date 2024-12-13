using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Collections;

public class EvidenceTracker : MonoBehaviour
{
    public static EvidenceTracker Instance;

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

        // Hide the evidence display initially
        evidenceDisplayPanel.SetActive(false);
    }

    private void Update()
    {
        // Check for 'R' key press to show evidence count
        if (Input.GetKeyDown(KeyCode.R))
        {
            ShowEvidenceCount();
        }
    }

    public void CollectEvidence(Evidence evidence)
    {
        if (!collectedEvidence.Contains(evidence))
        {
            collectedEvidence.Add(evidence);
            UpdateEvidenceUI(evidence);
        }
    }

    private void UpdateEvidenceUI(Evidence evidence)
    {
        // Display the evidence collected count in the format: "Evidence Collected: 1/6"
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

    public void HideEvidenceTracker()
    {
        if (evidenceTrackerPanel != null)
        {
            evidenceTrackerPanel.SetActive(false); // Hide the UI when you want
        }
    }
    private IEnumerator HideEvidenceUI()
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

    public void ShowEvidenceCount()
    {
        // Display the collected evidence count on the UI
        evidenceDisplayText.text = $"Collected Evidence: {collectedEvidence.Count}/{evidenceList.Count}";
        evidenceDisplayPanel.SetActive(true);

        // Start the fade-out process after showing the evidence count
        StartCoroutine(FadeOutEvidenceDisplay());
    }

    private IEnumerator FadeOutEvidenceDisplay()
    {
        // Wait for the fadeDelay time before starting the fade
        yield return new WaitForSeconds(fadeDelay);

        // Fade out the UI panel gradually
        if (evidenceDisplayPanel != null)
        {
            float elapsedTime = 0f;
            CanvasGroup canvasGroup = evidenceDisplayPanel.GetComponent<CanvasGroup>();

            if (canvasGroup == null)
            {
                canvasGroup = evidenceDisplayPanel.AddComponent<CanvasGroup>();
            }

            // Ensure the panel starts fully visible (alpha = 1)
            canvasGroup.alpha = 1f;

            // Fade the panel out over 1.5 seconds (or your desired fade duration)
            while (elapsedTime < 1.5f)
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / 1.5f);
                yield return null;
            }

            // Once the fade is complete, deactivate the evidence display panel
            evidenceDisplayPanel.SetActive(false);
        }
    }

    public int GetCollectedEvidenceCount()
    {
        return collectedEvidence.Count;
    }
}

