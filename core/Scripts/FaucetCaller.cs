using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FaucetCaller : MonoBehaviour
{
    SecretLoader SecretLoader;
    public void CallFaucetForSigner()
    {
        SecretLoader = FindObjectOfType<SecretLoader>();
        if (SecretLoader.networkType != SecretLoader.NetworkType.Testnet)
            return;
        UnityWebRequest request = new UnityWebRequest($"https://secretfaucet.azurewebsites.net/faucet?address={SecretLoader.Signer.Address}");
        var res = request.SendWebRequest();
        
    }
}
