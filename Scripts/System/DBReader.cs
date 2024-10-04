using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class DBReader : MonoBehaviour
{
    private GameManager cGameManager;
    private CardManager cCardManager;

    [SerializeField]
    private TextAsset txtHeroCSV, txtSynergyACSV, txtDefSkillCSV, txtAtkSkillCSV, txtCardCSV, txtMarketPercentCSV, txtChapterInfoCSV, txtStageUnitsCSV;

    private string[,] strHeroData, strSynergyData_D, strStnergyData_A, strDefSkillData, strAtkSkillData, strCardData, strMarketPercentData, strChapterData, strStageUnitsData;

    void Start()
    {
        ReadCSV();
    }

    [ContextMenu("ReadCSV")]
    private void ReadCSV()
    {
        SoltData(txtHeroCSV, out strHeroData);
        SoltData(txtSynergyACSV, out strSynergyData_D);
        SoltData(txtDefSkillCSV, out strDefSkillData);
        SoltData(txtAtkSkillCSV, out strAtkSkillData);
        SoltData(txtCardCSV, out strCardData);
        SoltData(txtMarketPercentCSV, out strMarketPercentData);
        SoltData(txtChapterInfoCSV, out strChapterData);
        SoltData(txtStageUnitsCSV, out strStageUnitsData);

        UploadHeroData();
        UploadSynergyData();
        UploadStageData();
        UploadCardData();
        UploadMarketPercentData();
        UploadChapterData();
        UploadStageData();

        Debug.Log("Loaded data complete");
    }


    private void SoltData(TextAsset _txtAsset, out string[,] _strString)
    {
        byte[] _bytesForEncoding = Encoding.UTF8.GetBytes(_txtAsset.text);
        string _encodedString = Convert.ToBase64String(_bytesForEncoding);

        byte[] _decodedBytes = Convert.FromBase64String(_encodedString);
        string _decodedString = Encoding.UTF8.GetString(_decodedBytes);

        string[,] _strSentence;
        int _iLineSize, _iRowSize;

        string _strCrtText = _decodedString.Substring(0, _txtAsset.text.Length - 1);
        string[] _strLine = _strCrtText.Split('\n');

        _iLineSize = _strLine.Length;
        _iRowSize = _strLine[0].Split(',').Length;

        _strSentence = new string[_iLineSize, _iRowSize];

        for (int i = 0; i < _iLineSize; i++)
        {
            string[] _strRow = _strLine[i].Split(',');

            for (int j = 0; j < _iRowSize; j++)
                _strSentence[i, j] = _strRow[j];
        }

        _strString = _strSentence;
    }

    private void UploadHeroData()
    {
        //AssetBundle.UnloadAllAssetBundles(false);
        //AssetBundle _cBundle = AssetBundle.LoadFromFile("C:/Users/g/Desktop/HeroData/hero");
        //GameManager.Instance.BuildDebugLog(File.Exists("C:/Users/g/Desktop/HeroData/hero"));
        cGameManager = FindObjectOfType<GameManager>(true);
        List<Hero> _listHero = cGameManager.heroData = new List<Hero>();

        for (int i = 0; i < strHeroData.GetLength(0) - 1; i++)
        {
            int _iCount = i;

            UnitStatus _sUnitStat = new UnitStatus();
            HeroStatus _sHeroStat = new HeroStatus();

            int _iHeroID = int.Parse(strHeroData[i + 1, 0]);
            string _strHeroName = strHeroData[i + 1, 1];
            //_listHero.Add(_cBundle.LoadAsset<GameObject>($"{_strHeroName}_{_iHeroID}").GetComponent<Hero>());
            _listHero.Add(Resources.Load<Hero>($"Unit/Hero/{_strHeroName}_{_iHeroID}"));
            _listHero[i].heroNumber = _iCount;
            _listHero[i].unitID = _iHeroID;
            _listHero[i].unitName = _strHeroName;
            _listHero[i].unitIllust = Resources.Load<Sprite>($"Illust/HeroIllust/{_strHeroName}_{_iHeroID}_Illust");
            _listHero[i].faceIllust = Resources.Load<Sprite>($"Illust/HeroFace/{_strHeroName}_{_iHeroID}_Face");
            Enum.TryParse(strHeroData[i + 1, 2], out _sUnitStat.eRank);
            float.TryParse(strHeroData[i + 1, 3], out _sUnitStat.fOrgMoveSpeed);
            int.TryParse(strHeroData[i + 1, 4], out _sUnitStat.iMaxHp);
            int.TryParse(strHeroData[i + 1, 5], out _sUnitStat.iMaxMp);
            int.TryParse(strHeroData[i + 1, 6], out _sUnitStat.iOrgStartMp);
            int.TryParse(strHeroData[i + 1, 7], out _sUnitStat.iOrgAttack);
            int.TryParse(strHeroData[i + 1, 8], out _sUnitStat.iOrgAbilPower);
            float.TryParse(strHeroData[i + 1, 9], out _sUnitStat.fOrgAtkSpeed);
            float.TryParse(strHeroData[i + 1, 10], out _sUnitStat.fOrgAtkRange);
            int.TryParse(strHeroData[i + 1, 11], out _sUnitStat.iOrgADDef);
            int.TryParse(strHeroData[i + 1, 12], out _sUnitStat.iOrgAPDef);
            Enum.TryParse(strHeroData[i + 1, 13], out _sHeroStat.eTribe);
            Enum.TryParse(strHeroData[i + 1, 14], out _sHeroStat.eClass_1);
            Enum.TryParse(strHeroData[i + 1, 15], out _sHeroStat.eClass_2);
            int _iIsProjectile = 0;
            int.TryParse(strHeroData[i + 1, 16], out _iIsProjectile);
            _sUnitStat.bIsProjectile = Convert.ToBoolean(_iIsProjectile);
            float.TryParse(strHeroData[i + 1, 17], out _sUnitStat.fAtkDelay);
            float.TryParse(strHeroData[i + 1, 18], out _sUnitStat.fProjSpeed);
            int _iIsDirectAtk = 0;
            int.TryParse(strHeroData[i + 1, 19], out _iIsDirectAtk);
            _sUnitStat.bDirectAttack = Convert.ToBoolean(_iIsDirectAtk);
            bool.TryParse(strHeroData[i + 1, 20], out _sUnitStat.isAir);

            _listHero[i].atkEffect = null;
            _listHero[i].hitEffect = null;
            
            if (!_sUnitStat.bIsProjectile)
                _listHero[i].atkEffect = Resources.Load<HeroEffect>($"Unit/AttackEffect/{_strHeroName}_{_iHeroID}_Attack_Effect");
            else
                _listHero[i].atkEffect = Resources.Load<HeroEffect>($"Unit/AttackProjectile/{_strHeroName}_{_iHeroID}_Attack_Projectile");

            _listHero[i].hitEffect = Resources.Load<HeroEffect>($"Unit/HitEffect/{_strHeroName}_{_iHeroID}_Hit_Effect");

            _listHero[i].unitStat = _sUnitStat;
            _listHero[i].heroStat = _sHeroStat;

            HeroSkillComponent _sHeroSkill_Defense = new HeroSkillComponent();
            HeroSkillComponent _sHeroSkill_Attack = new HeroSkillComponent();

            int.TryParse(strDefSkillData[i + 1, 0], out _sHeroSkill_Defense.iSkillID);
            _sHeroSkill_Defense.strHeroName = strDefSkillData[i + 1, 1];
            _sHeroSkill_Defense.strSkillName = strDefSkillData[i + 1, 2];
            Enum.TryParse(strDefSkillData[i + 1, 3], out _sHeroSkill_Defense.eTiming);
            _sHeroSkill_Defense.imgSkillIcon = Resources.Load<Sprite>($"Illust/Skill_Icon/SkillIcon_Defense/{_sHeroSkill_Defense.strHeroName}_{_sHeroSkill_Defense.iSkillID}_Skill_Icon_Defense");

            int.TryParse(strAtkSkillData[i + 1, 0], out _sHeroSkill_Attack.iSkillID);
            _sHeroSkill_Attack.strHeroName = strAtkSkillData[i + 1, 1];
            _sHeroSkill_Attack.strSkillName = strAtkSkillData[i + 1, 2];
            Enum.TryParse(strAtkSkillData[i + 1, 3], out _sHeroSkill_Attack.eTiming);
            _sHeroSkill_Attack.imgSkillIcon = Resources.Load<Sprite>($"Illust/Skill_Icon/SkillIcon_Attack/{_sHeroSkill_Attack.strHeroName}_{_sHeroSkill_Attack.iSkillID}_Skill_Icon_Attack");

            List<SkillValue> _fSkillValue_Def = new List<SkillValue>();
            List<SkillValue> _fSkillValue_Atk = new List<SkillValue>();

            for (int j = 0; j < 8; j++)
            {
                _fSkillValue_Def.Add(new SkillValue());
                _fSkillValue_Atk.Add(new SkillValue());
                float[] _fValueD = new float[strDefSkillData[i + 1, j + 4].Split('_').Length];
                _fSkillValue_Def[j].value = _fValueD;
                float[] _fValueA = new float[strAtkSkillData[i + 1, j + 4].Split('_').Length];
                _fSkillValue_Atk[j].value = _fValueA;
                for (int k = 0; k < strDefSkillData[i + 1, j + 4].Split('_').Length; k++)
                    float.TryParse(strDefSkillData[i + 1, j + 4].Split('_')[k], out _fSkillValue_Def[j].value[k]);
                for (int k = 0; k < strAtkSkillData[i + 1, j + 4].Split('_').Length; k++)
                    float.TryParse(strAtkSkillData[i + 1, j + 4].Split('_')[k], out _fSkillValue_Atk[j].value[k]);
            }

            _sHeroSkill_Defense.listValue = _fSkillValue_Def;
            _sHeroSkill_Defense.strExplanation = strDefSkillData[i + 1, 12];
            _sHeroSkill_Defense.listSkillEffect = new List<Transform>();
            _sHeroSkill_Defense.listFinalValue = new List<float>();
            _sHeroSkill_Defense.listFinalValue = new List<float>();

            _sHeroSkill_Attack.listValue = _fSkillValue_Atk;
            _sHeroSkill_Attack.strExplanation = strAtkSkillData[i + 1, 12];
            _sHeroSkill_Attack.listSkillEffect = new List<Transform>();
            _sHeroSkill_Attack.listFinalValue = new List<float>();
            _sHeroSkill_Attack.listFinalValue = new List<float>();

            for (int j = 0; j < 3; j++)
            {
                int _iIndex = j;

                if (Resources.Load<Transform>($"Unit/Skill_Effect_Defense/{_strHeroName}_{_sHeroSkill_Defense.iSkillID}_Skill_Effect_{_iIndex}") != null)
                    _sHeroSkill_Defense.listSkillEffect.Add(Resources.Load<Transform>($"Unit/Skill_Effect_Defense/{_strHeroName}_{_sHeroSkill_Defense.iSkillID}_Skill_Effect_{_iIndex}"));

                if (Resources.Load<Transform>($"Unit/Skill_Effect_Attack/{_strHeroName}_{_sHeroSkill_Attack.iSkillID}_Skill_Effect_{_iIndex}") != null)
                    _sHeroSkill_Attack.listSkillEffect.Add(Resources.Load<Transform>($"Unit/Skill_Effect_Attack/{_strHeroName}_{_sHeroSkill_Attack.iSkillID}_Skill_Effect_{_iIndex}"));

                _sHeroSkill_Defense.listFinalValue.Add(0);
                _sHeroSkill_Attack.listFinalValue.Add(0);
            }

            _listHero[i].skill.skillComponent_Def = _sHeroSkill_Defense;
            _listHero[i].skill.skillComponent_Atk = _sHeroSkill_Attack;
        }

        _listHero = null;

        //_cBundle.Unload(false);
    }

    private void UploadSynergyData()
    {
        SynergySystem _cSynergy = FindObjectOfType<SynergySystem>();

        List<SynergyStatus> _listSynergy = _cSynergy.synergyStatusList = new List<SynergyStatus>();

        for (int i = 0; i < strSynergyData_D.GetLength(0) - 1; i++)
        {
            SynergyStatus _sSynergy = new SynergyStatus();
            _sSynergy.listValue = new List<SynergyValue>();
            _sSynergy.listCount = new List<int>();

            _sSynergy.iNumber = i;
            _sSynergy.strSynergy = strSynergyData_D[i + 1, 0];
            _sSynergy.strSynergyName = strSynergyData_D[i + 1, 1];
            _sSynergy.strSynergyDescription = strSynergyData_D[i + 1, 7];

            string[] _strSplit = strSynergyData_D[i + 1, 2].Split('_');

            for (int j = 0; j < _strSplit.Length; j++)
                _sSynergy.listCount.Add(int.Parse(_strSplit[j]));

            for (int j = 0; j < 4; j++)
            {
                _sSynergy.listValue.Add(new SynergyValue());
                float[] _fValue = new float[strSynergyData_D[i + 1, j + 3].Split('_').Length];
                _sSynergy.listValue[j].value = _fValue;
                for (int k = 0; k < strSynergyData_D[i + 1, j + 3].Split('_').Length; k++)
                    float.TryParse(strSynergyData_D[i + 1, j + 3].Split('_')[k], out _sSynergy.listValue[j].value[k]);
            }

            if (_sSynergy.listValue.Count == 1)
            {
                foreach (var item in _sSynergy.listValue)
                {
                    item.value = _sSynergy.listValue[0].value;
                }
            }

            _cSynergy.synergyStatusList.Add(_sSynergy);
        }
    }

    private void UploadCardData()
    {
        cCardManager = FindObjectOfType<CardManager>(true);
        List<Card> _listCards = cCardManager.cardList = new List<Card>();

        for (int i = 0; i < strCardData.GetLength(0) - 1; i++)
        {
            Card _cCard = new Card();
            _listCards.Add(_cCard);

            int _iCount = i;

            CardStatus _sCardStat = new CardStatus();

            _sCardStat.iCardID = int.Parse(strCardData[i + 1, 0]);
            _sCardStat.strCardName = strCardData[i + 1, 1];
            Enum.TryParse(strCardData[i + 1, 2], out _sCardStat.eCardType);
            Enum.TryParse(strCardData[i + 1, 3], out _sCardStat.eCardClass);
            Enum.TryParse(strCardData[i + 1, 4], out _sCardStat.eCardTear);
            Enum.TryParse(strCardData[i + 1, 5], out _sCardStat.eCostType);
            _sCardStat.iLevel = int.Parse(strCardData[i + 1, 6]);
            _sCardStat.iCost = int.Parse(strCardData[i + 1, 7]);
            Enum.TryParse(strCardData[i + 1, 8], out _sCardStat.eCastTiming);
            Enum.TryParse(strCardData[i + 1, 9], out _sCardStat.eCastTarget);
            _sCardStat.imgCardIcon = Resources.Load<Sprite>("Illust/CardIllust/" + _sCardStat.iCardID);
            if (_sCardStat.eCardType == CardType.Equip)
                _sCardStat.imgEqupIcon = Resources.Load<Sprite>("Illust/Equipment_Icon/" + _sCardStat.iCardID);
            Enum.TryParse(strCardData[i + 1, 10], out _sCardStat.eScopeType);
            _sCardStat.strCardDescription = strCardData[i + 1, 18];

            _sCardStat.listValue = new List<float>();

            for (int j = 0; j < 8; j++)
            {
                _sCardStat.listValue.Add(0);
                float _iValue;
                float.TryParse(strCardData[i + 1, j + 11], out _iValue);
                _sCardStat.listValue[j] = _iValue;
            }

            _sCardStat.goEffect = Resources.Load<GameObject>("Card/Effect/" + _sCardStat.iCardID);

            _cCard.stat = _sCardStat;

            if (_sCardStat.eCardType == CardType.Equip)
            {
                string[] _strFinalValue = new string[4];
                _strFinalValue[0] = _sCardStat.strCardDescription.Contains('[') ? Tools.GetMiddleString(_sCardStat.strCardDescription, "[", "]") : string.Empty;
                _strFinalValue[1] = _sCardStat.strCardDescription.Contains('|') ? Tools.GetMiddleString(_sCardStat.strCardDescription, "|", "|") : string.Empty;
                _strFinalValue[2] = _sCardStat.strCardDescription.Contains('&') ? Tools.GetMiddleString(_sCardStat.strCardDescription, "&", "&") : string.Empty;
                _strFinalValue[3] = _sCardStat.strCardDescription.Contains('#') ? Tools.GetMiddleString(_sCardStat.strCardDescription, "#", "#") : string.Empty;

                for (int j = 0; j < _strFinalValue.Length; j++)
                {
                    _strFinalValue[j] = _strFinalValue[j].Replace("<", string.Empty);
                    _strFinalValue[j] = _strFinalValue[j].Replace(">", string.Empty);
                    _strFinalValue[j] = _strFinalValue[j].Replace("+", string.Empty);
                    _strFinalValue[j] = _strFinalValue[j].Replace("-", string.Empty);
                    _strFinalValue[j] = _strFinalValue[j].Replace("*", string.Empty);
                    _strFinalValue[j] = _strFinalValue[j].Replace("/", string.Empty);

                    for (int k = 0; k < _sCardStat.listValue.Count; k++)
                        _strFinalValue[j] = _strFinalValue[j].Replace($"Value0{k}", _sCardStat.listValue[k].ToString());

                    if (_strFinalValue[j].Contains("@Attack"))
                    {
                        _strFinalValue[j] = _strFinalValue[j].Replace("@Attack", string.Empty);
                        int.TryParse(_strFinalValue[j], out _sCardStat.iAddAD);
                    }
                    if (_strFinalValue[j].Contains("@Power"))
                    {
                        _strFinalValue[j] = _strFinalValue[j].Replace("@Power", string.Empty);
                        int.TryParse(_strFinalValue[j], out _sCardStat.iAddAP);
                    }
                    if (_strFinalValue[j].Contains("@Hp"))
                    {
                        _strFinalValue[j] = _strFinalValue[j].Replace("@Hp", string.Empty);
                        int.TryParse(_strFinalValue[j], out _sCardStat.iAddHP);
                    }
                    if (_strFinalValue[j].Contains("@ADDef"))
                    {
                        _strFinalValue[j] = _strFinalValue[j].Replace("@ADDef", string.Empty);
                        int.TryParse(_strFinalValue[j], out _sCardStat.iAddDef);
                    }
                    if (_strFinalValue[j].Contains("@+APDef"))
                    {
                        _strFinalValue[j] = _strFinalValue[j].Replace("@+APDef", string.Empty);
                        int.TryParse(_strFinalValue[j], out _sCardStat.iAddAPDef);
                    }
                    if (_strFinalValue[j].Contains("@AtkSpd"))
                    {
                        _strFinalValue[j] = _strFinalValue[j].Replace("@AtkSpd", string.Empty);
                        float.TryParse(_strFinalValue[j], out _sCardStat.fAddAtkSpd);
                    }
                }

                _cCard.stat = _sCardStat;
            }
        }

        _listCards = null;
    }

    private void UploadMarketPercentData()
    {
        cGameManager.marketPercentage = new MarketPercentageArray[(int)UnitRank.Legendary];

        for (int i = 0; i < (int)UnitRank.Legendary; i++)
        {
            cGameManager.marketPercentage[i].list = new List<float>();

            for (int j = 0; j < 19; j++)
                cGameManager.marketPercentage[i].list.Add(float.Parse(strMarketPercentData[j , i]));
        }
    }

    private void UploadChapterData()
    {
        cGameManager.chapterList = new List<ChapterStatus>();

        for (int i = 0; i < strChapterData.GetLength(0) - 1; i++)
        {
            ChapterStatus _newChapter = new ChapterStatus();
            int.TryParse(strChapterData[i + 1, 0], out _newChapter.iStageID);
            _newChapter.imgStageEmblem = Resources.Load<Sprite>($"Illust/ChapterIcon/{_newChapter.iStageID}");
            _newChapter.strChapterName = strChapterData[i + 1, 1];
            int.TryParse(strCardData[i + 1, 2], out _newChapter.iPlayerMaxHp);
            _newChapter.strRewards = new string[4];

            for (int j = 0; j < _newChapter.strRewards.Length; j++)
            {
                if (strChapterData[i + 1, j + 3] != string.Empty)
                    _newChapter.strRewards[j] = strChapterData[i + 1, j + 3];
            }

            int.TryParse(strChapterData[i + 1, 7], out _newChapter.iMaxStage);
            _newChapter.listStageUnits = new List<StageUnits>();

            cGameManager.chapterList.Add(_newChapter);
        }
    }

    private void UploadStageData()
    {
        for (int i = 1; i < strStageUnitsData.GetLength(1) + 1; i++)
        {
            for (int j = 0; j < strStageUnitsData.GetLength(0) - 1; j++)
            {
                StageUnits _sNewUnits = new StageUnits();
                _sNewUnits.listLevel = new List<int>();
                _sNewUnits.listId = new List<int>();

                for (int k = 0; k < strStageUnitsData[j + 1, i - 1].Split("/").Length; k++)
                {
                    try
                    {
                        int _iTemp = 0;
                        int.TryParse(strStageUnitsData[j + 1, i - 1].Split("/")[k].Split("_")[0], out _iTemp);
                        _sNewUnits.listLevel.Add(_iTemp);
                        int.TryParse(strStageUnitsData[j + 1, i - 1].Split("/")[k].Split("_")[1], out _iTemp);
                        _sNewUnits.listId.Add(_iTemp);
                    }
                    catch { }
                }

                cGameManager.chapterList[i - 1].listStageUnits.Add(_sNewUnits);
            }
        }
    }
}
