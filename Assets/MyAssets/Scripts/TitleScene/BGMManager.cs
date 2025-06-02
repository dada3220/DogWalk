using UnityEngine;
using UnityEngine.SceneManagement;

// 各シーンごとのBGMを管理するクラス
public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;

    private AudioSource audioSource;  // 実際にBGMを再生するAudioSource

    // シーン名とBGMクリップのペア
    [System.Serializable]
    public class BGMData
    {
        public string sceneName;
        public AudioClip bgmClip;
    }

    public BGMData[] bgms;  // Inspectorで設定

    private void Awake()
    {
        // シングルトン化
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;

            // シーン読み込み完了時のイベント登録
            SceneManager.sceneLoaded += OnSceneLoaded;

            // SoundManagerのBGMミュート切替イベントを購読
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.OnBgmMuteChanged += OnBgmMuteChanged;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // イベント登録解除（オブジェクト破棄時）
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.OnBgmMuteChanged -= OnBgmMuteChanged;
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // シーン切り替え時に呼ばれる
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayBGMForScene(scene.name);
    }

    // シーン名に対応したBGMを再生
    private void PlayBGMForScene(string sceneName)
    {
        foreach (var bgm in bgms)
        {
            if (bgm.sceneName == sceneName)
            {
                if (audioSource.clip != bgm.bgmClip)
                {
                    audioSource.clip = bgm.bgmClip;
                    audioSource.mute = !SoundManager.Instance?.IsBgmOn ?? false;
                    audioSource.Play();
                }
                return;
            }
        }

        // 対応BGMがなければ停止
        audioSource.Stop();
    }

    // SoundManagerのBGMミュート状態変更時に呼ばれるコールバック
    private void OnBgmMuteChanged(bool mute)
    {
        if (audioSource != null)
        {
            audioSource.mute = mute;
        }
    }
}
