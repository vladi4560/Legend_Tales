using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using System;
using DG.Tweening;

public class BoardManager : MonoBehaviourPunCallbacks
{
    public static BoardManager instance { get; private set; }

    public List<GameObject> playerCards; // List of cards player 1 has on the board
    public List<GameObject> opponentCards; // List of cards player 2 has on the board
    public GameObject[] selectedCards;
    public PlayerController player;
    public Health oppHealth;
    public Image oppHealthCircle;
    public TextMeshProUGUI healthText; // The UI Text object that displays the health
    private bool isGameOver = false;
    public bool isWon;
    private TargetArrow targetCursor;
    public int[] selectedIndex;
    public PhotonView view;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
        oppHealth = new Health(healthText, oppHealthCircle);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        targetCursor = GetComponent<TargetArrow>();
        view = GetComponent<PhotonView>();
        playerCards = new List<GameObject>();
        opponentCards = new List<GameObject>();
        selectedCards = new GameObject[2];
        selectedIndex = new int[2];
    }
    private void Update()
    {
        targetCursor.changeCursor(isSelected());
        if (isGameOver)
        {
            if (isWon)
            {
                GameController.instance.WonTheGame();
                isGameOver = false;
            }
            else
            {
                GameController.instance.LostTheGame();
                isGameOver = false;
            }

        }
    }
    public bool isSelected()
    {
        return selectedCards[0] != null && selectedCards[0].GetComponent<CardInfo>().canAttack;
    }
    public void cardSelected(GameObject card)
    {
        Debug.Log(playerCards.Contains(card) + "This is contain function");
        if (playerCards.Contains(card))
        {
            selectedCards[0] = card;
            selectedIndex[0] = playerCards.IndexOf(card);
        }
        else if (selectedCards[0] != null && opponentCards.Contains(card))
        {
            selectedCards[1] = card;
            selectedIndex[1] = opponentCards.IndexOf(card);
            HandleAttack();
            resetArrays();
        }
    }
    private void resetArrays()
    {
        selectedCards = new GameObject[2];
        selectedIndex = new int[2];
    }
    public void oppSelected()
    {
        Debug.Log("I tried to sekect" + opponentCards.Count + " " + isListNull(opponentCards));

        if (opponentCards.Count == 0 || isListNull(opponentCards))
        {

            Debug.Log("I ATTACKED HIM");

            if (selectedCards[0] != null)
            {
                HandleAttackVsPlayer();
                resetArrays();
            }
        }

    }
    private bool isListNull(List<GameObject> cards)
    {
        foreach (GameObject card in cards)
        {
            if (card != null)
            {
                return false;
            }
        }
        return true;
    }

    private void HandleAttackVsPlayer()
    {
        //Debug.Log("I ATTACKED HIM");
        if (!selectedCards[0].GetComponent<CardInfo>().canAttack)
            return;
        int damage = selectedCards[0].GetComponent<CardInfo>().damage;
        if (oppHealth.TakeDamage(damage))
        {
            isGameOver = true;
            isWon = true;
        };
        view.RPC("TakeRealDamage", RpcTarget.Others, damage);
        selectedCards[0].transform.DOPunchPosition(new Vector3(-6.1035e-05f,13f,0f), 0.5f, 10, 1, false);
        cardAttacked(selectedCards[0]);
    }

    [PunRPC]
    private void TakeRealDamage(int damage)
    {
        if (player.hp.TakeDamage(damage))
        {
            isGameOver = true;
            isWon = false;
        }
    }

    private void HandleAttack()
    {
        if (!selectedCards[0].GetComponent<CardInfo>().canAttack)
            return;
        int attackerDamage, receiverDamage;
        attackerDamage = selectedCards[0].GetComponent<CardInfo>().damage;
        receiverDamage = selectedCards[1].GetComponent<CardInfo>().damage;
        StartCoroutine(HandleAttackAnimationPlayer());
        if (cardDamaged(selectedCards[0], receiverDamage))
            cardAttacked(selectedCards[0]);
        cardDamaged(selectedCards[1], attackerDamage);
        view.RPC("GetAttacked", RpcTarget.Others, selectedIndex[0], selectedIndex[1]);

    }

    private IEnumerator HandleAttackAnimationPlayer()
    {
        SoundManager.instance.PlaySoundEffectAttack();
        Vector3 v = new Vector3(0, selectedCards[0].transform.position.y + 1.5f, 0);
        selectedCards[0].transform.DOPunchPosition(v, 0.5f, 10, 1, false);
        selectedCards[1].transform.DOShakePosition(1.5f, 40f, 5, 2, false).SetDelay(0.3f);
        yield return new WaitForSeconds(2);
    }
    [PunRPC]
    private void GetAttacked(int attackerIndex, int defenderIndex)
    {
        GameObject myCard, oppCard;
        myCard = playerCards[defenderIndex];
        oppCard = opponentCards[attackerIndex];
        int attackerDamage, receiverDamage;
        attackerDamage = oppCard.GetComponent<CardInfo>().damage;
        receiverDamage = myCard.GetComponent<CardInfo>().damage;

        cardDamaged(oppCard, receiverDamage);
        cardDamaged(myCard, attackerDamage);

    }

    public void cardDeselected(GameObject card)
    {
        if (selectedCards[0] != null && playerCards.Contains(card))
            selectedCards[0] = null;
    }
    public void startTurn()
    {
        foreach (GameObject card in playerCards)
        {
            card.GetComponent<CardInfo>().canAttack = true;
        }
    }

    public void AddCardToBoard(GameObject card, string zone)
    {
        if (zone.Equals("player"))
        {
            playerCards.Add(card);
            card.GetComponent<CardInfo>().canAttack = true;
        }
        else
        {
            opponentCards.Add(card);
        }
    }
    public void cardAttacked(GameObject card)
    {
        card.GetComponent<CardInfo>().canAttack = false;

    }
    private bool cardDamaged(GameObject card, int damage)
    {
        card.GetComponent<CardInfo>().life -= damage;
        if (card.GetComponent<CardInfo>().life <= 0)
        {
            StartCoroutine(RemoveCardFromBoard(card));
            return false;
        }
        else
        {
            card.GetComponent<CardInfo>().updateUICard();
            return true;
        }

    }


    // private  void RemoveCardFromBoard(GameObject card)
    // {
    //     if (playerCards.Remove(card))
    //     {
    //         Destroy(card);
    //     }
    //     else if (opponentCards.Remove(card))
    //     {
    //         Destroy(card);
    //     }
    // }
    private IEnumerator RemoveCardFromBoard(GameObject card)
    {
        SoundManager.instance.PlaySoundEffectDeath();
        card.transform.DOShakePosition(1.5f, 40f, 5, 2, false).SetDelay(0.5f); ;
        yield return new WaitForSeconds(2);
        if (playerCards.Remove(card))
        {
            Destroy(card);
        }
        else if (opponentCards.Remove(card))
        {
            Destroy(card);
        }
    }



}
