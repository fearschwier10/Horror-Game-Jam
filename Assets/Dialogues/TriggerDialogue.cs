using System.Collections;
using UnityEngine;

public class TriggerDialogue : MonoBehaviour
{
    public TextAsset NewText;
    private DialogueText DialogueSystem;
    [SerializeField]
    [Tooltip("Tells if Dialogue has repeated before")]
    private bool Repeat;
    [SerializeField]
    [Tooltip("Tells if Dialogue has triggered before")]
    private bool HasTriggered;
    [SerializeField]
    [Tooltip("Tells if Player has been locked")]
    private bool LockPlayer;

    public Transform FocusObject; // Object the camera should look at
    public float CameraMoveDuration = 2f; // Time taken for the camera to focus and return

    private Transform playerTransform; // Reference to the player's transform
    private Camera mainCamera;
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;

    private Renderer triggerRenderer;

    [SerializeField]
    [Tooltip("Disable player movement during camera focus")]
    private bool disableMovementDuringFocus = true; // Flag to control movement restrictions

    private bool isMovementDisabled; // Tracks current movement state

    private void Awake()
    {
        DialogueSystem = FindObjectOfType<DialogueText>();
        mainCamera = Camera.main; // Cache the main camera
        playerTransform = mainCamera.transform.parent; // Assuming the camera is a child of the player
    }

    void Start()
    {
        // Get the Renderer component of this GameObject
        triggerRenderer = GetComponent<Renderer>();

        // If a Renderer is attached, hide it during play
        if (triggerRenderer != null)
        {
            triggerRenderer.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !DialogueSystem.IsActive)
        {
            if (Repeat)
            {
                StartCoroutine(FocusOnObjectAndTriggerDialogue());
            }
            else
            {
                if (!HasTriggered)
                {
                    StartCoroutine(FocusOnObjectAndTriggerDialogue());
                    HasTriggered = true;
                }
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

        // Disable player movement
        isMovementDisabled = true;

        // Save the camera's original position and rotation
        originalCameraPosition = mainCamera.transform.localPosition;
        originalCameraRotation = mainCamera.transform.localRotation;

        // Move and rotate the camera to look at the focus object
        float elapsedTime = 0f;
        while (elapsedTime < CameraMoveDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, FocusObject.position - FocusObject.forward * 2f, elapsedTime / (CameraMoveDuration / 2));
            mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, Quaternion.LookRotation(FocusObject.position - mainCamera.transform.position), elapsedTime / (CameraMoveDuration / 2));
            yield return null;
        }

        // Trigger dialogue
        DialogueSystem.SetText(NewText, LockPlayer);

        // Wait until the dialogue system is no longer active
        yield return new WaitUntil(() => !DialogueSystem.IsActive);

        // Return the camera to its original position and rotation
        elapsedTime = 0f;
        while (elapsedTime < CameraMoveDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            mainCamera.transform.localPosition = Vector3.Lerp(mainCamera.transform.localPosition, originalCameraPosition, elapsedTime / (CameraMoveDuration / 2));
            mainCamera.transform.localRotation = Quaternion.Slerp(mainCamera.transform.localRotation, originalCameraRotation, elapsedTime / (CameraMoveDuration / 2));
            yield return null;
        }

        // Re-enable player movement
        isMovementDisabled = false;

        // Sync camera rotation with player's rotation to avoid misalignment
        mainCamera.transform.localRotation = Quaternion.identity; // Reset to match player's rotation
    }

    void Update()
    {
        if (isMovementDisabled)
        {
            // Skip all input processing to prevent side-to-side movement
            return;
        }

        // Handle other player interactions or actions here if necessary
    }
}
