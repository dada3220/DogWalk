using System.Collections.Generic;
using UnityEngine;

// 効果音(SE)の再生と管理を行うシングルトンクラス
public class SEManager : MonoBehaviour
{
    // シングルトンインスタンス
    public static SEManager Instance;

    private AudioSource audioSource;

    // 効果音データ（名前とAudioClipのセット）
    [System.Serializable]
    public class SEData
    {
        public string name;      // 効果音の識別名
        public AudioClip clip;   // 対応するAudioClip
    }

    [Header("登録するSE")]
    public List<SEData> seList = new List<SEData>(); // Inspector で登録するSEのリスト

    // 名前→AudioClip の辞書（高速アクセス用）
    private Dictionary<string, AudioClip> seDict;

    // SEがミュートされているかどうか
    public bool IsMuted { get; private set; } = false;

    private void Awake()
    {
        // シングルトン化処理（既に存在すれば破棄）
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンを跨いで保持

            // AudioSourceを自動的に追加
            audioSource = gameObject.AddComponent<AudioSource>();
            seDict = new Dictionary<string, AudioClip>();

            // リストから辞書に登録（名前重複は無視）
            foreach (var se in seList)
            {
                if (!seDict.ContainsKey(se.name))
                    seDict.Add(se.name, se.clip);
            }
        }
        else
        {
            Destroy(gameObject); // 複製は破棄
        }
    }

    // 効果音を名前で再生する
    public void Play(string name)
    {
        if (IsMuted) return; // ミュート中なら何もしない

        if (seDict.TryGetValue(name, out var clip))
        {
            audioSource.PlayOneShot(clip); // 一度だけ再生
        }
        else
        {
            Debug.LogWarning($"SE '{name}' が見つかりません");
        }
    }

    // 外部（オプション画面など）からSEのミュートを切り替える
    public void SetMute(bool mute)
    {
        IsMuted = mute;
    }
}
