using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 効果音（SE）の再生と管理を行うシングルトンクラス
/// SoundManager と連携してミュート状態を同期する
/// </summary>
public class SEManager : MonoBehaviour
{
    public static SEManager Instance;

    private AudioSource audioSource;

    [System.Serializable]
    public class SEData
    {
        public string name;      // 効果音の名前
        public AudioClip clip;   // 対応するAudioClip
    }

    [Header("登録するSE")]
    public List<SEData> seList = new List<SEData>();

    private Dictionary<string, AudioClip> seDict;

    public bool IsMuted { get; private set; } = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = gameObject.AddComponent<AudioSource>();
            seDict = new Dictionary<string, AudioClip>();
            foreach (var se in seList)
            {
                if (!seDict.ContainsKey(se.name))
                    seDict.Add(se.name, se.clip);
            }

            // SoundManager が存在していればミュート状態を同期
            if (SoundManager.Instance != null)
            {
                SetMuteFromManager(!SoundManager.Instance.IsSeOn);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 効果音を名前で再生する（ミュート時は無効）
    /// </summary>
    public void Play(string name)
    {
        if (IsMuted) return;

        if (seDict.TryGetValue(name, out var clip))
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"SE '{name}' が見つかりません");
        }
    }

    /// <summary>
    /// SoundManager からミュート状態を反映するための連携用メソッド
    /// </summary>
    public void SetMuteFromManager(bool mute)
    {
        IsMuted = mute;
    }

    /// <summary>
    /// 単体でミュート切り替えしたい場合（通常は使わない）
    /// </summary>
    public void SetMute(bool mute)
    {
        IsMuted = mute;
    }
}
