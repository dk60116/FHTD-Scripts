using System.Collections.Generic;
using UnityEngine;

public class UnitDamageTextArea : MonoBehaviour
{
    [SerializeField, ReadOnlyInspector]
    private List<DamageText> listDamageText;

    [ContextMenu("Init")]
    private void Init()
    {
        listDamageText = new List<DamageText>();

        for (int i = 0; i < transform.childCount; i++)
            listDamageText.Add(transform.GetChild(i).GetComponent<DamageText>());
    }

    public void OpenDamage(int _iDamge, DamageType _eDmgType, bool _bIsCritical, bool _bIsEvation, int _iCode)
    {
        for (int i = 0; i < listDamageText.Count; i++)
        {
            if (_iCode != 0 && listDamageText[i].code == _iCode)
            {
                listDamageText[i].gameObject.SetActive(true);
                listDamageText[i].ExtendAble(_iDamge);
                return;
            }
        }

        for (int i = 0; i < listDamageText.Count; i++)
        {
            if (listDamageText[i].gameObject.activeSelf)
                continue;

            listDamageText[i].SetDamage(_iDamge, _eDmgType, _bIsCritical, _bIsEvation, _iCode);
            listDamageText[i].gameObject.SetActive(true);

            DamageText _temp = listDamageText[i];

            listDamageText.Remove(listDamageText[i]);
            listDamageText.Insert(2, _temp);

            return;
        }

        listDamageText[0].SetDamage(_iDamge, _eDmgType, _bIsCritical, _bIsEvation, _iCode);
        listDamageText[0].gameObject.SetActive(true);
    }


    public void ResetTextList()
    {
        foreach (var item in listDamageText)
            item.gameObject.SetActive(false);
    }
}
