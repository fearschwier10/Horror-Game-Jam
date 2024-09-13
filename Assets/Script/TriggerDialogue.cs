using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDialogue : MonoBehaviour
{
    public TextAsset NewText;
    private DialogueText DialogueSystem;
    private void Awake()
    {
        DialogueSystem = FindObjectOfType<DialogueText>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")){
            DialogueSystem.SetText(NewText);
        }
    }
}
