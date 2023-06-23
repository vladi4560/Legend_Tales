using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class CardAttack : MonoBehaviour ,IPointerDownHandler
{
    public bool isSelected = false; // New flag for whether the card is selected
    // Assuming you have a reference to a target
    public void OnPointerDown(PointerEventData eventData)
    {
        isSelected = !isSelected; // This will toggle the selection status each click
        if(isSelected)
            BoardManager.instance.cardSelected(gameObject);
        else
            BoardManager.instance.cardDeselected(gameObject);
    }
}
