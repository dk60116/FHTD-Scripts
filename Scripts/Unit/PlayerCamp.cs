using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class PlayerCamp : Unit
{
    [SerializeField]
    private TextMeshProUGUI txtExpUp, txtLVUp, txtGoldUp;

    protected override void Awake()
    {
        base.Awake();

        txtExpUp.gameObject.SetActive(false);
        txtLVUp.gameObject.SetActive(false);
        txtGoldUp.gameObject.SetActive(false);
    }

    protected override void Start()
    {
        base.Start();
        InGameManager.instance.cPlayerController.placedUnits.Add(this);
    }

    public override void SetTile(Tile _cTile)
    {
        FindObjectOfType<InGameManager>().cAStar.SetTargetNode(_cTile);

        base.SetTile(_cTile);
    }

    public void SetTextUI()
    {
        txtExpUp = GameObject.Find("Camp Exp Text").GetComponent<TextMeshProUGUI>();
        txtLVUp = GameObject.Find("Camp LV Text").GetComponent<TextMeshProUGUI>();
        txtGoldUp = GameObject.Find("Camp Gold Text").GetComponent<TextMeshProUGUI>();
    }


    public void AbleExpUp(int _iValue)
    {
        txtExpUp.transform.position = Camera.main.WorldToScreenPoint(InGameManager.instance.GetPlayerCamp().transform.position);
        txtExpUp.text = $"Exp + {_iValue}";
        StopCoroutine(nameof(AbleExpUpCoroutine));
        StartCoroutine(nameof(AbleExpUpCoroutine));
    }

    IEnumerator AbleExpUpCoroutine()
    {
        yield return new WaitUntil(() => !txtGoldUp.gameObject.activeSelf);

        txtExpUp.gameObject.SetActive(true);
        txtExpUp.rectTransform.DOMove(Camera.main.WorldToScreenPoint(InGameManager.instance.GetPlayerCamp().transform.position + Vector3.up), 0.5f).OnComplete(() => txtExpUp.gameObject.SetActive(false));
    }
}
