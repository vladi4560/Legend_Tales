using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public PhotonView photonView;
    public PlayerController player;
    public List<GameObject> opponentHand;
    public Image UserPic;
    public Image UserPicBackground;
    public Image OppPic;
    public Image OppPicBackground;
    public GameObject cardInDeck1;
    public GameObject cardInDeck2;
    public GameObject cardInDeck3;
    public GameObject cardInDeck4;
    public GameObject backCardPrefab;
    public GameObject gameOverCanvas;
    public Button mainMenuBtn;
    public Button quitBtn;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        photonView = GetComponent<PhotonView>();
        player = GetComponent<PlayerController>();
        gameOverCanvas.SetActive(false);
        mainMenuBtn.onClick.AddListener(returnToMenu);
        quitBtn.onClick.AddListener(QuitGame);

    }

    void Start()
    {
        InitializedGame();
        UploadPic();
    }
    void UploadPic()
    {
        UserPic.sprite = Demo.instance._avatarImageSprite;
        UserPicBackground.sprite = Demo.instance._backgroundImageSprite;
        photonView.RPC("UploadOppPic", RpcTarget.Others,Demo.instance._currentBackgroundSpriteIndex,Demo.instance._currentAvatarSpriteIndex);
    }
    [PunRPC]
    public void UploadOppPic(int background, int pic)
    {
        OppPicBackground.sprite = Demo.instance.backgroundSprites[background];
        OppPic.sprite = Demo.instance.avatarSprites[pic];
    }
    void InitializedGame()
    {
        player.StartGame();
        OpponentCard();
        cardsInDeck();
    }

    public void OpponentCard()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject backCard = Instantiate(backCardPrefab);
            opponentHand.Add(backCard);
            Debug.Log("line 65 OpponentCard " + backCardPrefab);
        }
    }
    public void DestroyCardFromHand()
    {
        GameObject cardToDestroy = opponentHand[opponentHand.Count - 1];
        if (opponentHand.Count > 0)
        {
            opponentHand.Remove(cardToDestroy);
            Destroy(cardToDestroy);
            Debug.Log("line 75 Destroy Card From Hand" + opponentHand.Count + "   " + opponentHand.ToString());
        }
    }
    public void AddCardFromHand()
    {

        GameObject backCard = Instantiate(backCardPrefab);
        opponentHand.Add(backCard);

    }
    public void cardsInDeck()
    {
        int deckSize = player.deckManager.myDeck.Count;

        if (deckSize < 15)
        {
            cardInDeck1.SetActive(false);
        }
        if (deckSize < 12)
        {
            cardInDeck2.SetActive(false);
        }
        if (deckSize < 10)
        {
            cardInDeck3.SetActive(false);
        }
        if (deckSize < 1)
        {
            cardInDeck4.SetActive(false);
        }

    }

    public void WonTheGame()
    {
        gameOverCanvas.SetActive(true);
        PlayerManagerData.instance.getPaid(50);
    }

    public void LostTheGame()
    {
        gameOverCanvas.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    public void returnToMenu()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(1);  
    }
}
