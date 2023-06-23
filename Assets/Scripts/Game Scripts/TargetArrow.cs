using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetArrow : MonoBehaviour
{
    public Texture2D cursorTexture; // assign the target image in the inspector
    private Vector2 hotSpot = Vector2.zero; // the point in the texture which is treated as the cursor position
    private CursorMode cursorMode = CursorMode.Auto;
    // Start is called before the first frame update
    void Start()
    {
    }
    public void changeCursor(bool isSelected)
    {
        if (isSelected)
            Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
        else
            Cursor.SetCursor(null, hotSpot, cursorMode);

    }

    // Update is called once per frame
    void Update()
    {

    }
}
