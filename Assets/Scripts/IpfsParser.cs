using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class IpfsParser : MonoBehaviour
{
    public static IpfsParser instance { get; private set; }
    const int MaxRetries = 5;
    const float InitialWaitTime = 2f; // Initial wait time in seconds
    const float MaxWaitTime = 10f; // Maximum wait time in seconds
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    public IEnumerator getMetadata(string ipfsUrl, bool isForPlayer)
    {
        for (int attempt = 0; attempt <= MaxRetries; attempt++)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(ipfsUrl))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(www.error);
                    float waitTime = Mathf.Min(InitialWaitTime * Mathf.Pow(2, attempt), MaxWaitTime);
                    Debug.LogError($"Retrying in {waitTime} seconds...");

                    // Wait for the specified time before retrying
                    yield return new WaitForSeconds(waitTime);
                }
                else
                {
                    // Show results as text
                    Debug.Log(www.downloadHandler.text);

                    // Or retrieve results as binary data
                    byte[] results = www.downloadHandler.data;

                    string jsonString = System.Text.Encoding.Default.GetString(results);

                    // Parse the json string here
                    var nftMetadata = JsonUtility.FromJson<NFTMetadata>(jsonString);
                    Debug.Log("NFT Name: " + nftMetadata.name.ToString() + "  " + nftMetadata.attributes[0].trait_type.ToString() + "  " + nftMetadata.description.ToString());
                    if (isForPlayer)
                    {
                        BlockchainManager.instance.addName(nftMetadata.name);
                        PlayerManagerData.instance.myDeck.Add(nftMetadata.name);
                    }
                    
                    break;
                }
            }
        }
    }

    public IEnumerator attachTokenIdToName(string ipfsUrl, int tokenId, bool isPlayer = false)
    {
        for (int attempt = 0; attempt <= MaxRetries; attempt++)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(ipfsUrl))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(www.error);
                    float waitTime = Mathf.Min(InitialWaitTime * Mathf.Pow(2, attempt), MaxWaitTime);
                    Debug.LogError($"Retrying in {waitTime} seconds...");

                    // Wait for the specified time before retrying
                    yield return new WaitForSeconds(waitTime);
                }
                else
                {
                    // Show results as text
                    Debug.Log(www.downloadHandler.text);

                    // Or retrieve results as binary data
                    byte[] results = www.downloadHandler.data;

                    string jsonString = System.Text.Encoding.Default.GetString(results);

                    // Parse the json string here
                    var nftMetadata = JsonUtility.FromJson<NFTMetadata>(jsonString);
                    CardDataBase.instance.attachIdToName(tokenId, nftMetadata.name);


                    if (isPlayer)
                    {
                        PlayerManagerData.instance.AddCard(tokenId, nftMetadata.name);
                        PlayerManagerData.instance.myDeck.Add(nftMetadata.name);
                    }

                    break;
                }
            }
        }
    }


}
// [System.Serializable]
// public class NFTMetadata
// {
//     public Dictionary<string, int> attributes;
//     public string name;
//     public string description;
//     // and other fields...
// }

[System.Serializable]
public class Attribute
{
    public string trait_type;
    public int value;
}
[System.Serializable]
public class NFTMetadata
{
    public List<Attribute> attributes;
    public string description;
    public string name;
}

public class LogEntry
{
    public string address { get; set; }
    public string blockHash { get; set; }
    public string blockNumber { get; set; }
    public string data { get; set; }
    public string logIndex { get; set; }
    public bool removed { get; set; }
    public List<string> topics { get; set; }
    public string transactionHash { get; set; }
    public string transactionIndex { get; set; }
}