using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CardInfo : MonoBehaviour
{
    public int ownerId;
    [SerializeField]
    public string id;
    [SerializeField]
    public string cardName;
    [SerializeField]
    public int cost;
    [SerializeField]
    public int damage;
    [SerializeField]
    public int life;
    [SerializeField]
    public string cardDescription;
    [SerializeField]
    public Sprite thisSprite;

    public ScriptableCard cardDetails;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI lifeText;
    public Image thisImage;
    public bool canAttack;
    public PlayerController playerController;
    public GameObject battleZone;

    void Awake()
    {
        GameObject obj = GameObject.Find("GameManagers");
        playerController = obj.GetComponent<PlayerController>();
    }
    void Start()
    {
        battleZone = GameObject.Find("PlayZone");
    }

    void Update()
    {
    }

    public void InitData(ScriptableCard cardDetails, int OwnerId,bool canAttack = false)
    {
        ownerId = OwnerId;
        this.canAttack = canAttack;
        id = cardDetails.CardID;
        cardName = cardDetails.cardName;
        cost = cardDetails.cost;
        damage = cardDetails.damage;
        cardDescription = cardDetails.description;
        life = cardDetails.life;
        thisSprite = cardDetails.image;

        this.cardDetails = cardDetails;

        nameText.text = "" + cardName;
        costText.text = "" + cost;
        damageText.text = "" + damage;
        descriptionText.text = " " + cardDescription;
        lifeText.text = " " + life;
        thisImage.sprite = thisSprite;
    }
    public void updateUICard(){
        lifeText.text = " " + life;
    }

}
