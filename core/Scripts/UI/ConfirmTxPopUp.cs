using SecretNET.Common;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ConfirmTxPopUp : WindowBase
{
    SecretLoader secretLoader;
    [SerializeField] private TextMeshProUGUI dataField;
    [SerializeField] private TextMeshProUGUI gasCostField;


    private TransactionApprovalData data;
    private UserApprovalDecision result;
    public override void Initialize()
    {
    }

    public void AssignTxData(TransactionApprovalData data)
    {
        this.data = data;
    }

    protected override void OnShow()
    {
        secretLoader = SecretLoader.Instance;
        var sb = new StringBuilder();
        foreach (var msg in data.Messages)
            sb.AppendLine(msg.Value?.ToString() ?? "none");
        dataField.SetText(sb.ToString());
        gasCostField.SetText($"{data.EstimatedGasFee/1000 / (double)1000} SCRT");
    }

    public async Task<UserApprovalDecision> WaitForResult()
    {
        while (result == null)
        {
            await Task.Delay(500);
        }
        return result;
    }

    public void ConfirmTx()
    {
        result = new UserApprovalDecision(true);
        Close(WindowResult.Success, null);
    }

    public void CancelTx()
    {
        result = new UserApprovalDecision(false);
        Close(WindowResult.Success, null);
    }
}
