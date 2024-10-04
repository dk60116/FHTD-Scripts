using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class ConfirmPanel : MonoBehaviour
{
    [SerializeField, ReadOnlyInspector]
    private TextMeshProUGUI txtConfrim;
    [SerializeField, ReadOnlyInspector]
    private Button uiOkButton, uiCancelButton;

    void Awake()
    {
        uiCancelButton.onClick.AddListener(() => gameObject.SetActive(false));
    }

    [ContextMenu("Init")]
    private void Init()
    {
        txtConfrim = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        uiCancelButton = GetComponentsInChildren<Button>()[0];
        uiOkButton = GetComponentsInChildren<Button>()[1];
    }

    public void OpenPaenl(string _strText, UnityAction _cEvent, bool _bCancelButton)
    {
        uiCancelButton.gameObject.SetActive(_bCancelButton);

        txtConfrim.text = _strText;

        uiOkButton.onClick.RemoveAllListeners();
        if (_cEvent != null)
            uiOkButton.onClick.AddListener(_cEvent);
        uiOkButton.onClick.AddListener(() => gameObject.SetActive(false));
    }
}
