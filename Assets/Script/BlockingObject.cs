using UnityEngine;

public class BlockingObject : MonoBehaviour
{
    public EvidenceTracker evidenceTracker; // Reference to the EvidenceTracker

    private void Start()
    {
        if (evidenceTracker == null)
        {
            evidenceTracker = EvidenceTracker.Instance; // Ensure we have a reference to the EvidenceTracker
        }
    }

    private void Update()
    {
        // Check if the player has collected 5 or more pieces of evidence
        if (evidenceTracker != null && evidenceTracker.GetCollectedEvidenceCount() >= 5)
        {
            // Disable the blocking object when the player collects 5/6 pieces of evidence
            Destroy(gameObject); // This will make the blocking object disappear
        }
    }
}
