using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    #region Define
    private readonly int checkCount = 5;
    #endregion

    [SerializeField]
    private Transform _parent;

    [SerializeField]
    private GameObject _pooledObject;

    private List<GameObject> _pooledObject_list;
    private Stack<GameObject> _usebleObject_stack;

    private void Start()
    {
        RequestCreateObjects();
    }

    /// <summary>
    /// オブジェクトプールの作成をレクエストする
    /// </summary>
    public void RequestCreateObjects(int capacity = 10)
    {
        StartCoroutine(CreateObjectsAsync(capacity));
    }

    /// <summary>
    /// オブジェクトプールの作成
    /// </summary>
    private IEnumerator CreateObjectsAsync(int capacity)
    {
        _pooledObject_list = new List<GameObject>(capacity);

        for (int i = 0; i < _pooledObject_list.Count; i++)
        {
            var go = Instantiate(_pooledObject, _parent);

            go.SetActive(false);
            // リストに設定する
            _pooledObject_list[i] = go;
            PushToObjectStack(go);
            yield return null;
        }
        yield return null;
    }

    /// <summary>
    /// オブジェクトの取得をする
    /// </summary>
    public GameObject Get(Vector3 pos)
    {
        CheckUseableObjectsCount(checkCount);

        // スタックから取得
        if (_usebleObject_stack.TryPop(out GameObject go))
        {
            SetObjectDate(go, true, pos);
            return go;
        }
        // スタックに無かった場合は生成する
        else
        {
            go = CreatePooledObject(_pooledObject);
            go.transform.position = pos;
            _pooledObject_list.Add(go);
            return go;
        }
    }

    /// <summary>
    /// プールオブジェクトのクリア
    /// </summary>
    public void ClearPool()
    {
        _usebleObject_stack.Clear();
        _pooledObject_list.Clear();
    }

    /// <summary>
    /// 使用可能なオブジェクトの数を確認する
    /// </summary>
    private void CheckUseableObjectsCount(int checkCount)
    {
        // スタック量が一定以下になった時
        if (GetStackCount() < checkCount)
        {
            StartCoroutine(AddUseableObjectToStackAsync());
        }
    }

    /// <summary>
    /// <para>GameObjectのプロパティを設定する</para>
    /// </summary>
    private void SetObjectDate(GameObject go, bool setActive, Vector3 pos)
    {
        go.SetActive(setActive);
        go.transform.position = pos;
    }

    /// <summary>
    /// スタック数の取得
    /// </summary>
    private int GetStackCount()
    {
        return _usebleObject_stack.Count;
    }

    /// <summary>
    /// オブジェクトの作成
    /// </summary>
    private GameObject CreatePooledObject(GameObject pooledObject)
    {
        var go = Instantiate(pooledObject);
        AddToObjectList(go);
        return go;
    }

    /// <summary>
    /// <para>リストにオブジェクトの追加をする</para>
    /// <see cref="_pooledObject_list"/>
    /// </summary>
    private void AddToObjectList(GameObject go)
    {
        _pooledObject_list.Add(go);
    }

    private void PushToObjectStack(GameObject go)
    {
        _usebleObject_stack.Push(go);
    }

    /// <summary>
    /// <para>使用可能なオブジェクトをスタックに追加する</para> 
    /// <see cref="_usebleObject_stack"/>
    /// </summary>
    private IEnumerator AddUseableObjectToStackAsync()
    {
        foreach (var item in _pooledObject_list)
        {
            if (item.activeSelf is false)
            {
                _usebleObject_stack.Push(item);
            }
            yield return null;
        }
        yield return null;
    }
}
