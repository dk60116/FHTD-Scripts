using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlotHUD : MonoBehaviour
{
    [SerializeField, ReadOnlyInspector]
    private Image[] imgFrames, imgIcons;

    [ContextMenu("Init")]
    private void Init()
    {
        imgFrames = new Image[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            imgFrames[i] = transform.GetChild(i).GetComponent<Image>();
            imgIcons[i] = transform.GetChild(i).GetChild(0).GetComponent<Image>();
        }
    }

    public void SetItem(int _iCardId)
    {
        switch (CardManager.instance.GetCardWithID(_iCardId).stat.eCardClass)
        {
            case CardClass.Weapon:
                imgIcons[0].sprite = CardManager.instance.GetCardWithID(_iCardId).stat.imgEqupIcon;
                imgFrames[0].gameObject.SetActive(true);
                break;
            case CardClass.Armor:
                imgIcons[1].sprite = CardManager.instance.GetCardWithID(_iCardId).stat.imgEqupIcon;
                imgFrames[1].gameObject.SetActive(true);
                break;
            case CardClass.Accessories:
                imgIcons[2].sprite = CardManager.instance.GetCardWithID(_iCardId).stat.imgEqupIcon;
                imgFrames[2].gameObject.SetActive(true);
                break;
        }
    }

    public void DisableAllItem()
    {
        foreach (var item in imgFrames)
            item.gameObject.SetActive(false);
    }
}
