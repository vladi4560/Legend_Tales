using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class DeckManager : MonoBehaviour
{
    // public static DeckManager instance;
    public List<ScriptableCard> myDeck = new List<ScriptableCard>();
    public List<CardInfo> hand = new List<CardInfo>();
    public int deckSize = 40;
    public int maxHandSize = 7;
    public PhotonView view;

    private void Awake()
    {
        view = GetComponent<PhotonView>();
    }
    public void CreateDeck(List<string> cardNames)
    {
        cardNames.Add("Luffy");
        cardNames.Add("Shinji");
        cardNames.Add("Tanjiro");
        cardNames.Add("Vash the Stampede");
        cardNames.Add("Bulma");

        foreach (string cardName in cardNames)
        {
            ScriptableCard cardData = CardDataBase.instance.getCardData(cardName);

            if (cardData != null)
            {

                myDeck.Add(cardData);
            }
        }

        ShuffleDeck();
    }
    public void ShuffleDeck()
    {
        for (int i = myDeck.Count - 1; i > 0; i--)
        {
            int rand = Random.Range(0, i + 1);
            ScriptableCard temp = myDeck[i];
            myDeck[i] = myDeck[rand];
            myDeck[rand] = temp;
        }
    }

    public void DrawCard()
    {
        if (myDeck.Count > 0)
        {
            ScriptableCard cardData = myDeck[myDeck.Count - 1]; // Get the last card in the deck
            myDeck.RemoveAt(myDeck.Count - 1); // Remove the last card from the deck
            GameObject newCardObj = Instantiate(cardData.cardPrefabHand);
            CardInfo newCard = newCardObj.GetComponent<CardInfo>();
            if (newCard != null)
            {
                newCard.InitData(cardData, view.Owner.ActorNumber);
                hand.Add(newCard);
            }
        }
    }
}
