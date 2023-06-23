using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckCardToHand : MonoBehaviour
{
    public GameObject Hand;
    public GameObject It;
    void Start()
    { 
        Hand = GameObject.Find("OpponentHand");
        It.transform.SetParent(Hand.transform);
        It.transform.localScale = Vector3.one;
        It.transform.position = new Vector3(transform.position.x,transform.position.y,-48);
        It.transform.eulerAngles = new Vector3(25,0,0);
    }

}
