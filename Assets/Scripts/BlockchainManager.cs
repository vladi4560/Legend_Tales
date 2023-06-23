using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Web3Unity.Scripts.Library.Web3Wallet;
using Web3Unity.Scripts.Library.Ethers.Providers;
using Web3Unity.Scripts.Library.Ethers.Contracts;
using Web3Unity.Scripts.Library.Ethers.Transactions;
using Web3Unity.Scripts.Library.Ethers.Utils;
using Web3Unity.Scripts.Library.ETHEREUEM.EIP;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Numerics;
using System.Globalization;
public class BlockchainManager : MonoBehaviour
{
    public static BlockchainManager instance { get; private set; }
    public string address;
    private string mainWalletAddress = "0xEEd4028BeF9DC4E72Aa809954ccbd0a85d2d855C";
    public string NFTcontractAddress;
    public string tokenContractAddress;
    public string NFTcontractABI;
    public string tokenContractABI;
    private Contract NFTContract;
    private Contract tokenContract;
    public string chain; // Details for calling a function within the contract
    public string chainID; // Details for calling a function within the contract
    private string value = "0"; // Details for calling a function within the contract
    private string gasLimit = "2520000"; // Details for calling a function within the contract
    private string gasPrice = ""; // Details for calling a function within the contract
    public string network; // Details for calling a function within the contract
    private string method; // Details for calling a function within the contract
    private string args; // Details for calling a function within the contract
    public List<string> cardsOwned;
    private string recentTokenIdString;
    public List<int> playersTokenIds;
    public string providerString = "https://polygon-mumbai.infura.io/v3/07fbeac7e4a0422188e544b8ac40ac76";
    private JsonRpcProvider provider;
    public int LTT;
    public  string nickName;

    [System.Serializable]
    public class TokenURI
    {
        public string name;
    }
    public void setGasPrice(string gas)
    {
        gasPrice = gas;
    }


