using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Summon : MonoBehaviour
{
    public bool canBeSummon;
    public bool summoned;
    public GameObject battleZone;
    public GameObject OppBattleZone;
    public PlayerController playerController;
    public CardInfo cardInfo;
    // Start is called before the first frame update
    void Awake()
    {
        battleZone = GameObject.Find("PlayZone");
        //OppBattleZone = GameObject.Find("OppPlayZone");
        GameObject obj = GameObject.Find("GameManagers");
        playerController = obj.GetComponent<PlayerController>();
        cardInfo=GetComponent<CardInfo>();
        canBeSummon = false;
        summoned = false;
    }
    // Update is called once per frame
    void Update()
    {
        checkCardSummning();
    }
    public void checkCardSummning(){

            if (playerController.mana.currentMana >= cardInfo.cost && summoned == false)
            {
                canBeSummon = true;
            }
            else
            {
                canBeSummon = false;
            }
            Draggable d = GetComponent<Draggable>();
            if (d == null)
            {
                return;
            }
            if (canBeSummon == true)
            {
                d.enabled = true;
            }
            else
            {
                d.enabled = false;
            }
            if (summoned == false && this.transform.parent == battleZone.transform)
            {
                summoned = true;
                d.enabled = false;
                summon();
            }
        }
     private void summon(){
        //Debug.Log("i know the card is in PlayZone line 96 yes");
        playerController.PlayCard(cardInfo.cardName);
        Destroy(gameObject);
        // TurnSystem.instance.currentMana -= cost;
        // summoned = true;
    }
}