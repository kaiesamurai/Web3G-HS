using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SignerInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI walletAddressField;
    // Start is called before the first frame update
    void Start()
    {
        var addr = SecretLoader.Instance.Signer.Address;
        var formatted = $"{addr.Substring(0, 7)}..{addr.Substring(addr.Length - 5)}";
        walletAddressField.SetText(formatted);
    }
}
