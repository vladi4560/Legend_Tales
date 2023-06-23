using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;


public class PlayerController : MonoBehaviourPunCallbacks
{
    public int id;
    public Health hp;
    public DeckManager deckManager;
    public Button endButton;
    public Image Health;
    public Mana mana;
    public TextMeshProUGUI manaText;
    public TextMeshProUGUI hpText;
    public PhotonView view;
    private GameObject playZone;
    private GameObject opponentZone;


    void Awake()
    {
        endButton.onClick.AddListener(EndTurn);
        view = GetComponent<PhotonView>();
        deckManager = GetComponent<DeckManager>();
        mana = new Mana(manaText);
        hp = new Health(hpText, Health);
        playZone = GameObject.Find("PlayZone");
        opponentZone = GameObject.Find("OppPlayZone");
    }

    public void StartGame()
    {
        deckManager.CreateDeck(PlayerManagerData.instance.myDeck);
        for (int i = 0; i < 3; i++)
        {
            deckManager.DrawCard();
        }
        if (PhotonNetwork.IsMasterClient)
        {
            TurnManager.Instance.myTurn = true;
            BeginTurn();
        }
        else
        {
            DisableButton();
            GameController.instance.AddCardFromHand();
        }
    }
    public bool isMyTurn(){
        return TurnManager.Instance.myTurn;
    }

    [PunRPC]
    public void EndTurn()
    {
        TurnManager.Instance.myTurn = false;
        DisableButton();
        photonView.RPC("RpcUpdateTurn", RpcTarget.Others, true);
        GameController.instance.AddCardFromHand();
    }
    [PunRPC]
    public void RpcUpdateTurn(bool isTurnMine)
    {
        EnableButton();
        BeginTurn();
        TurnManager.Instance.myTurn = isTurnMine;

    }
    private void DisableButton()
    {
        ColorBlock colors = endButton.colors;
        colors.disabledColor = Color.gray;
        endButton.colors = colors;
        endButton.interactable = false;
    }

    private void EnableButton()
    {
        ColorBlock colors = endButton.colors;
        colors.normalColor = new Color(139 / 255f, 0, 0);
        endButton.colors = colors;
        endButton.interactable = true;
    }

    public void BeginTurn()
    {
        mana.StartTurn();
        deckManager.DrawCard();
        BoardManager.instance.startTurn();

    }

    public void PlayCard(string cardName)
    {
        ScriptableCard cardData = CardDataBase.instance.getCardData(cardName);
        GameObject newCard = Instantiate(cardData.cardPrefabField, playZone.transform);
        newCard.GetComponent<CardInfo>().InitData(cardData, view.Owner.ActorNumber,true);
        BoardManager.instance.AddCardToBoard(newCard,"player");
        mana.UseMana(cardData.cost);
        view.RPC("SynchronizeCard", RpcTarget.Others, cardName, view.Owner.ActorNumber);
    }

    [PunRPC]
    void SynchronizeCard(string cardName, int playerId)
    {
        ScriptableCard cardData = CardDataBase.instance.getCardData(cardName);
        GameObject newCard = Instantiate(cardData.cardPrefabField, opponentZone.transform);
        newCard.GetComponent<CardInfo>().InitData(cardData, playerId);
        BoardManager.instance.AddCardToBoard(newCard,"opponent");
        GameController.instance.DestroyCardFromHand();
    }


}