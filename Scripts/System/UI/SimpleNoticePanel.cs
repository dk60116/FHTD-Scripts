using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SimpleNoticePanel : MonoBehaviour
{
    [SerializeField, ReadOnlyInspector]
    private TextMeshProUGUI txtNotice;

    [ContextMenu("Init")]
    private void Init()
    {
        txtNotice = transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetNotice(string _strNotice)
    {
        txtNotice.text = _strNotice;
        LayoutRebuilder.ForceRebuildLayoutImmediate(txtNotice.rectTransform);

        StopAllCoroutines();
        StartCoroutine(AutoDisableCoroutine());
    }

    IEnumerator AutoDisableCoroutine()
    {
        yield return new WaitForSeconds(3f);

        gameObject.SetActive(false);
    }
}
