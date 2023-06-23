using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

public class PlayerManagerData : MonoBehaviour
{
    public static PlayerManagerData instance;

    public List<string> myDeck;

    public List<int> myIds;
    public List<string> ipfsURLs;
    public List<NFTMetadata> metaDataList;
    public Sprite avatrImg;
    public Sprite avatrBackgroundImg;
    // public Dictionary<int, string> myDeckIdToName;
    public string nickName;
    public int gLTT;
    public int wins;
    public int losts;

    void Awake()
    {
        if (instance == null)
        {
            // If not, set this as the Instance
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    void Start()
    {
//        avatrImg = Demo.instance._avatarImageSprite;
  //      avatrBackgroundImg = Demo.instance._backgroundImageSprite;
        myDeck = new List<string>();
        // myDeckIdToName = new Dictionary<int, string>();
        myIds = new List<int>();

    }

    public async void AddCard(int tokenId, string name)
    {
        // myDeckIdToName.Add(tokenId, name);
        myDeck.Add(name);
        myIds.Add(tokenId);
        await FireBaseManager.instance.AddItemToIdList(tokenId);
    }
    public async void RemoveCard(int tokenId)
    {
        // myDeckIdToName.Remove(tokenId);
        myDeck.Remove(CardDataBase.instance.idToName[tokenId]);
        Debug.Log("Debug RemoveCard");
        if (myIds.Remove(tokenId))
        {
            Debug.Log("Debug RemoveCard2");

            await FireBaseManager.instance.RemoveIdFromList(tokenId);
        }
    }
    public void loadDeckFromDB()
    {
        foreach (int id in myIds)
        {
            myDeck.Add(CardDataBase.instance.idToName[id]);
        }
    }
    public async Task initFromDB()
    {
        myIds = await FireBaseManager.instance.LoadIntList();
        //loadDeckFromDB();
    }
    public async Task loadGLTT()
    {
        string str = "";
        await FireBaseManager.instance.LoadGLTT().ContinueWith((task) =>
        {
            if (task.IsFaulted)
            {
                FireBaseManager.instance.SaveNewUser(PlayerPrefs.GetString("Account"), PlayerPrefs.GetString("Nickname"));
                str = "100";
            }
            else
            {
                str = task.Result.ToString();
            }
        });
        string str2 = Regex.Replace(str, "[^0-9]", "");
        int amount;
        if (int.TryParse(str, out amount))
        {
            this.gLTT = amount;
        }
        else
        {
            Debug.Log("Error: string contains non-numeric characters ");
        }
    }
    public bool payUp(int cost)
    {
        if (gLTT > cost)
        {
            gLTT -= cost;
            FireBaseManager.instance.updateGLTT(gLTT);
            return true;
        }

        return false;
    }
    public void getPaid(int amount)
    {
        gLTT += amount;
        FireBaseManager.instance.updateGLTT(gLTT);
    }

}