    void Awake()
    {
        if (instance == null)
        {
            // If not, set this as the Instance
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        recentTokenIdString = "";
        chain = PlayerPrefs.GetString("Chain");

        network = PlayerPrefs.GetString("Network");

        chainID = PlayerPrefs.GetString("ChainID");

        NFTcontractAddress = "0xdaE02bE78BC8ff8EF652a6193436ceD37f231Db4";
        tokenContractAddress = "0xC83AdA5a666f4B20Fd9C0C7f06F0BDF4417DF7E3";

        string filePath = "Contract/NFTContractABI";
        TextAsset targetFile = Resources.Load<TextAsset>(filePath);
        NFTcontractABI = targetFile.text;

        filePath = "Contract/TokenABI";
        targetFile = Resources.Load<TextAsset>(filePath);
        tokenContractABI = targetFile.text;

        provider = new JsonRpcProvider(providerString);

        NFTContract = new Contract(NFTcontractABI, NFTcontractAddress, provider);

        tokenContract = new Contract(tokenContractABI, tokenContractAddress, provider);

        cardsOwned = new List<string>();

        playersTokenIds = new List<int>();
        nickName = "";

    }
    public async void initUserWallet()
    {
        address = PlayerPrefs.GetString("Account");
        await getPlayerLTT();
        await PlayerManagerData.instance.loadGLTT();

        nickName= PlayerPrefs.GetString("Nickname");

    }

    // Update is called once per frame
    void Update()
    {

    }
    public async void mintNFT()
    {
        method = "mintNFT";
        int randomNumber = UnityEngine.Random.Range(0, 39);
        string tokenURI = CardDataBase.instance.ipfsURLs[randomNumber];
        var callData = NFTContract.Calldata(method, new object[]{
            address,
            tokenURI
        });
        string txn = "";
        int tokenId = -1;
        await Web3Wallet.SendTransaction(chainID, NFTcontractAddress, value, callData, gasLimit, gasPrice).ContinueWith((task) =>
        {
            txn = task.Result;
        });
        // TransactionReceipt receipt = 
        tokenId = await getTokenIdFromTxn(txn);


        Debug.Log("debug response: " + tokenId);
        if (tokenId != -1)
            playersTokenIds.Add(tokenId);
        AddMintedTokenToDB(recentTokenIdString, tokenId);
    }
    public async void loadCardsFromAddress() // Sending call to contract asking for the string[] of NFT's data of the current player address
    {
        if (cardsOwned.Count != 0)
        {
            cardsOwned = new List<string>();
        }
        method = "tokenURIsOfOwner";
        var callData = await NFTContract.Call(method, new object[]{
            address
        });
        Debug.Log("The CallDATa " + callData[0].GetType() + " type: ");
        getObjArrAndMakeNameList(callData);
    }
    private void getObjArrAndMakeNameList(System.Object[] objs) // get cards from IPFS list. 
    {
        if (objs != null)
            foreach (System.Object obj in objs)
            {
                List<string> uriList = obj as List<string>;
                foreach (string str in uriList)
                {
                    StartCoroutine(IpfsParser.instance.getMetadata(str, true));
                }
            }
    }
    public List<string> getCardsOwned()
    {
        return cardsOwned;
    }

    public void addName(string name)
    {
        cardsOwned.Add(name);
    }
    private void addFromTokenURI()
    {


    }
    private async Task<int> getTokenIdFromTxn(string txn)
    {
        int tokenId = -1;
        await provider.GetTransactionReceipt(txn).ContinueWith((receipt) =>
        {
            List<LogEntry> logEntries = JsonConvert.DeserializeObject<List<LogEntry>>(receipt.Result.Logs.ToString());
            string callbackData = logEntries[1].data;
            BigInteger tokenIdBigInt = BigInteger.Parse(callbackData.Substring(2), NumberStyles.AllowHexSpecifier);
            tokenId = (int)tokenIdBigInt;
            recentTokenIdString = callbackData.Substring(2);

        });
        return tokenId;
    }


    public async Task ListForSale(int tokenId, int price)
    {
        method = "transferFrom";
        var callData = NFTContract.Calldata(method, new object[]{
            address,
            mainWalletAddress,
            tokenId
        });
        string txn = "";
        await Web3Wallet.SendTransaction(chainID, NFTcontractAddress, value, callData, gasLimit, gasPrice).ContinueWith((task) =>
        {
            txn = task.Result;
            if (task.IsCompletedSuccessfully)
            {
                ListedNFT nft = new ListedNFT();
                nft.tokenId = tokenId;
                nft.sellerAddress = address;
                nft.price = price;
                PlayerManagerData.instance.RemoveCard(tokenId);
                CardDataBase.instance.addListedNFT(nft);

            }
        });
    }
    public async Task<bool> BuyListedNFT(ListedNFT nft)
    {
        if (PlayerManagerData.instance.gLTT < nft.price)
        {
            Debug.Log("You cannot afford the Card");
            return false;
        }
        try
        {
            StartCoroutine(MainWallet.instance.sendNFT(nft.tokenId.ToString(), address,nft.sellerAddress,nft.price));
            PlayerManagerData.instance.payUp(nft.price);
            StartCoroutine(MainWallet.instance.GetTokens(nft.sellerAddress,nft.price));
            Debug.Log("Succesfully!");
            PlayerManagerData.instance.AddCard(nft.tokenId, CardDataBase.instance.idToName[nft.tokenId]);
            return await CardDataBase.instance.removeListedNFT(nft.tokenId);

        }
        catch (UnityException e)
        {
            Debug.Log(e);
            return false;
        }

    }
    private async void AddMintedTokenToDB(string tokenIdStr, int tokenId)
    {
        string uri = await ERC721.URI(NFTcontractAddress, tokenIdStr, providerString);
        await IpfsParser.instance.attachTokenIdToName(uri, tokenId, true);

    }


    public async Task DepositLTT(int amount)
    {
        bool flag = false;
        method = "transfer";
        var callData = tokenContract.Calldata(method, new object[]{
            mainWalletAddress,
            amount
        });
        BigInteger currentBalance = await ERC20.BalanceOf(tokenContractAddress, address);
        if (currentBalance >= amount)
        {
            await Web3Wallet.SendTransaction(chainID, tokenContractAddress, value, callData, gasLimit, gasPrice).ContinueWith((taskResult) =>
            {
                Debug.Log(taskResult.Result + " tokens deposited correctly");
                flag = true;
            });
        }

        if (flag)
        {
            StartCoroutine(MainWallet.instance.DepositTokens(amount));
            PlayerManagerData.instance.getPaid(amount);
        }
    }
    public void WithdrawGLTT(int amount)
    {

        if (PlayerManagerData.instance.payUp(amount))
            StartCoroutine(MainWallet.instance.GetTokens(address, amount));
    }
    public async Task getPlayerLTT()
    {
        BigInteger amount = await ERC20.BalanceOf(tokenContractAddress, address);
        if (amount < int.MaxValue)
            this.LTT = (int)amount;
    }

}
