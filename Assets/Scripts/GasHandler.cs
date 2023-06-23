using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Web3Unity.Scripts.Library.Ethers.Providers;
using Web3Unity.Scripts.Library.Ethers.Contracts;
using System.Threading.Tasks;

public class GasHandler : MonoBehaviour
{
    public static GasHandler instance;
    private void Awake(){
        if (instance == null)
        {
            // If not, set this as the Instance
            instance = this;
        }
    }
    public async Task<string> GasPrice()
    {
        var provider = new JsonRpcProvider(BlockchainManager.instance.providerString);
        var gasPrice = await provider.GetGasPrice();
        return gasPrice.ToString();
    }
}