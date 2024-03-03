using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateCurrencyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currencyText;

    public PlayerStats player;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // currencyText.text = player.CurrenCurrency;
    }
}