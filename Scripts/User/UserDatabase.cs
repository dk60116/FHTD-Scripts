using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct UserDatabase
{
    public string deviceId;
    [SerializeField, ReadOnlyInspector]
    public string nickname;
    [SerializeField, ReadOnlyInspector]
    public string emailAdress;
    [SerializeField, ReadOnlyInspector]
    public int selectedAvatar;
    [SerializeField, ReadOnlyInspector]
    public int level;
    [SerializeField, ReadOnlyInspector]
    public int needExp, crtExp;
}
