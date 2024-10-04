using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public struct QObjectList
{
    public Queue<GameObject> list;
}

public class PoolingManager : MonoBehaviour
{
    [SerializeField]
    private bool bCanInstantiate;
    [SerializeField, ReadOnlyInspector]
    private List<GameObject> listTargetObj, listCopy;
    [SerializeField, ReadOnlyInspector]
    private List<Transform> listObjHolder;

    [SerializeField, ReadOnlyInspector]
    private List<int> listCount;
    [SerializeField, ReadOnlyInspector]
    private List<QObjectList> listPools;

    [ContextMenu("Init")]
    public void Reset()
    {
        listObjHolder = new List<Transform>();
        listTargetObj = new List<GameObject>();
        listPools = new List<QObjectList>();
        listCount = new List<int>();
        listCopy = new List<GameObject>();
    }

    public void AddTargetObj(GameObject _goTarget)
    {
        listTargetObj.Add(_goTarget);
    }

    public void Init()
    {
        for (int i = 0; i < listTargetObj.Count; i++)
        {
            listPools.Add(new QObjectList());

            QObjectList _sQueue = listPools[i];
            _sQueue.list = new Queue<GameObject>();

            GameObject _goParent = new GameObject($"Pool_{listTargetObj[i]}");
            _goParent.transform.SetParent(transform);
            listObjHolder.Add(_goParent.transform);

            for (int j = 0; j < listCount[i]; j++)
            {
                int _iIndex = j;
                GameObject _goNewObj = Instantiate(listTargetObj[i], listObjHolder[i]);
                PoolableObject _cPoolable = _goNewObj.AddComponent<PoolableObject>();
                _cPoolable.InitObject(this, i);
                _goNewObj.gameObject.SetActive(false);
                _goNewObj.name = $"{_goNewObj.name} ({_iIndex})";
                _goNewObj.SetActive(false);
                _sQueue.list.Enqueue(_goNewObj);
            }

            listPools[i] = _sQueue;
        }

        listCopy = GetAllObjList();
    }

    public GameObject GetObj(int _iIndex, Transform _tfParent = null)
    {
        GameObject _goNewObj;

        if (listPools[_iIndex].list.Count > 0)
            _goNewObj = listPools[_iIndex].list.Dequeue();
        else
        {
            if (bCanInstantiate)
            {
                _goNewObj = Instantiate(listTargetObj[_iIndex], listObjHolder[_iIndex]);
                PoolableObject _cPoolable = _goNewObj.AddComponent<PoolableObject>();
                _cPoolable.InitObject(this, _iIndex);
            }
            else
                return null;
        }

        _goNewObj.transform.SetParent(_tfParent);
        _goNewObj.SetActive(true);

        return _goNewObj;
    }

    public void ReturnObj(GameObject _goObj, int _iIndex)
    {
        listPools[_iIndex].list.Enqueue(_goObj);
        _goObj.SetActive(false);
        _goObj.transform.SetParent(listObjHolder[_iIndex]);
    }

    public List<GameObject> GetAllObjList()
    {
        LinkedList<GameObject> _newList = new LinkedList<GameObject>();

        foreach (var item in listPools)
        {
            if (item.list != null)
            {
                foreach (var item2 in item.list)
                    _newList.AddLast(item2);
            }
        }

        return _newList.ToList();
    }

    public void AddListCount(int _iValue)
    {
        listCount.Add(_iValue);
    }

    public List<GameObject> copyList { get => listCopy;}
    public List<Transform> listHolder { get => listObjHolder; }
}
