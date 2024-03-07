using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateCurrencyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currencyText;

    public PlayerStats player;

    // Update is called once per frame
    void Update()
    {
        currencyText.text = $"{player.CurrentCurrency}";
    }
}