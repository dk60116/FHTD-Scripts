using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class HeroMarketSystem : MonoBehaviour
{
    [SerializeField]
    private PoolingManager cObjectPool;
    [SerializeField]
    private Button uiReroleButton;
    private List<MarketSlot> listSlot;

    [SerializeField, ReadOnlyInspector]
    private Sprite[] imgBGSprites;

    [SerializeField, ReadOnlyInspector]
    private Toggle uiLockToggle;

    void Awake()
    {
        listSlot = new List<MarketSlot>();

        for (int i = 0; i < transform.childCount; i++)
            listSlot.Add(transform.GetChild(i).GetComponent<MarketSlot>());
    }

    private void Start()
    {
        AddShopHeros();
        Invoke(nameof(Reroll), 0.05f);
    }

    [ContextMenu("Init")]
    private void Init()
    {
        imgBGSprites = Resources.LoadAll<Sprite>($"Illust/HeroBG");
        uiLockToggle = GameObject.Find("Rock Toggle").GetComponent<Toggle>();
    }

    private void AddShopHeros()
    {
        for (int i = 0; i < GameManager.instance.heroData.Count; i++)
        {
            cObjectPool.AddTargetObj(GameManager.instance.heroData[i].gameObject);

            switch (GameManager.instance.heroData[i].unitStat.eRank)
            {
                case UnitRank.None:
                    break;
                case UnitRank.Common:
                    cObjectPool.AddListCount(30);
                    break;
                case UnitRank.UnCommon:
                    cObjectPool.AddListCount(27);
                    break;
                case UnitRank.Rare:
                    cObjectPool.AddListCount(21);
                    break;
                case UnitRank.Epic:
                    cObjectPool.AddListCount(15);
                    break;
                case UnitRank.Legendary:
                    cObjectPool.AddListCount(9);
                    break;
                default:
                    break;
            }
        }
        
        cObjectPool.Init();
    }

    public void TryReroll()
    {
        if (InGameManager.instance.cPlayerController.TryUseGold(2))
        {
            uiLockToggle.isOn = false;
            Reroll();
        }
    }

    public void Reroll()
    {
        List<int> _listNums = new List<int>();
        List<float> _listPercentValue = new List<float>();

        for (int i = 0; i < (int)UnitRank.Legendary; i++)
            _listPercentValue.Add(0);

        int _iLevel = InGameManager.instance.cPlayerController.campStat.iCampLevel;

        for (int i = 0; i < (int)UnitRank.Legendary; i++)
        {
            if (i <= 0)
                _listPercentValue[i] = GameManager.instance.marketPercentage[i].list[_iLevel - 1];
            else
                _listPercentValue[i] = _listPercentValue[i - 1] + GameManager.instance.marketPercentage[i].list[_iLevel - 1];
        }

        for (int i = 0; i < listSlot.Count; i++)
        {
            _listNums.Add(Random.Range(0, 100));

            List<Hero> _listHero = GameManager.instance.heroData;
            int _iHeroCount = 0;

            if (_listNums[i] < (int)_listPercentValue[0])
                _listHero = _listHero.Where(h => h.unitStat.eRank == UnitRank.Common).ToList();
            else if (_listNums[i] < (int)_listPercentValue[1])
                _listHero = _listHero.Where(h => h.unitStat.eRank ==UnitRank.UnCommon).ToList();
            else if (_listNums[i] < (int)_listPercentValue[2])
                _listHero = _listHero.Where(h => h.unitStat.eRank == UnitRank.Rare).ToList();
            else if (_listNums[i] < (int)_listPercentValue[3])
                _listHero = _listHero.Where(h => h.unitStat.eRank == UnitRank.Epic).ToList();
            else
                _listHero = _listHero.Where(h => h.unitStat.eRank == UnitRank.Legendary).ToList();

            _iHeroCount = _listHero.Count;

            int _iRandomId = _listHero[Random.Range(0, _iHeroCount)].unitID;
            Hero _cPickRandomHero = null;

            _cPickRandomHero = GetHeroWithID(_iRandomId);

            listSlot[i].SetHero(_cPickRandomHero);
        }
    }

    public Hero GetHeroWithID(int _iId)
    {
        foreach (var item in cObjectPool.GetAllObjList())
        {
            if (item.GetComponent<Hero>().unitID == _iId)
                return item.GetComponent<Hero>();
        }

        return null;
    }
    
    public bool IsRerollLock()
    {
        return uiLockToggle.isOn;
    }

    public PoolingManager objPool { get => cObjectPool; }
    public Sprite[] heroBGImages { get => imgBGSprites; }
}
