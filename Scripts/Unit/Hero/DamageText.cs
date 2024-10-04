using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class DamageText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI txtUi;
    [SerializeField, ReadOnlyInspector]
    private int iDamge, iCode;
    [SerializeField, ReadOnlyInspector]
    private DamageType eType;
    private Vector2 v2RandomPos;

    void Awake()
    {
        txtUi = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        Invoke(nameof(Disable), 0.5f);
    }

    public void SetDamage(int _iDamge, DamageType _eDmgType, bool _bIsCritical, bool _bIsEvation, int _iCode)
    {
        iDamge = _iDamge;
        eType = _eDmgType;
        iCode = _iCode;

        float _fFontSize = 18f;

        if (_iDamge < 50)
            _fFontSize = 20f;
        else if (_iDamge >= 50)
            _fFontSize = 24f;
        else if (_iDamge >= 100)
            _fFontSize = 30;
        else if (_iDamge > 500)
            _fFontSize = 32f;

        if (_bIsCritical)
            _fFontSize *= 1.5f;
        if (_bIsEvation)
            _fFontSize = 20f;

        Color _color = Color.white;

        switch (_eDmgType)
        {
            case DamageType.AD:
                _color = Color.red;
                if (_bIsCritical)
                    _color = new Color(0.8f, 0, 0);
                break;
            case DamageType.AP:
                _color = Color.blue;
                if (_bIsCritical)
                    _color = new Color(0, 0, 0.8f);
                break;
            case DamageType.True:
                _color = Color.white;
                break;
        }

        if (_bIsEvation)
            _color = Color.white;

        txtUi.fontSize = _fFontSize;
        txtUi.color = _color;
        txtUi.text = $"{_iDamge}";
        if (_bIsEvation)
            txtUi.text = "MISS";
        txtUi.rectTransform.anchoredPosition = Vector2.zero;
        txtUi.rectTransform.DOJumpAnchorPos(new Vector2(Random.Range(-25f, 25f), Random.Range(0, 25f)), 10f, 1, 0.25f);
        txtUi.rectTransform.localScale = Vector2.one * 2f;
        txtUi.rectTransform.DOScale(Vector2.one, 0.25f);

        v2RandomPos = new Vector2(Random.Range(-25f, 25f), Random.Range(0, 25f));
    }

    public void ExtendAble(int _iAddDamage)
    {
        txtUi.DOKill();
        txtUi.rectTransform.anchoredPosition = v2RandomPos;
        CancelInvoke(nameof(Disable));
        iDamge += _iAddDamage;
        txtUi.text = iDamge.ToString();
        Invoke(nameof(Disable), 0.5f);
    }

    private void Disable()
    {
        txtUi.rectTransform.position = Vector2.zero;
        gameObject.SetActive(false);
    }

    public int code { get => iCode; }
}
