using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardScale : MonoBehaviour, IPointerDownHandler, IPointerExitHandler
{
    private Vector3 originalScale;
    public float scaleMultiplier = 1.5f; // The amount by which the card grows. You can change this to suit your needs.

    void Start()
    {
        originalScale = transform.localScale;
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        
        transform.localScale = originalScale;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        transform.localScale = originalScale * scaleMultiplier;// Reset to original size

    }
}

