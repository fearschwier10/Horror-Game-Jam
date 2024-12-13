using UnityEngine;

public class EvidenceColliderActivator : MonoBehaviour
{
    [Header("Collider Settings")]
    public BoxCollider boxCollider; // Reference to the BoxCollider
    public int requiredEvidenceCount = 6; // Number of evidences required to activate the collider

    private void Start()
    {
        // Ensure the collider is initially disabled
        if (boxCollider != null)
        {
            boxCollider.enabled = false;
        }
    }

    // Call this method when the evidence count changes
    public void CheckEvidenceCount(int collectedEvidenceCount)
    {
        // If the collected evidence reaches the required count, enable the collider
        if (collectedEvidenceCount >= requiredEvidenceCount)
        {
            EnableCollider();
        }
    }

    private void EnableCollider()
    {
        if (boxCollider != null)
        {
            boxCollider.enabled = true;
            Debug.Log("Collider is now active!");
        }
    }
}
