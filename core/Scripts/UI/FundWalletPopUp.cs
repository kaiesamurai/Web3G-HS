using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FundWalletPopUp : WindowBase
{
    [SerializeField] private TextMeshProUGUI addressText;
    [SerializeField] private Image qrCodeField;

    private UnityWebRequestAsyncOperation qrCodeOperation;
    private SecretLoader secretLoader;
    public override void Initialize()
    {
        secretLoader = SecretLoader.Instance;
        UnityWebRequest qrCode = UnityWebRequestTexture.GetTexture($"https://api.qrserver.com/v1/create-qr-code/?data={secretLoader.Signer.Address}&size=130x130");
        qrCodeOperation = qrCode.SendWebRequest();
        qrCodeOperation.completed += (a) =>
        {
            var tx2d = DownloadHandlerTexture.GetContent(qrCode);
            qrCodeField.sprite = Sprite.Create(tx2d, new Rect(0, 0, tx2d.width, tx2d.height), Vector2.one /2);
        };
    }

    protected async override void OnShow()
    {
        addressText.SetText(secretLoader.Signer.Address);
        while (!await secretLoader.IsWalletInitialized().ConfigureAwait(false))
        {
            await Task.Delay(2500).ConfigureAwait(false);
        }
        Close(WindowResult.Success, null);
    }

    public void CopyAddressToClipboard()
    {
        GUIUtility.systemCopyBuffer = secretLoader.Signer.Address;
    }
}
