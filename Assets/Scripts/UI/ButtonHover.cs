using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    TextMeshProUGUI text;
    Color defaultColour;
    public Color hoverColor = new Color(1, 0, 1, 1);

    public Texture2D overideCursorTexture;

    private void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        defaultColour = text.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        CursorManager.instance.SetCursor(overideCursorTexture == null ? CursorManager.instance.hoverCursor : overideCursorTexture);
        text.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CursorManager.instance.SetCursor();
        text.color = defaultColour;
    }
}
