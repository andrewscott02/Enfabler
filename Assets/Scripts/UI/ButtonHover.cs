using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler
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

    #region Handler Events

    public void OnPointerEnter(PointerEventData eventData)
    {
        Selected();
    }

    private void OnDisable()
    {
        Unselected();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Unselected();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Selected();
    }

    public void OnSelect(BaseEventData eventData)
    {
        Selected();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        Unselected();
    }

    #endregion

    void Selected()
    {
        CursorManager.instance.SetCursor(overideCursorTexture == null ? CursorManager.instance.hoverCursor : overideCursorTexture);
        text.color = hoverColor;
    }

    void Unselected()
    {
        CursorManager.instance.SetCursor();
        text.color = defaultColour;
    }
}
