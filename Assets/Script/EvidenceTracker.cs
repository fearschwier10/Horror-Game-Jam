using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EvidenceTracker : MonoBehaviour
{
    public static EvidenceTracker Instance;

    [Header("UI Elements")]
    public TextMeshProUGUI evidenceCountText;
    public Transform evidenceListContent; // Reference to the ScrollView Content
    public GameObject evidenceListItemPrefab; // Prefab for listing evidence dynamically

    [Header("Evidence Tracking")]
    public List<Evidence> evidenceList = new List<Evidence>();
    private List<Evidence> collectedEvidence = new List<Evidence>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void CollectEvidence(Evidence evidence)
    {
        if (!collectedEvidence.Contains(evidence))
        {
            collectedEvidence.Add(evidence);
            UpdateEvidenceUI();
        }
    }

    private void UpdateEvidenceUI()
    {
        // Update evidence count
        evidenceCountText.text = $"Evidence Collected: {collectedEvidence.Count}/{evidenceList.Count}";

        // Optionally update the evidence list in the Scroll View
        foreach (Transform child in evidenceListContent)
        {
            Destroy(child.gameObject);
        }

        foreach (Evidence evidence in collectedEvidence)
        {
            GameObject item = Instantiate(evidenceListItemPrefab, evidenceListContent);
            item.GetComponentInChildren<TextMeshProUGUI>().text = evidence.evidenceName;
        }
    }
}

