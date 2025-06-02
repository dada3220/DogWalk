using UnityEngine;

// サウンド全体を管理するシングルトンクラス
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource bgmSource;  // BGM用AudioSource（Inspectorでセット）
    [SerializeField] private AudioSource seSource;   // SE用AudioSource（Inspectorでセット）

    // BGMとSEのON/OFF状態
    public bool IsBgmOn { get; private set; } = true;
    public bool IsSeOn { get; private set; } = true;

    // BGMのON/OFF切り替え時に通知するイベント
    public delegate void OnBgmMuteChangedHandler(bool mute);
    public event OnBgmMuteChangedHandler OnBgmMuteChanged;

    private void Awake()
    {
        // シングルトンの初期化
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadSettings();  // PlayerPrefsから設定を読み込み
        ApplySettings(); // 読み込んだ設定を反映
    }

    // BGMのON/OFFを設定するメソッド
    public void SetBgm(bool on)
    {
        IsBgmOn = on;
        ApplySettings();
        PlayerPrefs.SetInt("BGM_ON", on ? 1 : 0);
    }

    // SEのON/OFFを設定するメソッド
    public void SetSe(bool on)
    {
        IsSeOn = on;
        PlayerPrefs.SetInt("SE_ON", on ? 1 : 0);
    }

    // SE再生用メソッド
    public void PlaySe(AudioClip clip)
    {
        if (IsSeOn && seSource != null && clip != null)
        {
            seSource.PlayOneShot(clip);
        }
    }

    // 現在の設定をAudioSourceに反映し、BGMのON/OFFイベントも発行
    private void ApplySettings()
    {
        if (bgmSource != null)
        {
            bgmSource.mute = !IsBgmOn;
        }

        // BGMのミュート状態をイベントで通知
        OnBgmMuteChanged?.Invoke(!IsBgmOn);
    }

    // PlayerPrefsから設定を読み込む
    private void LoadSettings()
    {
        IsBgmOn = PlayerPrefs.GetInt("BGM_ON", 1) == 1;
        IsSeOn = PlayerPrefs.GetInt("SE_ON", 1) == 1;
    }
}
