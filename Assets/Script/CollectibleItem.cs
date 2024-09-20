using UnityEngine;
using UnityEngine.UI;

public class CollectibleItem : MonoBehaviour
{
    public float collectionTime = 3f;  // Time to hold E for collection
    private float holdTime = 0f;       // Timer for holding E
    private bool isInRange = false;    // Player in range of the item
    public bool isCollected = false;   // Is item already collected?

    public GameObject pressEIndicator; // UI prompt to press E
    public Slider collectionProgressBar; // Progress bar for holding E
    public GameObject itemUI;          // Reference to the GameObject for UI (collected count)

    private void Start()
    {
        pressEIndicator.SetActive(false);   // Initially hide the "Press E" prompt
        collectionProgressBar.gameObject.SetActive(false);  // Hide progress bar
    }

    private void Update()
    {
        if (isInRange && !isCollected)
        {
            if (Input.GetKey(KeyCode.E))    // If player is holding E
            {
                holdTime += Time.deltaTime; // Increase hold time
                collectionProgressBar.value = holdTime / collectionTime;  // Update progress bar

                if (holdTime >= collectionTime) // If hold time exceeds 3 seconds
                {
                    CollectItem();
                }
            }
            else
            {
                holdTime = 0f;  // Reset the hold time if E is released
                collectionProgressBar.value = 0f;  // Reset progress bar
            }
        }
    }

    private void CollectItem()
    {
        isCollected = true;
        pressEIndicator.SetActive(false);   // Hide the "Press E" prompt
        collectionProgressBar.gameObject.SetActive(false);  // Hide progress bar

        GameManager.instance.AddCollectedItem();  // Inform GameManager to update UI
        Destroy(gameObject);  // Optionally destroy the item after collection
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = true;
            pressEIndicator.SetActive(true);   // Show the "Press E" prompt
            collectionProgressBar.gameObject.SetActive(true);  // Show progress bar
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = false;
            holdTime = 0f;   // Reset hold time
            pressEIndicator.SetActive(false);   // Hide "Press E" prompt
            collectionProgressBar.gameObject.SetActive(false);  // Hide progress bar
        }
    }
}
