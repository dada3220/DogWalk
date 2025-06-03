using UnityEngine;

/// <summary>
/// ゲーム全体のサウンド（BGM/SE）のON/OFFと再生を管理するシングルトンクラス
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource bgmSource;  // BGM 用 AudioSource
    [SerializeField] private AudioSource seSource;   // SE 用 AudioSource（未使用だが残しておく）

    public bool IsBgmOn { get; private set; } = true;
    public bool IsSeOn { get; private set; } = true;

    public delegate void OnBgmMuteChangedHandler(bool mute);
    public event OnBgmMuteChangedHandler OnBgmMuteChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadSettings();  // 保存データの読み込み
        ApplySettings(); // 状態を反映
    }

    /// <summary>
    /// BGM の ON/OFF を切り替える
    /// </summary>
    public void SetBgm(bool on)
    {
        IsBgmOn = on;
        PlayerPrefs.SetInt("BGM_ON", on ? 1 : 0);
        ApplySettings();
    }

    /// <summary>
    /// SE の ON/OFF を切り替える（SEManager にも反映）
    /// </summary>
    public void SetSe(bool on)
    {
        IsSeOn = on;
        PlayerPrefs.SetInt("SE_ON", on ? 1 : 0);

        // SEManager にミュート状態を通知
        if (SEManager.Instance != null)
        {
            SEManager.Instance.SetMuteFromManager(!on);
        }
    }

    /// <summary>
    /// SE を直接再生したいとき用（SEManager推奨）
    /// </summary>
    public void PlaySe(AudioClip clip)
    {
        if (IsSeOn && seSource != null && clip != null)
        {
            seSource.PlayOneShot(clip);
        }
    }

    /// <summary>
    /// 現在の設定を AudioSource と SEManager に反映する
    /// </summary>
    private void ApplySettings()
    {
        if (bgmSource != null)
        {
            bgmSource.mute = !IsBgmOn;
        }

        OnBgmMuteChanged?.Invoke(!IsBgmOn);

        // SEManager にミュート状態を渡す
        if (SEManager.Instance != null)
        {
            SEManager.Instance.SetMuteFromManager(!IsSeOn);
        }
    }

    /// <summary>
    /// 保存されている設定を PlayerPrefs から読み込む
    /// </summary>
    private void LoadSettings()
    {
        IsBgmOn = PlayerPrefs.GetInt("BGM_ON", 1) == 1;
        IsSeOn = PlayerPrefs.GetInt("SE_ON", 1) == 1;
    }
}
