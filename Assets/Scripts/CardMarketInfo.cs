using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;

public class CardMarketInfo : MonoBehaviour, IPointerDownHandler, IPointerExitHandler
{
    public string cardName;
    public int price;
    public Sprite thisSprite;
    public int cost;
    public int damage;
    public int hp;
    public int id;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI contentText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI hpText;
    public Image cardImage;
    public GameObject popUpWindow;
    public TMP_InputField priceToSell;
    public Button closeBtn;
    public Button sellBtn;
    public bool deleteCard = false;

    public void InitData(ScriptableCard card, int ID)
    {
        cardName = card.cardName;
        thisSprite = card.image;
        cost = card.cost;
        damage = card.damage;
        hp = card.life;
        id = ID;
        nameText.text = cardName;
        cardImage.sprite = thisSprite;
        damageText.text = "" + damage;
        costText.text = "" + cost;
        hpText.text = "" + hp;

        closeBtn.onClick.AddListener(closePopUpWindow);
        sellBtn.onClick.AddListener(sellCard);
    }

    public void Update()
    {
        if (deleteCard)
            UIManager.instance.destroyCardAfterSell(id);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        popUpWindow.SetActive(false);

    }
    public void OnPointerDown(PointerEventData eventData)
    {
        popUpWindow.SetActive(true);
    }

    public void closePopUpWindow()
    {
        popUpWindow.SetActive(false);
    }
    public async void sellCard()
    {
        if (priceText.text.Length < 0)
            return;
        int price;
        string str = Regex.Replace(priceText.text, "[^0-9]", "");

        if (int.TryParse(str, out price))
        {
            if (price < 0)
            {
                Debug.Log("Error: number is less than 0");
            }
            else
            {
                await BlockchainManager.instance.ListForSale(id, price).ContinueWith((task) =>
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        deleteCard = true;
                    }
                });
            }
        }
        else
        {
            Debug.Log("Error: string contains non-numeric characters " + priceText.text);
        }

    }
    public void SetId(int ID)
    {

    }
    public void SetPrice(int price)
    {
        this.price = price;
        priceText.text = price + " GLTT";
    }

}
