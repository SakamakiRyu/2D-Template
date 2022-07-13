using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    #region Define
    private const int DEFAULT_CAPACITY = 10;
    #endregion

    [SerializeField]
    private Transform _parent;

    [SerializeField]
    private GameObject _pooledObject;

    private List<GameObject> _pooledObject_list;
    private Stack<GameObject> _usebleObject_stack;

    private int _useableObjectCount;

    private void Start()
    {
        CreateObjects(DEFAULT_CAPACITY);
    }

    /// <summary>
    /// オブジェクトの作成
    /// </summary>
    /// <param name="capacity">プールの上限</param>
    public void CreateObjects(int capacity = DEFAULT_CAPACITY)
    {
        _pooledObject_list = new List<GameObject>(capacity);

        for (int i = 0; i < DEFAULT_CAPACITY; i++)
        {
            var go = Instantiate(_pooledObject);
            go.SetActive(false);

            _usebleObject_stack.Push(go);
            _useableObjectCount++;

            go.transform.SetParent(_parent, false);
            _pooledObject_list.Add(go);
        }
    }

    /// <summary>
    /// オブジェクトの取得
    /// </summary>
    /// <param name="pos">初期位置</param>
    /// <returns></returns>
    public GameObject Get(Vector3 pos)
    {
        GameObject go;

        if (_usebleObject_stack.TryPop(out go))
        {
            _useableObjectCount--;
        }

        if (go is not null)
        {
            go.SetActive(true);
            go.transform.position = pos;
            return go;
        }
        else
        {
            var ngo = Instantiate(_pooledObject);
            ngo.transform.position = pos;
            _pooledObject_list.Add(ngo);
            return go;
        }
    }

    private void PoshToStack(GameObject go)
    {
        _useableObjectCount++;
        _usebleObject_stack.Push(go);
    }

    private GameObject CreateObject()
    {
        return Instantiate(_pooledObject);
    }

    private IEnumerator CheckGetableObjectAsync()
    {
        while (true)
        {
            foreach (var item in _pooledObject_list)
            {
                if (item.activeSelf is false)
                {
                    _usebleObject_stack.Push(item);
                    yield return null;
                }
            }
            yield return null;
        }
        yield return null;
    }
}
