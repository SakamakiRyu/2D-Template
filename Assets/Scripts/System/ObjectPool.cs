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
    /// �I�u�W�F�N�g�v�[���̍쐬�����N�G�X�g����
    /// </summary>
    public void RequestCreateObjects(int capacity = 10)
    {
        StartCoroutine(CreateObjectsAsync(capacity));
    }

    /// <summary>
    /// �I�u�W�F�N�g�v�[���̍쐬
    /// </summary>
    private IEnumerator CreateObjectsAsync(int capacity)
    {
        _pooledObject_list = new List<GameObject>(capacity);

        for (int i = 0; i < _pooledObject_list.Count; i++)
        {
            var go = Instantiate(_pooledObject, _parent);

            go.SetActive(false);
            // ���X�g�ɐݒ肷��
            _pooledObject_list[i] = go;
            PushToObjectStack(go);
            yield return null;
        }
        yield return null;
    }

    /// <summary>
    /// �I�u�W�F�N�g�̎擾������
    /// </summary>
    public GameObject Get(Vector3 pos)
    {
        CheckUseableObjectsCount(checkCount);

        // �X�^�b�N����擾
        if (_usebleObject_stack.TryPop(out GameObject go))
        {
            SetObjectDate(go, true, pos);
            return go;
        }
        // �X�^�b�N�ɖ��������ꍇ�͐�������
        else
        {
            go = CreatePooledObject(_pooledObject);
            go.transform.position = pos;
            _pooledObject_list.Add(go);
            return go;
        }
    }

    /// <summary>
    /// �v�[���I�u�W�F�N�g�̃N���A
    /// </summary>
    public void ClearPool()
    {
        _usebleObject_stack.Clear();
        _pooledObject_list.Clear();
    }

    /// <summary>
    /// �g�p�\�ȃI�u�W�F�N�g�̐����m�F����
    /// </summary>
    private void CheckUseableObjectsCount(int checkCount)
    {
        // �X�^�b�N�ʂ����ȉ��ɂȂ�����
        if (GetStackCount() < checkCount)
        {
            StartCoroutine(AddUseableObjectToStackAsync());
        }
    }

    /// <summary>
    /// <para>GameObject�̃v���p�e�B��ݒ肷��</para>
    /// </summary>
    private void SetObjectDate(GameObject go, bool setActive, Vector3 pos)
    {
        go.SetActive(setActive);
        go.transform.position = pos;
    }

    /// <summary>
    /// �X�^�b�N���̎擾
    /// </summary>
    private int GetStackCount()
    {
        return _usebleObject_stack.Count;
    }

    /// <summary>
    /// �I�u�W�F�N�g�̍쐬
    /// </summary>
    private GameObject CreatePooledObject(GameObject pooledObject)
    {
        var go = Instantiate(pooledObject);
        AddToObjectList(go);
        return go;
    }

    /// <summary>
    /// <para>���X�g�ɃI�u�W�F�N�g�̒ǉ�������</para>
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
    /// <para>�g�p�\�ȃI�u�W�F�N�g���X�^�b�N�ɒǉ�����</para> 
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
