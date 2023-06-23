using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MintButton : MonoBehaviour
{
    public Button mintButton;
    // Start is called before the first frame update
    void Start()
    {
        mintButton = GetComponent<Button>();
        mintButton.onClick.AddListener(OnMint);
    }

    // Update is called once per frame
    void Update()
    {
    }
    void OnMint(){
        //BlockchainManager.instance.loadCardsFromAddress();
        BlockchainManager.instance.mintNFT();
    }
    
}
