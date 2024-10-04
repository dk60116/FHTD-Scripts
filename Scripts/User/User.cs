using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UserDataComponent { Nickname, Email, Avatar, Level, FullExp, CrtExp}

public class User : SingletonMono<User>
{
    [SerializeField]
    private UserDatabase sUser;
    [SerializeField, ReadOnlyInspector]
    private List<bool> listCardOwn;

    public UserDatabase GetUser()
    {
        return sUser;
    }

    public void SetUser(UserDatabase _sUserData)
    {
        sUser = _sUserData;
    }

    public void SetOwnCard(List<bool> _listOwnCard)
    {
        listCardOwn = _listOwnCard;
    }

    public void SetOwnWithCardID(int _iCardID, bool _bValue)
    {
        listCardOwn[_iCardID] = _bValue;
    }

    public void SetUserAvatar(int _iIndex)
    {
        sUser.selectedAvatar = _iIndex;
    }
}
