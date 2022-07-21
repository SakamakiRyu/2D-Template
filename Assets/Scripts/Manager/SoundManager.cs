using UnityEngine;

public class SoundManager : MonoBehaviour, ISoundManager
{
    #region Field
    private AudioSource _bgmSource { get; set; }
    private AudioSource _seSource { get; set; }
    #endregion

    #region Unity Function
    private void Awake()
    {
        Initialize();
    }

    private void OnEnable()
    {
        if (ServiceLocator<ISoundManager>.IsValid is false)
        {
            ServiceLocator<ISoundManager>.Regist(this);
        }
    }

    private void OnDisable()
    {
        if (ServiceLocator<ISoundManager>.IsValid)
        {
            ServiceLocator<ISoundManager>.UnRegist(this);
        }
    }
    #endregion

    #region Public Function
    public void PlayBGM(AudioClip clip)
    {
    }

    public void PlaySE(AudioClip clip)
    {
    }
    #endregion

    #region Private Function
    private void Initialize()
    {
        // 既にAudioSourceを持っていたら削除
        var go = GetComponents<AudioSource>();
        foreach (var item in go)
        {
            Destroy(item);
        }

        _bgmSource = null;
        _seSource = null;

        // コンポーネントの追加
        _bgmSource = this.gameObject.AddComponent<AudioSource>();
        _seSource = this.gameObject.AddComponent<AudioSource>();
    }
    #endregion
}