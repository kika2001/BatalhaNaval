using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ButtonResizeExample : MonoBehaviour
{
    public TextMeshProUGUI TextComponent;
    private void Awake()
    {
        TextComponent = GetComponent<TextMeshProUGUI>();
       
    }

    private void LateUpdate()
    {
        // Get the size of the text for the given string.
        Vector2 textSize = TextComponent.GetPreferredValues(TextComponent.text);
        // Adjust the button size / scale.
        TextComponent.rectTransform.localScale = textSize;
    }
}
