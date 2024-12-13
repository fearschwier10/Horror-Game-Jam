using UnityEngine;

public class ObjectAdderOnTrigger : MonoBehaviour
{
    public GameObject objectToAdd; // Reference to the object to be instantiated
    private bool objectAdded = false; // Keeps track if the object has been added

    void Start()
    {
        // Make sure the object is inactive at the start
        if (objectToAdd != null)
        {
            objectToAdd.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the object colliding with the trigger is the player
        if (other.CompareTag("Player") && !objectAdded)
        {
            AddObjectToScene();
        }
    }

    private void AddObjectToScene()
    {
        if (objectToAdd != null)
        {
            // Instantiate the object at the trigger's position with no rotation
            GameObject newObject = Instantiate(objectToAdd, transform.position, Quaternion.identity);
            newObject.SetActive(true); // Activate the object after instantiation
            objectAdded = true; // Set the flag to true so the object won't be added again
            Debug.Log("Object added to the scene!");
        }
        else
        {
            Debug.LogWarning("No object assigned to add to the scene.");
        }
    }
}