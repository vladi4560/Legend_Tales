using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public GameObject loadingCanvas;
    public GameObject menuCanvas;
    public GameObject profileCanvas;
    public GameObject settingsCanvas;
    public GameObject marketCanvas;
    public GameObject inventoryCanvas;
    public Button playBtn;
    public Button profileBtn;
    public Button settingsBtn;
    public Button marketBtn;
    public Button inventoryBtn;
    public Button quitBtn;
    public Button backFromProfile;
    public Button backFromSettings;
    public Button backFromMarket;
    public Button backFrominventory;
    //public Button inventoryBtn;
    //public Button disconnectBtn;
    public Button createRoomBtn;
    //public Button joinRoomBtn;

    public Room roomItemPrefab;
    public GameObject cardInventoryPrefab;
    public GameObject cardMarketPrefab;
    public List<Room> roomItemsList = new List<Room>();
    public Transform contentObject;
    public Transform contentMydeck;
    public Transform contentCardsOnSale;
    public TMP_InputField CreateinputField;


    public void Start()
    {
        if (instance == null)
            instance = this;
        playBtn.onClick.AddListener(openLobbyCanvas);
        createRoomBtn.onClick.AddListener(createRoomInLobby);
        profileBtn.onClick.AddListener(openProfileCanvas);
        settingsBtn.onClick.AddListener(openSettingCanvas);
        backFromProfile.onClick.AddListener(closeProfileCanvas);
        backFromSettings.onClick.AddListener(closeProfileCanvas);
        marketBtn.onClick.AddListener(openMarketCanvas);
        backFromMarket.onClick.AddListener(closeMarketCanvas);
        inventoryBtn.onClick.AddListener(openInventoryCanvas);
        backFrominventory.onClick.AddListener(closeInventoryCanvas);
        inventoryMyWalletBtn.onClick.AddListener(ShowMyWallet);
        inventoryMydeckBtn.onClick.AddListener(ShowMyDeck);
        marketShopBtn.onClick.AddListener(openShopPanel);
        marketTradeBtn.onClick.AddListener(showTrades);
        quitBtn.onClick.AddListener(CloseGame);
        SoundManager.instance.PlayGameSound();
    }
    public void closeInventoryCanvas()
    {
        inventoryCanvas.SetActive(false);
        menuCanvas.SetActive(true);
        foreach (GameObject obj in myCardsObjects)
        {
            Destroy(obj);
        }

    }
    public void openInventoryCanvas()
    {
        inventoryCanvas.SetActive(true);
        menuCanvas.SetActive(false);
        myDeckPanel.SetActive(true);
        myWalletPanel.SetActive(false);
        //PlayerManagerData.instance.loadDeckFromDB();
        ShowMyDeckUI();
    }
    public void closeMarketCanvas()
    {
        menuCanvas.SetActive(true);
        marketCanvas.SetActive(false);
        foreach (GameObject obj in myCardsObjects)
        {
            Destroy(obj);
        }
    }
    public void closeProfileCanvas()
    {
        profileCanvas.SetActive(false);
        menuCanvas.SetActive(true);
    }
    public void closeSettingsCanvas()
    {
        settingsCanvas.SetActive(false);
        menuCanvas.SetActive(true);
    }

    public void openMarketCanvas()
    {
        marketCanvas.SetActive(true);
        menuCanvas.SetActive(false);
        trade.SetActive(false);
    }
    public void openSettingCanvas()
    {
        menuCanvas.SetActive(false);
        settingsCanvas.SetActive(true);
    }
    public void openLobbyCanvas()
    {
        menuCanvas.SetActive(false);
        loadingCanvas.SetActive(true);
        NetworkManager.instance.startServer();
        SoundManager.instance.StopGameSound();
    }
    public void openProfileCanvas()
    {
        menuCanvas.SetActive(false);
        profileCanvas.SetActive(true);
        Demo.instance.UpdateProfileName();
    }
    public void createRoomInLobby()
    {
        string s = CreateinputField.text;
        if (s.Length >= 4)
        {
            NetworkManager.instance.CreateRoom(s);
        }
    }

    public void joinRoomInLobby(string roomName)
    {
        NetworkManager.instance.JoinRoom(roomName);
    }

    public void UpdateRoomList(List<RoomInfo> roomList)
    {
        foreach (Room item in this.roomItemsList)
        {
            Destroy(item.gameObject);
        }
        this.roomItemsList.Clear();

        foreach (RoomInfo room in roomList)
        {
            Room newRoom = Instantiate(roomItemPrefab, contentObject);
            newRoom.SetRoomName(room.Name);
            this.roomItemsList.Add(newRoom);
        }
    }

    public List<GameObject> myCardsObjects;
    public void ShowMyDeckUI()
    {

        myCardsObjects = new List<GameObject>();
        foreach (int cardId in PlayerManagerData.instance.myIds)
        {
            string name = "";
            if (CardDataBase.instance.idToName.ContainsKey(cardId))
            {
                name = CardDataBase.instance.idToName[cardId];
                ScriptableCard scriptableCard = CardDataBase.instance.getCardData(name);
                Debug.Log("Card name is ::" + name);
                GameObject cardPrefab = Instantiate(cardInventoryPrefab, contentMydeck);
                cardPrefab.GetComponent<CardMarketInfo>().InitData(scriptableCard, cardId);
                myCardsObjects.Add(cardPrefab);
            }
            else
            {
                Debug.Log("Cannot read");
            }
        }
    }
    public void CloseGame()
    {
        Application.Quit();
    }
    public Button marketShopBtn;
    public Button marketTradeBtn;
    public Button marketMintBtn;

    public GameObject marketCardPanel;
    public GameObject shop;
    public GameObject trade;

    public void openShopPanel()
    {
        //marketCardPanel.SetActive(false);
        shop.SetActive(true);
        trade.SetActive(false);
    }
    public void showTrades()
    {
        shop.SetActive(false);
        trade.SetActive(true);
        showCardForTrades();
    }

    public void showCardForTrades()
    {
        foreach (ListedNFT card in CardDataBase.instance.listed)
        {
            GameObject cardPrefab = Instantiate(cardMarketPrefab, contentCardsOnSale);
            string name = CardDataBase.instance.idToName[card.tokenId];
            ScriptableCard scriptableCard = CardDataBase.instance.getCardData(name);
            cardPrefab.GetComponent<CardMarketInfo>().InitData(scriptableCard, card.tokenId);
            cardPrefab.GetComponent<CardMarketInfo>().SetPrice(card.price);
            myCardsObjects.Add(cardPrefab);
        }

    }


    public Button inventoryMydeckBtn;
    public Button inventoryMyWalletBtn;
    public GameObject myDeckPanel;
    public GameObject myWalletPanel;
    public TextMeshProUGUI GLTT_Text;
    public TextMeshProUGUI LTT_Text;

    public void ShowMyWallet()
    {
        myDeckPanel.SetActive(false);
        myWalletPanel.SetActive(true);
    }
    public void ShowMyDeck()
    {
        myDeckPanel.SetActive(true);
        myWalletPanel.SetActive(false);
    }

    public void destroyCardAfterSell(int tokenId)
    {
        foreach (GameObject obj in myCardsObjects)
        {
            if (obj.GetComponent<CardMarketInfo>().id == tokenId)
            {
                Destroy(obj);
                break;
            }
        }
    }

    public void destroyCardAfterBuy(int tokenId)
    {
        foreach (GameObject obj in myCardsObjects)
        {
            if (obj.GetComponent<CardMarketInfo>().id == tokenId)
            {
                Destroy(obj);
                break;
            }
        }
    }
}
