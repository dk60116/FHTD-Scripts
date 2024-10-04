using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynergyActivate : MonoBehaviour
{
    private int iBeastAttack;

    public void CheckPlayerSynergy(HeroTribe _eTribe, int _iCount)
    {
        switch (_eTribe)
        {
            case HeroTribe.None:
                break;
            case HeroTribe.Human:
                break;
            case HeroTribe.Beast:
                foreach (var item in InGameManager.instance.cPlayerController.ownHeros)
                {
                    HeroStatus _status = item.heroStat;

                    if (item.heroStat.eTribe == HeroTribe.Beast)
                    {
                        if (_iCount < 2)
                            _status.iBeastAttack = 0;
                        if (_iCount >= 2)
                            _status.iBeastAttack = (int)GameManager.instance.cSynergySystem.synergyStatusList[1].listValue[0].value[0];
                        if (_iCount >= 4)
                            _status.iBeastAttack = (int)GameManager.instance.cSynergySystem.synergyStatusList[1].listValue[0].value[1];
                        if (_iCount >= 6)
                            _status.iBeastAttack = (int)GameManager.instance.cSynergySystem.synergyStatusList[1].listValue[0].value[2];
                        if (_iCount >= 8)
                            _status.iBeastAttack = (int)GameManager.instance.cSynergySystem.synergyStatusList[1].listValue[0].value[3];
                    }
                    else
                    {
                        if (_iCount < 2)
                            _status.iBeastAttack = 0;
                        if (_iCount >= 2)
                            _status.iBeastAttack = (int)GameManager.instance.cSynergySystem.synergyStatusList[1].listValue[1].value[0];
                        if (_iCount >= 4)
                            _status.iBeastAttack = (int)GameManager.instance.cSynergySystem.synergyStatusList[1].listValue[1].value[1];
                        if (_iCount >= 6)
                            _status.iBeastAttack = (int)GameManager.instance.cSynergySystem.synergyStatusList[1].listValue[1].value[2];
                        if (_iCount >= 8)
                            _status.iBeastAttack = (int)GameManager.instance.cSynergySystem.synergyStatusList[1].listValue[1].value[3];
                    }


                    if (!item.isPlaced)
                        _status.iBeastAttack = 0;

                    item.heroStat = _status;
                    item.SetupStat();
                }
                break;
            case HeroTribe.Dragon:
                break;
            case HeroTribe.Dragonoid:
                break;
            case HeroTribe.Elemental:
                break;
            case HeroTribe.Fairy:
                break;
            case HeroTribe.Undead:
                break;
            case HeroTribe.Plant:
                break;
            case HeroTribe.Cavalry:
                break;
            case HeroTribe.Guardian:
                break;
            case HeroTribe.Berserker:
                break;
            default:
                break;
        }
    }

    public void CheckPlayerSynergy(HeroClass _eClass, int _iCount)
    {
        switch (_eClass)
        {
            case HeroClass.None:
                break;
            case HeroClass.Magician:
                foreach (var item in InGameManager.instance.cPlayerController.ownHeros)
                {
                    if (item.heroStat.eClass_1 == HeroClass.Magician || item.heroStat.eClass_2 == HeroClass.Magician)
                    {
                        HeroStatus _status = item.heroStat;

                        if (_iCount < 2)
                            _status.iMagicianPower = 0;
                        if (_iCount >= 2)
                            _status.iMagicianPower = (int)GameManager.instance.cSynergySystem.synergyStatusList[11].listValue[0].value[0];
                        if (_iCount >= 4)
                            _status.iMagicianPower = (int)GameManager.instance.cSynergySystem.synergyStatusList[11].listValue[0].value[1];
                        if (_iCount >= 6)
                            _status.iMagicianPower = (int)GameManager.instance.cSynergySystem.synergyStatusList[11].listValue[0].value[2];
                        if (_iCount >= 8)
                            _status.iMagicianPower = (int)GameManager.instance.cSynergySystem.synergyStatusList[11].listValue[0].value[3];

                        if (!item.isPlaced)
                            _status.iMagicianPower = 0;

                        item.heroStat = _status;
                        item.SetupStat();
                    }
                }
                break;
            case HeroClass.Flying:
                break;
            case HeroClass.Armoric:
                break;
            case HeroClass.TwinSwords:
                break;
            case HeroClass.Insect:
                break;
            case HeroClass.Fighter:
                break;
            case HeroClass.Natual:
                break;
            case HeroClass.Swordsman:
                break;
            case HeroClass.BigHorns:
                break;
            case HeroClass.Ranger:
                break;
            case HeroClass.Priest:
                break;
            default:
                break;
        }
    }
}
