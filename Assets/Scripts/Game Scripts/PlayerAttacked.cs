using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class PlayerAttacked : MonoBehaviour, IPointerDownHandler
{
    public bool isSelected = false; // New flag for whether the card is selected
    // Assuming you have a reference to a target
    public void OnPointerDown(PointerEventData eventData)
    {
        isSelected=!isSelected;
        if (isSelected)
            BoardManager.instance.oppSelected();

    }
}
