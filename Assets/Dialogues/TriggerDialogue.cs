using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class TriggerDialogue : MonoBehaviour
{
    public TextAsset NewText;
    private DialogueText DialogueSystem;
    [SerializeField]
    private bool Repeat;
    [SerializeField]
    private bool HasTriggered;
    [SerializeField]
    private bool LockPlayer;
    public bool isEvidenceTrigger = true;  // Default is true, but can be toggled off for non-evidence triggers
    public bool RequireSixEvidence = false;  // Add this boolean to check if the player needs 6 pieces of evidence

    public Transform FocusObject; // Object the camera should look at
    public float CameraMoveDuration = 2f; // Time taken for the camera to focus and return

    public Evidence evidence; // Reference to the evidence associated with this trigger

    private Transform playerTransform;
    private Camera mainCamera;
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;

    private Renderer triggerRenderer;

    [SerializeField]
    private bool disableMovementDuringFocus = true;

    private bool isMovementDisabled;
    private FirstPersonScript firstPersonScript;

    public List<EvidenceGlow> targetEvidenceGlows = new List<EvidenceGlow>();

    private void Awake()
    {
        DialogueSystem = FindObjectOfType<DialogueText>();
        mainCamera = Camera.main;
        playerTransform = mainCamera.transform.parent;
        firstPersonScript = playerTransform.GetComponentInChildren<FirstPersonScript>();
    }

    void Start()
    {
        triggerRenderer = GetComponent<Renderer>();
        if (triggerRenderer != null)
        {
            triggerRenderer.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !DialogueSystem.IsActive)
        {
            if (!HasTriggered)
            {
                // Check if the evidence requirement is enabled and whether the player has 6 evidences
                if (RequireSixEvidence && EvidenceTracker.Instance.GetCollectedEvidenceCount() < 6)
                {
                    // If 6 evidences haven't been collected, exit
                    Debug.Log("You need 6 pieces of evidence to trigger this dialogue.");
                    return;
                }

                Debug.Log("Evidence Triggered!");  // Debug log to confirm trigger hit
                StartCoroutine(FocusOnObjectAndTriggerDialogue());

                if (isEvidenceTrigger && evidence != null)
                {
                    Debug.Log($"Collecting evidence: {evidence.evidenceName}"); // Confirm evidence is being passed
                    EvidenceTracker.Instance.CollectEvidence(evidence);
                }

                HasTriggered = true;
            }
        }
    }

    private IEnumerator FocusOnObjectAndTriggerDialogue()
    {
        if (FocusObject == null)
        {
            Debug.LogWarning("FocusObject is not assigned. Skipping camera movement.");
            DialogueSystem.SetText(NewText, LockPlayer);
            yield break;
        }

        DialogueSystem.SetText(NewText, LockPlayer);

        yield return null;

        if (targetEvidenceGlows.Count > 0)
        {
            foreach (EvidenceGlow glow in targetEvidenceGlows)
            {
                if (glow != null)
                {
                    glow.StopGlow();
                }
            }
        }
        else
        {
            Debug.LogWarning("No targetEvidenceGlows assigned!");
        }

        playerTransform.GetComponent<CubeMovement>()?.SetDialogueState(true);
        isMovementDisabled = true;
        if (firstPersonScript != null)
            firstPersonScript.enabled = false;

        originalCameraPosition = mainCamera.transform.localPosition;
        originalCameraRotation = mainCamera.transform.localRotation;

        float elapsedTime = 0f;
        while (elapsedTime < CameraMoveDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, FocusObject.position - FocusObject.forward * 2f, elapsedTime / (CameraMoveDuration / 2));
            mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, Quaternion.LookRotation(FocusObject.position - mainCamera.transform.position), elapsedTime / (CameraMoveDuration / 2));
            yield return null;
        }

        yield return new WaitUntil(() => !DialogueSystem.IsActive);

        playerTransform.GetComponent<CubeMovement>()?.SetDialogueState(false);

        elapsedTime = 0f;
        while (elapsedTime < CameraMoveDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            mainCamera.transform.localPosition = Vector3.Lerp(mainCamera.transform.localPosition, originalCameraPosition, elapsedTime / (CameraMoveDuration / 2));
            mainCamera.transform.localRotation = Quaternion.Slerp(mainCamera.transform.localRotation, originalCameraRotation, elapsedTime / (CameraMoveDuration / 2));
            yield return null;
        }

        isMovementDisabled = false;
        if (firstPersonScript != null)
            firstPersonScript.enabled = true;

        mainCamera.transform.localRotation = Quaternion.identity;
    }

    void Update()
    {
        if (isMovementDisabled)
        {
            return;
        }
    }
}
