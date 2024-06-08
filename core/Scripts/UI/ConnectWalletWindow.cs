using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConnectWalletWindow : WindowBase
{
    [SerializeField] private Button generateWalletButton;
    [SerializeField] private Button importWalletButton;

    private SecretLoader secretLoader;
    private GenerateWalletPopUp generateWalletPopUp;

    public override void Initialize()
    {        secretLoader = FindAnyObjectByType<SecretLoader>();
        generateWalletButton.onClick.RemoveAllListeners();
        generateWalletButton.onClick.AddListener(() =>
        {
            generateWalletPopUp = WindowBase.GetSpecificInstanceOfType<GenerateWalletPopUp>();
            generateWalletPopUp.Show();
            generateWalletPopUp.Success += (o, ea) =>
            {
                Close(WindowResult.Success, null);
                PostWalletInitialize();
            };
        });
    }

    private void PostWalletInitialize()
    {
        if (secretLoader.IsSetupCompleted())
        {
            SceneManager.LoadScene(secretLoader.postInitScene, LoadSceneMode.Single);
            Debug.Log(SceneManager.GetActiveScene());
        }
    }



}
