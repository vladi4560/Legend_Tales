using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform parentToReturnTo = null;

    public Transform placeHolderParent = null;
    public GameObject placeholder = null;

    public void OnBeginDrag(PointerEventData eventData)
    {

        if (!TurnManager.Instance.myTurn)
            return;
        //CardInfo c = GetComponent<CardInfo>();
        placeholder = new GameObject();
        placeholder.transform.SetParent(this.transform.parent);
        LayoutElement le = placeholder.AddComponent<LayoutElement>();
        le.preferredWidth = placeholder.GetComponent<LayoutElement>().preferredWidth;
        le.preferredHeight = placeholder.GetComponent<LayoutElement>().preferredHeight;
        le.flexibleWidth = 0;
        le.flexibleHeight = 0;

        placeholder.transform.SetSiblingIndex(this.transform.GetSiblingIndex());

        parentToReturnTo = this.transform.parent;
        placeHolderParent = parentToReturnTo;
        this.transform.SetParent(this.transform.parent.parent);
        GetComponent<CanvasGroup>().blocksRaycasts = false;

        // parentToReturnTo=this.transform.parent;
        // this.transform.SetParent(this.transform.parent.parent);
        // Debug.Log(parentToReturnTo.name);
        // GetComponent<CanvasGroup>().blocksRaycasts =false;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (!TurnManager.Instance.myTurn)
            return;
        //this.transform.position = eventData.position; 


        this.transform.position = eventData.position;
        this.transform.position = eventData.position;
        if (placeholder.transform.parent != placeHolderParent)
        {
            placeholder.transform.SetParent(placeHolderParent);
        }
        int newSiblingIndex = placeHolderParent.childCount;

        for (int i = 0; i < placeHolderParent.childCount; i++)
        {
            if (this.transform.position.x < placeHolderParent.GetChild(i).position.x)
            {
                newSiblingIndex = i;

                if (placeholder.transform.GetSiblingIndex() < newSiblingIndex)
                {
                    newSiblingIndex--;
                }
                break;
            }

        }
        placeholder.transform.SetSiblingIndex(newSiblingIndex);

    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!TurnManager.Instance.myTurn)
            return;
        GameObject hand = GameObject.Find("PlayZone");
        this.transform.SetParent(parentToReturnTo);
        this.transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        if (parentToReturnTo.name.Equals(hand.name))
        {
            //Debug.Log(parentToReturnTo.name + "line 73 On End Drag " + i.cardDetails);
            //Debug.Log(parentToReturnTo.name + " On OnEnd Drag line 67" + hand.name);
            this.enabled = false;
        }

        Destroy(placeholder);
        // Debug.Log (eventData.pointerDrag.name +" On End Drag "+ parentToReturnTo.name);
        // this.transform.SetParent(parentToReturnTo);
        // GetComponent<CanvasGroup>().blocksRaycasts =true;
    }

}