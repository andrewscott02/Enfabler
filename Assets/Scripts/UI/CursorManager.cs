using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        SetCursor(defaultCursor);
    }

    public Texture2D defaultCursor, hoverCursor;
    public Vector2 hotSpot = new Vector2(20, 5);

    public void SetCursor(Texture2D newCursor = null)
    {
        Cursor.SetCursor(newCursor == null ? defaultCursor : newCursor, hotSpot, CursorMode.Auto);
    }
}