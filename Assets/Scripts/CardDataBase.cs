using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;

public class CardDataBase : MonoBehaviour
{

    public static CardDataBase instance;
    public List<ScriptableCard> _cache;
    public List<string> ipfsURLs;
    public List<NFTMetadata> metaDataList;
    public List<ListedNFT> listed;
    public Dictionary<int, string> idToName;


    void Awake()
    {
        if (instance == null)
        {
            // If not, set this as the Instance
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        ipfsURLs = new List<string>();
        LoadData();
        loadIPFSFile();

    }
    public void LoadData()
    {
        if (_cache.Count == 0)
        {
            ScriptableCard[] cards = Resources.LoadAll<ScriptableCard>("Cards/");
            Debug.Log(cards.ToString() + "line 29");
            _cache = new List<ScriptableCard>(cards.ToList());
        }
    }
    public async Task<bool> InitFromDataBase()
    {
        loadListedNFTList();
        loadIdToNameDictionary();

        await PlayerManagerData.instance.initFromDB();
        return true;
    }
    private void loadIPFSFile()
    {
        //Ensure that the file exists before trying to read from it
        TextAsset textAsset = Resources.Load<TextAsset>("ipfsList");
        if (textAsset == null)
        {
            Debug.LogError("TextAsset not found at Resources/" + "ipfsList");

            // Convert the array to a List and return it
            return;
        }
        string[] lines = textAsset.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        ipfsURLs = new List<string>(lines);


    }
    public void addCardMetaData(NFTMetadata data)
    {
        if (metaDataList != null)
            metaDataList = new List<NFTMetadata>();
        metaDataList.Add(data);
    }

    public ScriptableCard getCardData(string cardName)
    {
        ScriptableCard cardData = _cache.Find(x => x.cardName == cardName);
        return cardData;
    }
    public void addListedNFT(ListedNFT nft)
    {
        if (listed == null)
            listed = new List<ListedNFT>();
        listed.Add(nft);
        FireBaseManager.instance.AddNewListedNFT(nft);
    }
    public async Task<bool> removeListedNFT(int idToRemove)
    {
        ListedNFT nftToRemove = null;
        if (listed == null)
            return false;
        foreach (ListedNFT nft in listed)
        {
            if (nft.tokenId == idToRemove)
            {
                nftToRemove = nft;
                break;
            }
        }
        if (nftToRemove != null)
        {
            await FireBaseManager.instance.RemoveFromListingByTokenId(idToRemove).ContinueWith((task)=>{
                if(task.IsCompletedSuccessfully)
                            listed.Remove(nftToRemove);

            });
            return true;
        }
        else
        {
            Debug.Log("No NFT Found");
            return false;
        }
    }
    public async void loadListedNFTList()
    {
        listed = new List<ListedNFT>();
        await FireBaseManager.instance.initListedNFTList();
    }

    public async void attachIdToName(int id, string name)
    {
        if (idToName == null)
            idToName = new Dictionary<int, string>();
        if (!idToName.ContainsKey(id))
        {
            idToName.Add(id, name);
            await FireBaseManager.instance.AddIdToName(id, name);
        }
    }
    public async void loadIdToNameDictionary()
    {
        idToName = new Dictionary<int, string>();
        idToName = await FireBaseManager.instance.LoadIdToNameDictionary();

    }

}

[System.Serializable]
public class ListedNFT
{
    public string sellerAddress;
    public int tokenId;
    public int price;
}