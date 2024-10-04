using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDP_Open : CardDisplay
{
    [SerializeField]
    private Button btn;
    [ReadOnlyInspector]
    public bool isOpen;

    void Awake()
    {
        btn.onClick.AddListener(() => OpenCard());
    }

    public override void UpdateCardInfo(Card _cCard)
    {
        base.UpdateCardInfo(_cCard);

        isOpen = false;
        imgBack.color = Color.white;
        imgBack.gameObject.SetActive(true);
    }

    public void OpenCard()
    {
        imgBack.DOFade(0, 1f);
        isOpen = true;
        GameManager.instance.OpenCardInfo(cCard, MainSceneManager.instance.openCardSystem.infoArea.position, new Vector2(0, 0.5f));
    }
}
