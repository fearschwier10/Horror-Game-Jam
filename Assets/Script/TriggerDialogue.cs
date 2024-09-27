using System.Collections;
using System.Collections.Generic;
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
    private void Awake()
    {
        DialogueSystem = FindObjectOfType<DialogueText>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")&&!DialogueSystem.IsActive)
        {
            if (Repeat)
            {
                DialogueSystem.SetText(NewText, LockPlayer);

            }
            else
            { 
                if (!HasTriggered)
                {
                    DialogueSystem.SetText(NewText, LockPlayer);
                    HasTriggered = true;
                }
            }
            
            
        }
    }
}
