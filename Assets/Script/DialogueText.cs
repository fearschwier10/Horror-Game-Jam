using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueText : MonoBehaviour
{
    public TextAsset text;
    public TMP_Text TMP_Text;
    public Image panel;
    // Start is called before the first frame update
    void Start()
    { 
        DisableText();
    }

    public void SetText(TextAsset newtext)
    {
        text= newtext;
        //references TMP_Text
        TMP_Text.text = text.text;
        TMP_Text.enabled = true;
        panel.enabled = true;
    }
    public void DisableText()
    {
        TMP_Text.enabled = false;
        panel.enabled = false;
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
