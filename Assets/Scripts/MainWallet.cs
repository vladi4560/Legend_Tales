using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class MainWallet : MonoBehaviour
{
    public static MainWallet instance;

    public bool recentTxnFlag;
    private string resBody;
    // private string address;
    // private string toAddress;
    // private string fromAddress;
    // private string key;

    // Start is called before the first frame update
    void Start()
    {
        recentTxnFlag = false;
    }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    // public void init()
    // {

    // }

    public IEnumerator sendNFT(string tokenId, string toAddress, string sellerAddress ,int amount)
    { // utilizing the tradeAproval function in firebase functions
        recentTxnFlag = false;
        string url = "https://us-central1-lagened-tales.cloudfunctions.net/tradeAproval";
        UnityWebRequest www = new UnityWebRequest(url, "POST");
        string json = "{\"tokenId\": \"" + tokenId + "\", \"toAddress\": \"" + toAddress + "\", \"sellerAddress\": \"" + sellerAddress + "\", \"amount\": \"" + amount + "\"}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
            Debug.Log(www.error + "Web Error");
        else
        {
            if (www.responseCode == 200)
            {
                Debug.Log("Success!");
                recentTxnFlag = true;
            }
            else
            {
                recentTxnFlag = false;
            }
            resBody = www.downloadHandler.text;
        }



    }
    void Update()
    {

    }


    public IEnumerator GetTokens(string toAddress, int amount)
    {
        recentTxnFlag = false;
        string url = "https://us-central1-lagened-tales.cloudfunctions.net/sendTokens";

        string jsonData = "{\"toAddress\": \"" + toAddress + "\", \"amount\": " + amount + "}";

        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);


        using (UnityWebRequest www = new UnityWebRequest(url, "POST", new DownloadHandlerBuffer(), new UploadHandlerRaw(bodyRaw)))
        {
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                if (www.responseCode == 200)
                    recentTxnFlag = true;
                else
                    recentTxnFlag = false;
            }
            else
            {
                recentTxnFlag = false;
                Debug.Log("Send Tokens Response: " + www.downloadHandler.text);
            }
        }
    }

    public IEnumerator DepositTokens(int amount)
    {
        recentTxnFlag = false;
        string url = $"https://us-central1-lagened-tales.cloudfunctions.net/receiveTokens?amount={amount}";


        using (UnityWebRequest www = UnityWebRequest.Post(url, ""))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                if (www.responseCode == 200)
                    recentTxnFlag = true;
                else
                    recentTxnFlag = false;
            }
            else
            {
                recentTxnFlag = false;
                Debug.Log("Burn Tokens Response: " + www.downloadHandler.text);
            }
        }
    }

}