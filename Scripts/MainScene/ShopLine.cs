using PlayFab.GroupsModels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ShopLine : MonoBehaviour
{
    public MainShopComponents component;
    [SerializeField]
    private List<Button> btn;
    [ReadOnlyInspector]
    public List<string> contentId;
    [SerializeField]
    private List<RawImage> imageList;   
    [SerializeField]
    private List<TextMeshProUGUI> titleText, costText;
    [SerializeField]
    private List<CanvasGroup> cg;
    [SerializeField]
    private List<GameObject> soldOutImage;

    public void SetInfo(MainShopComponents _component)
    {
        component = _component;
        contentId = new List<string>();

        int _count = _component.contentId.Count;

        for (int i = 0; i < _count; i++)
        {
            int _i = i;

            bool _soldOut = SocialManager.instance.playerInfo.purchasedList.Any(x => x == _component.contentId[_i]);

            btn[_i].interactable = !_soldOut;
            soldOutImage[_i].SetActive(_soldOut);
            cg[_i].alpha = _component.canSoldOut[i] && _soldOut ? 0.7f : 1f;
            contentId.Add(_component.contentId[i]);
            titleText[i].text = _component.contentName[i];
            costText[i].text = _component.cost[i].ToString();
            MainSceneManager.instance.ShopImageDownload(_component.imageLink[_i], imageList[_i]);
            Debug.LogError(btn[_i]);
            btn[i].onClick.RemoveAllListeners();
            if (!_soldOut)
                btn[i].onClick.AddListener(() => MainSceneManager.instance.mainShopPanel.OpenConfirm(component, imageList[_i], _i));
        }

        gameObject.SetActive(_component.tag.Contains(MainSceneManager.instance.mainShopPanel.sideTagIndex));
    }

    public void SetSoldOut(int _num, bool _sold)
    {
        btn[_num].interactable = !_sold;
        soldOutImage[_num].gameObject.SetActive(_sold);
        cg[_num].alpha = _sold ? 0.7f : 1f;
    }
}
