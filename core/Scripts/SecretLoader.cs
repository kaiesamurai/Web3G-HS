using SecretNET;
using SecretNET.Common;
using SecretNET.Crypto;
using SecretNET.Crypto.BIP39;
using SecretNET.Query;
using SecretNET.Tx;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using TransformsAI.Unity.Grpc.Web;
using UnityEngine;

public class SecretLoader : MonoBehaviour
{
    public static SecretLoader Instance { get; private set; }

    private bool walletSetupSceneInitialized;

    private Wallet wallet;
    private SecretNetworkClient queryClient;
    private SecretNetworkClient client;
    private Dictionary<string, string> codeHashes;

    private ConnectWalletWindow connectWalletWindow;
    private GenerateWalletPopUp generateWalletPopUp;
    private FundWalletPopUp fundWalletPopUp;
    private ConfirmTxPopUp confirmTxPopUp;

    #region PUBLIC PROPERTIES
    public Wallet Signer { get {  return wallet; } }
    public Queries Queries { get { return queryClient.Query; }  }
    #endregion

    [Header("Initialization")]
    [Tooltip("Ensure that the UI is displayed by instantiating a canvas and all necessary prefabs (Windows, popups...)")]
    public bool loadInterfaceOnStartup = true;
    [Tooltip("What scene to transition to after a successful wallet initialization")]
    public string postInitScene;
    [Tooltip("Pick the network you want to connect to")]
    public NetworkType networkType = NetworkType.Testnet;

    void Awake()
    {
        if (walletSetupSceneInitialized) return;
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else if (Instance == null)
            Instance = this;

        DontDestroyOnLoad(gameObject);
        
        if (loadInterfaceOnStartup)
        {
            var canvas = FindFirstObjectByType<Canvas>() ?? new GameObject("Canvas").AddComponent<Canvas>();
            connectWalletWindow = canvas.transform.Find("ConnectWalletWindow").GetComponent<ConnectWalletWindow>() ?? GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/SCRT/ConnectWalletWindow")).GetComponent<ConnectWalletWindow>();
            generateWalletPopUp = canvas.transform.Find("GenerateWalletPopUp").GetComponent<GenerateWalletPopUp>() ?? GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/SCRT/GenerateWalletPopUp")).GetComponent<GenerateWalletPopUp>();
            fundWalletPopUp = canvas.transform.Find("FundWalletPopUp").GetComponent<FundWalletPopUp>() ?? GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/SCRT/FundWalletPopUp")).GetComponent<FundWalletPopUp>();
        }
        confirmTxPopUp = Resources.Load<ConfirmTxPopUp>("SCRT/Prefabs/ConfirmTxPopUp");
        queryClient = ConnectToSecret(null);
        connectWalletWindow.Initialized += (e) =>
        {
            connectWalletWindow.Show();
        };
        codeHashes = new Dictionary<string, string>();
        walletSetupSceneInitialized = true;
    }

    private SecretNetworkClient ConnectToSecret(Wallet wallet)
    {
        var grpcUrl = "https://grpc.pulsar.scrttestnet.com";
        SecretNET.SecretNetworkClient client = new SecretNetworkClient(
            new SecretNET.Common.CreateClientOptions(grpcUrl, "pulsar-3", wallet, wallet?.Address),
            new Grpc.Net.Client.GrpcChannelOptions()
            {
                HttpHandler = UnityGrpcWebHandler.Create(),
            },
            null,
            (o) =>
            {
                var channel = UnityGrpcWeb.MakeChannel(grpcUrl, options: o);
                return channel;
            }
        );
        if (wallet != null)
        {
            client.GasEstimationMultiplier = 2;
        }
        return client;
    }

    #region PUBLIC METHODS
    public async Task<string> LoadRandomWallet()
    {
        var wordlist = Wordlist.English;
        var mnemonic = new Mnemonic(wordlist);
        wallet = await Wallet.Create(mnemonic: mnemonic.ToString());
        return mnemonic.ToString();
    }

    public async Task<bool> IsWalletInitialized()
    {
        var res = await queryClient.Query.Bank.Balance(new Cosmos.Bank.V1Beta1.QueryBalanceRequest()
        {
            Address = wallet.Address,
            Denom = "uscrt",
        });
        return res.Balance.Amount != "0";
    }

    public bool IsSetupCompleted()
    {
        return queryClient != null && client != null && wallet != null;
    }

    public void ConnectSigner()
    {
        if (wallet == null)
        {
            Debug.LogError("No signer wallet initialized.");
            return;
        }
        client = ConnectToSecret(wallet);
        client.TransactionApprovalCallback = TransactionApprovalCallback;
    }

    public async Task SignTransaction(MsgBase[] msgs, Action<SecretTx> onComplete = null) 
    {
        var tx = await client.Tx.Broadcast(msgs, new TxOptions() { BroadcastCheckIntervalMs = 600, GasLimit = 1_144_000, WaitForCommit = true });
        onComplete?.Invoke(tx);
    }

    public async Task<SecretQueryContractResult<T>> QueryContractState<T>(string contract, object msg) where T : class
    {
        if (!codeHashes.ContainsKey(contract))
            codeHashes.Add(contract, await queryClient.Query.Compute.GetCodeHash(contract));
        return await client.Query.Compute.QueryContract<T>(contract, msg, codeHashes[contract]);
    }
    #endregion


    private async Task<UserApprovalDecision> TransactionApprovalCallback(TransactionApprovalData data)
    {
        var localCanvas = FindObjectOfType<Canvas>();
        var popup = Instantiate(confirmTxPopUp, localCanvas.transform);
        popup.showOnInitialize = true;
        popup.Initialize();
        popup.gameObject.SetActive(true);
        popup.AssignTxData(data);
        popup.Show();
        return 
            await popup.WaitForResult();
    }

    public enum NetworkType
    {
        Mainnet,
        Testnet,
        LocalDev,
    }
}
