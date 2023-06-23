using System.Text;
using Nethereum.Signer;
using Nethereum.Util;
using UnityEngine;
using UnityEngine.UI;
using Web3Unity.Scripts.Library.Web3Wallet;
using TMPro;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoginScript : MonoBehaviour
{
    public Toggle rememberMe;
    public string userAddress;
    public Button loginButton;
    ProjectConfigScriptableObject projectConfigSO = null;
    public TMP_InputField nickName;

    public GameObject loginCanvas;
    public GameObject menuCanvas;
    public GameObject instructions;
    private void Awake()
    {
        loginButton.interactable = true;  // Disable the button initially
    }

    private void EnableLoginButton()
    {
        // Enable the login button
//        loginButton.interactable = true;
    }
    private void Start()
    {
        // change this if you are implementing your own sign in page

        Web3Wallet.url = "https://chainsafe.github.io/game-web3wallet/";
        // loads the data saved from the editor config
        projectConfigSO = (ProjectConfigScriptableObject)Resources.Load("ProjectConfigData", typeof(ScriptableObject));
        PlayerPrefs.SetString("ProjectID", projectConfigSO.ProjectID);
        PlayerPrefs.SetString("ChainID", projectConfigSO.ChainID);
        PlayerPrefs.SetString("Chain", projectConfigSO.Chain);
        PlayerPrefs.SetString("Network", projectConfigSO.Network);
        PlayerPrefs.SetString("RPC", projectConfigSO.RPC);
        // if remember me is checked, set the account to the saved account
        userAddress = PlayerPrefs.GetString("Account");
        if (PlayerPrefs.HasKey("RememberMe") && PlayerPrefs.HasKey("Account"))
            if (PlayerPrefs.GetInt("RememberMe") == 1 && PlayerPrefs.GetString("Account") != "")
            {
                //loginCanvas.SetActive(false);  // Hide the login canvas
                // menuCanvas.SetActive(true);
                // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }


    }
    public async void OnLogin()
    {
        //instructions.SetActive(true);

        // if (string.IsNullOrWhiteSpace(nickName.text))
        // {
        //     Debug.Log("Please enter a name");
        //     return;
        // }
        // get current timestamp
        var timestamp = (int)System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1)).TotalSeconds;
        // set expiration time
        var expirationTime = timestamp + 60;
        // set message
        var message = expirationTime.ToString();
        // sign message
        var signature = await Web3Wallet.Sign(message);
        // verify account
        var account = SignVerifySignature(signature, message);
        var now = (int)System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1)).TotalSeconds;
        // validate
        if (account.Length == 42 && expirationTime >= now)
        {
            // save account
            PlayerPrefs.SetString("Account", account);
            // save NickName 
            if (!string.IsNullOrWhiteSpace(nickName.text))
                PlayerPrefs.SetString("Nickname", nickName.text);
            if (rememberMe.isOn)
                PlayerPrefs.SetInt("RememberMe", 1);
            else
                PlayerPrefs.SetInt("RememberMe", 0);
            print("Account: " + account);
            await commitAfterLoginAsync();

        }
    }
    public string SignVerifySignature(string signatureString, string originalMessage)
    {
        var msg = "\x19" + "Ethereum Signed Message:\n" + originalMessage.Length + originalMessage;
        var msgHash = new Sha3Keccack().CalculateHash(Encoding.UTF8.GetBytes(msg));
        var signature = MessageSigner.ExtractEcdsaSignature(signatureString);
        var key = EthECKey.RecoverFromSignature(signature, msgHash);
        return key.GetPublicAddress();
    }
    public void Update()
    {
        if (FireBaseManager.instance.IsFirebaseInitialized)
        {
            EnableLoginButton();
        }
    }

    private async Task commitAfterLoginAsync()
    {
        FireBaseManager.instance.SaveNewUser(userAddress, nickName.text);
        //loginCanvas.SetActive(false);
        //menuCanvas.SetActive(true);
        await GasHandler.instance.GasPrice().ContinueWith((gas) =>
        {
            BlockchainManager.instance.setGasPrice(gas.Result);
        });

        await CardDataBase.instance.InitFromDataBase();



        Debug.Log(BlockchainManager.instance.NFTcontractAddress + "line 109");
        BlockchainManager.instance.initUserWallet();
        BlockchainManager.instance.loadCardsFromAddress();

        SceneManager.LoadScene(1);


    }
}