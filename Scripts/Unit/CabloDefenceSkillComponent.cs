using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class CabloDefenceSkillComponent : MonoBehaviour
{
    public Unit parentUnit;

    void OnEnable()
    {
        FindObjectOfType<HeroSkill>(true).cabloBlockList.Add(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Unit>() != null)
        {
            Unit _unit = other.GetComponent<Unit>();

            if (!_unit.unitStat.isAir)
                _unit.OnOffNavAgent(false);

            if (_unit == parentUnit)
                return;

            if (!FindObjectOfType<HeroSkill>(true).cabloBlockUnitList.Contains(_unit))
            {
                CabloDefenceSkillComponent _eft = Instantiate(gameObject, _unit.body.position, Quaternion.identity).GetComponent<CabloDefenceSkillComponent>();
                FindObjectOfType<HeroSkill>(true).cabloBlockUnitList.Add(_unit);
                _eft.parentUnit = _unit;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Unit>() != null)
        {
            Unit _unit = other.GetComponent<Unit>();

            _unit.OnOffNavAgent(true);
        }
    }
}
