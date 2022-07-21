using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> : MonoBehaviour where T : MonoBehaviour
{
    #region コンストラクタ
    private ObjectPool() { }

    public ObjectPool(T pooledObject, Action onCreatedPool = null, Action onCreatedObject = null, int defaultCount = 10)
    {
        _pooledObject = pooledObject;
        OnCreatedPool += onCreatedPool;
        OnCreatedObject += onCreatedObject;
        _objectCount = defaultCount;
    }
    #endregion

    #region Field
    // プールが作成済みか
    public bool IsCreatedPool { get; private set; } = false;
    // 作成するオブジェクト
    private T _pooledObject { get; set; } = null;
    // プールの容量
    private int _objectCount { get; set; } = 10;

    // プールオブジェクトのリスト
    private List<T> _pooledObjectList;

    // イベント
    private Action OnCreatedPool;
    private Action OnCreatedObject;
    #endregion

    #region Public Function
    /// <summary>
    /// オブジェクトの取得
    /// </summary>
    public T Get(Vector3 pos, Transform parent = null)
    {
        foreach (var item in _pooledObjectList)
        {
            if (item.gameObject.activeInHierarchy is false)
            {
                item.transform.position = pos;
                item.transform.parent = parent;
                return item;
            }
        }
        var go = CreatePooledObject();
        go.transform.parent = parent;
        go.gameObject.SetActive(true);
        return go;
    }

    /// <summary>
    /// <para>プールの作成</para>
    /// </summary>
    public void CreatePool()
    {
        if (IsCreatedPool is false)
            return;

        _pooledObjectList = new List<T>(_objectCount);

        for (int i = 0; i < _objectCount; i++)
        {
            // オブジェクトの生成
            CreatePooledObject();
        }

        IsCreatedPool = true;
        OnCreatedPool?.Invoke();
    }

    /// <summary>
    /// プールのクリア
    /// </summary>
    public void ClearPool()
    {
        _pooledObjectList.Clear();
        OnCreatedObject = null;
        OnCreatedPool = null;
        IsCreatedPool = false;
    }
    #endregion

    #region Private Function
    /// <summary>
    /// プールオブジェクトの作成
    /// </summary>
    private T CreatePooledObject()
    {
        // 生成
        var obj = Instantiate(_pooledObject);
        obj.gameObject.SetActive(false);
        OnCreatedObject?.Invoke();

        // 管理オブジェクトに追加
        _pooledObjectList.Add(obj);
        return obj;
    }
    #endregion
}
