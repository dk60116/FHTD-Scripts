using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrencyText : MonoBehaviour
{
    public CurrencyType currency;
    [SerializeField]
    private TextMeshProUGUI text;

    public void UpdateCount(int _count)
    {
        text.text = _count < 100000 ? _count.ToString() : "99999+";
    }
}
