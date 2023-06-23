using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class MyWallet : MonoBehaviour
{
    public Button depositButton, withdrawButton, confirmDeposit, confirmWithdraw, closeDeposit, closeWithdraw;
    public TextMeshProUGUI gLTT_Text, LTT_Text;
    public TMP_InputField withdrawInput, depositInput;
    public GameObject popUpWindowDeposit;
    public GameObject popUpWindowWithDraw;

    // Start is called before the first frame update
    void Start()
    {
        closeDeposit.onClick.AddListener(closeWindowDeposite);
        closeWithdraw.onClick.AddListener(closeWindowWithdraw);
        depositButton.onClick.AddListener(Deposit);
        withdrawButton.onClick.AddListener(Withdraw);
        confirmDeposit.onClick.AddListener(ConfirmDeposit);
        confirmWithdraw.onClick.AddListener(ConfirmWithdraw);
        LTT_Text.text = BlockchainManager.instance.LTT + "";
        gLTT_Text.text = PlayerManagerData.instance.gLTT + "";
        popUpWindowDeposit.SetActive(false);
        popUpWindowWithDraw.SetActive(false);

    }

    private void ConfirmWithdraw()
    {
        int amount;
        string str = Regex.Replace(withdrawInput.text, "[^0-9]", "");

        if (int.TryParse(str, out amount))
        {
            if (amount <= 0)
            {
                Debug.Log("Error: number is less than 0");
            }
            else
            {
                BlockchainManager.instance.WithdrawGLTT(amount);

                BlockchainManager.instance.LTT += amount;
                updateText();
            }
        }
        else
        {
            Debug.Log("Error: string contains non-numeric characters " + withdrawInput.text);
        }

    }

    private async void ConfirmDeposit()
    {
        int amount;
        string str = Regex.Replace(depositInput.text, "[^0-9]", "");

        if (int.TryParse(str, out amount))
        {
            if (amount <= 0)
            {
                Debug.Log("Error: number is less than 0");
            }
            else
            {
                await BlockchainManager.instance.DepositLTT(amount);
                BlockchainManager.instance.LTT -= amount;
                updateText();
            }
        }
        else
        {
            Debug.Log("Error: string contains non-numeric characters " + depositInput.text);
        }

    }

    private void closeWindowWithdraw()
    {
        popUpWindowWithDraw.SetActive(false);

    }

    private void closeWindowDeposite()
    {
        popUpWindowDeposit.SetActive(false);
    }

    private void Withdraw()
    {
        popUpWindowWithDraw.SetActive(true);
    }

    private void Deposit()
    {
        popUpWindowDeposit.SetActive(true);
    }

    private void updateText()
    {
        LTT_Text.text = BlockchainManager.instance.LTT + "";
        gLTT_Text.text = PlayerManagerData.instance.gLTT + "";

    }

    // Update is called once per frame
    void Update()
    {

    }
}
