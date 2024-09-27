using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Text itemsCollectedText;  // Reference to the UI text for collected items
    private int itemsCollected = 0;  // Number of items collected
    private int totalItems = 10;     // Total number of items to collect

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddCollectedItem()
    {
        itemsCollected++;
        UpdateItemUI();  // Update the UI after collecting an item
    }

    private void UpdateItemUI()
    {
        itemsCollectedText.text = "Collected " + itemsCollected + " / " + totalItems + " items";
    }
}
