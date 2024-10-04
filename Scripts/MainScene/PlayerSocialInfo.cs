using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PlayerSocialInfo
{
    public string masterID, nickName, equipCardFrame, equipProfileIcon, equipProfileFrame;
    public List<string> haveCardFrame, haveProfileIcon, haveProfileFrame, purchasedList;
    public int level, exp;
    public int diamond, powder;
}
