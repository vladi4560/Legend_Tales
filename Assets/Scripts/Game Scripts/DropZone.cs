using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!TurnManager.Instance.myTurn)
            return;
        if (eventData.pointerDrag == null)
        {
            return;
        }
        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (d != null)
        {
            d.placeHolderParent = this.transform;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!TurnManager.Instance.myTurn)
            return;
        if (eventData.pointerDrag == null)
        {
            return;
        }
        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        //Debug.Log(eventData.pointerDrag.name + " On Pointer Exit " + gameObject.name);
        if (d != null && d.placeHolderParent == this.transform)
        {
            d.placeHolderParent = d.parentToReturnTo;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!TurnManager.Instance.myTurn)
            return;
        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        // Debug.Log (eventData.pointerDrag.name +" On Drop "+ gameObject.name);
        if (d != null)
        {
            d.parentToReturnTo = this.transform;
            //d.enabled = false;
            //Debug.Log(eventData.pointerDrag.name + " On Drop " + d.parentToReturnTo.name);
        }

    }

}
