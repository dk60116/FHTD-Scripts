using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckSettingPanel : MonoBehaviour
{
    [SerializeField]
    private List<CardFrameDP> cardFrameList;

    void OnEnable()
    {
        Invoke(nameof(EnableHandler), Time.deltaTime);
        for (int i = 0; i < CardManager.instance.cardFrameList.Count; i++)
            cardFrameList[i].UpdateFrame(CardManager.instance.cardFrameList[i].itemId);
    }

    private void EnableHandler()
    {
        foreach (var item in cardFrameList)
        {
            if (item.frame.itemId == SocialManager.instance.playerInfo.equipCardFrame)
            {
                item.OnToggle();
                return;
            }
        }

    }
}
