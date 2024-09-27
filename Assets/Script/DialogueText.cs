using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueText : MonoBehaviour
{
    CubeMovement PlayerMovement;
    public TextAsset text;
    public TMP_Text TMP_Text;
    public Image panel;
    public float typingSpeed = 0.05f; // Speed of typing effect

    private Coroutine typingCoroutine;

    public bool IsActive => TMP_Text.enabled;
    public UnityEvent OnDialogueStart;
    public UnityEvent OnDialogueEnd;

    private void Awake()
    {
        PlayerMovement = FindObjectOfType<CubeMovement>();
    }

    // Start is called before the first frame update
    void Start()
    {
        DisableText();
    }

    public void SetText(TextAsset newText, bool LockPlayer)
    {
        text = newText;
        // Start typing effect
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine); // Stop any ongoing typing coroutine
        }
        TMP_Text.text = ""; // Clear the current text
        TMP_Text.enabled = true;
        panel.enabled = true;
        typingCoroutine = StartCoroutine(TypeText()); // Start typing the new text
        
        OnDialogueStart.Invoke();
        if (LockPlayer)
        {
            //PlayerMovement.LockMovement
            Debug.Log("Locking Movement");
        }

    }

    IEnumerator TypeText()
    {
        foreach (char letter in text.text.ToCharArray())
        {
            TMP_Text.text += letter;
            yield return new WaitForSeconds(typingSpeed); // Wait between each letter
        }
    }

    public void DisableText()
    {
        TMP_Text.enabled = false;
        panel.enabled = false;
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine); // Stop typing effect if active
        }
        OnDialogueEnd.Invoke();
    }

    private void Update()
    {
        if (TMP_Text.enabled)
        {
            if (Input.GetMouseButtonDown(0))
            {
                DisableText();
            }
        }
    }
}
