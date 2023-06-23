using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class BuyButtonScript : MonoBehaviour
{
    public Button buyButton;
    private bool isBought;
    private int tokenId;
    // Start is called before the first frame update
    void Start()
    {
        buyButton.onClick.AddListener(onBuy);
        isBought = false;
    }
    void Update()
    {
        if (isBought)
            UIManager.instance.destroyCardAfterBuy(tokenId);


    }

    // Update is called once per frame
    public async void onBuy()
    {
        ListedNFT cardToBuy = new ListedNFT();
        CardMarketInfo card = GetComponent<CardMarketInfo>();
        cardToBuy.tokenId = card.id;
        cardToBuy.sellerAddress = await FireBaseManager.instance.getListedAddress(card.id);
        cardToBuy.price = card.price;
        tokenId = card.id;
        if(PlayerManagerData.instance.gLTT < card.price)
            return;

        await BlockchainManager.instance.BuyListedNFT(cardToBuy).ContinueWith((task) =>
        {
            if (task.Result == true)
            {
                isBought = true;
            }
        });


    }
}
