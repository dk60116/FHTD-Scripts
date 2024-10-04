using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    [SerializeField, ReadOnlyInspector]
    private Collider cCollider;
    [SerializeField, ReadOnlyInspector]
    private List<Unit> listEnteredUnit;
    [SerializeField]
    private MeshRenderer renMesh;
    private bool bEntered;

    public delegate void HitBoxEnterChain(Unit _cEnemy, out BuffValue? _sBuff);
    public delegate void HitBoxExitChain(Unit _cEnemy, BuffValue? _sBuff);
    public HitBoxEnterChain hitBoxEnterChain;
    public HitBoxExitChain hitBoxExitChain;

    private BuffValue? sTempBuff;

    void Awake()
    {
        listEnteredUnit = new List<Unit>();
        if (renMesh != null)
        {
            renMesh.enabled = false;
            renMesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }

    [ContextMenu("Init")]
    void Reset()
    {
        cCollider = GetComponent<Collider>();
        cCollider.isTrigger = true;
        renMesh = GetComponent<MeshRenderer>();
    }

    private void OnEnable()
    {
        bEntered = false;
    }

    void OnDisable()
    {
        foreach (var item in listEnteredUnit)
        {
            try
            {
                hitBoxExitChain(item, sTempBuff);
            }
            catch { }
        }

        listEnteredUnit.Clear();
    }

    public void OnOffCollider(bool _bOn)
    {
        cCollider.enabled = _bOn;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Hero"))
        {
            if (hitBoxEnterChain == null)
                return;

            Unit _cUnit = null;

            if (other.TryGetComponent(out _cUnit))
            {
                listEnteredUnit.Add(_cUnit);
                hitBoxEnterChain.Invoke(_cUnit, out sTempBuff);
                bEntered = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Hero"))
        {
            Hero _cHero = null;

            if (other.TryGetComponent(out _cHero))
            {
                listEnteredUnit.Remove(_cHero);
                try
                {
                    hitBoxExitChain.Invoke(_cHero, sTempBuff);
                }
                catch (System.Exception) { }
            }
        }
    }

    public void AutoReturn(float _fTime)
    {
        StartCoroutine(AutoReturnCoroutine(_fTime));
    }

    IEnumerator AutoReturnCoroutine(float _fTime)
    {
        yield return new WaitForSeconds(_fTime);

        GetComponent<PoolableObject>().ReturnObj();
    }

    public bool entered { get => bEntered; }
}
