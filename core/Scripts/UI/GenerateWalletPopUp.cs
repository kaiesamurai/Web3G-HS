using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GenerateWalletPopUp : WindowBase
{
    [SerializeField] TextMeshProUGUI mnemonicText;
    [SerializeField] Button connectButton;
    private SecretLoader secretLoader;
    private FundWalletPopUp fundWalletPopUp;
    public override async void Initialize()
    {
        connectButton.enabled = false;
        mnemonicText.SetText("Generating your new wallet...");
        secretLoader = FindObjectOfType<SecretLoader>();
        var mnemonic = await secretLoader.LoadRandomWallet();
        mnemonicText.SetText(mnemonic);
        fundWalletPopUp = WindowBase.GetSpecificInstanceOfType<FundWalletPopUp>();
        connectButton.enabled = true;
        connectButton.onClick.RemoveAllListeners();
        fundWalletPopUp.Success += (o, d) =>
        {
            Close(WindowResult.Success, null);
        };
        connectButton.onClick.AddListener(() =>
        {
            secretLoader.ConnectSigner();
            fundWalletPopUp.Show();
        });
    }
}
