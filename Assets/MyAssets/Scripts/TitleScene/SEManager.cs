using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ʉ��iSE�j�̍Đ��ƊǗ����s���V���O���g���N���X
/// SoundManager �ƘA�g���ă~���[�g��Ԃ𓯊�����
/// </summary>
public class SEManager : MonoBehaviour
{
    public static SEManager Instance;

    private AudioSource audioSource;

    [System.Serializable]
    public class SEData
    {
        public string name;      // ���ʉ��̖��O
        public AudioClip clip;   // �Ή�����AudioClip
    }

    [Header("�o�^����SE")]
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

            // SoundManager �����݂��Ă���΃~���[�g��Ԃ𓯊�
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
    /// ���ʉ��𖼑O�ōĐ�����i�~���[�g���͖����j
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
            Debug.LogWarning($"SE '{name}' ��������܂���");
        }
    }

    /// <summary>
    /// SoundManager ����~���[�g��Ԃ𔽉f���邽�߂̘A�g�p���\�b�h
    /// </summary>
    public void SetMuteFromManager(bool mute)
    {
        IsMuted = mute;
    }

    /// <summary>
    /// �P�̂Ń~���[�g�؂�ւ��������ꍇ�i�ʏ�͎g��Ȃ��j
    /// </summary>
    public void SetMute(bool mute)
    {
        IsMuted = mute;
    }
}
