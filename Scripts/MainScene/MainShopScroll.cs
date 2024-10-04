using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainShopScroll : MonoBehaviour
{
    [SerializeField]
    private MainShopPanel mainShop;
    [SerializeField,ReadOnlyInspector]
    private int sideTag;

    [SerializeField]
    private List<Toggle> tagToggleList;
    public ScrollRect scroll;

    void Awake()
    {
        for (int i = 0; i < tagToggleList.Count; i++)
        {
            int _i = i;
            tagToggleList[i].onValueChanged.AddListener((bool _on) => OnTagToggleHandler(_on, _i));
        }
    }

    private void OnEnable()
    {
        Invoke(nameof(OnEnableHandler), Time.deltaTime);
    }

    private void OnEnableHandler()
    {
        scroll.verticalNormalizedPosition = 1f;
    }

    private void OnTagToggleHandler(bool _on, int _index)
    {
        if (_on)
            mainShop.SetTag(_index);
    }

    public string GetSideToggleText(int _index)
    {
        return tagToggleList[_index].GetComponentInChildren<TextMeshProUGUI>().text;
    }

    public void TriggerTagToggle(int _index)
    {
        tagToggleList[_index].isOn = true;
    }
}
