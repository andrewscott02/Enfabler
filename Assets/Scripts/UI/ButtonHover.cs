using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISelectHandler
{
    TextMeshProUGUI text;
    Color defaultColour;
    public Color hoverColor = new Color(0.7f, 0.7f, 0.7f, 1);

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

    private void OnDisable()
    {
        CursorManager.instance.SetCursor();
        text.color = defaultColour;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CursorManager.instance.SetCursor();
        text.color = defaultColour;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        CursorManager.instance.SetCursor(overideCursorTexture == null ? CursorManager.instance.hoverCursor : overideCursorTexture);
        text.color = hoverColor;
    }

    public void OnSelect(BaseEventData eventData)
    {
        CursorManager.instance.SetCursor(overideCursorTexture == null ? CursorManager.instance.hoverCursor : overideCursorTexture);
        text.color = hoverColor;
    }
}
