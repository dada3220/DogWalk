using System.Collections.Generic;
using UnityEngine;

// ���ʉ�(SE)�̍Đ��ƊǗ����s���V���O���g���N���X
public class SEManager : MonoBehaviour
{
    // �V���O���g���C���X�^���X
    public static SEManager Instance;

    private AudioSource audioSource;

    // ���ʉ��f�[�^�i���O��AudioClip�̃Z�b�g�j
    [System.Serializable]
    public class SEData
    {
        public string name;      // ���ʉ��̎��ʖ�
        public AudioClip clip;   // �Ή�����AudioClip
    }

    [Header("�o�^����SE")]
    public List<SEData> seList = new List<SEData>(); // Inspector �œo�^����SE�̃��X�g

    // ���O��AudioClip �̎����i�����A�N�Z�X�p�j
    private Dictionary<string, AudioClip> seDict;

    // SE���~���[�g����Ă��邩�ǂ���
    public bool IsMuted { get; private set; } = false;

    private void Awake()
    {
        // �V���O���g���������i���ɑ��݂���Δj���j
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �V�[�����ׂ��ŕێ�

            // AudioSource�������I�ɒǉ�
            audioSource = gameObject.AddComponent<AudioSource>();
            seDict = new Dictionary<string, AudioClip>();

            // ���X�g���玫���ɓo�^�i���O�d���͖����j
            foreach (var se in seList)
            {
                if (!seDict.ContainsKey(se.name))
                    seDict.Add(se.name, se.clip);
            }
        }
        else
        {
            Destroy(gameObject); // �����͔j��
        }
    }

    // ���ʉ��𖼑O�ōĐ�����
    public void Play(string name)
    {
        if (IsMuted) return; // �~���[�g���Ȃ牽�����Ȃ�

        if (seDict.TryGetValue(name, out var clip))
        {
            audioSource.PlayOneShot(clip); // ��x�����Đ�
        }
        else
        {
            Debug.LogWarning($"SE '{name}' ��������܂���");
        }
    }

    // �O���i�I�v�V������ʂȂǁj����SE�̃~���[�g��؂�ւ���
    public void SetMute(bool mute)
    {
        IsMuted = mute;
    }
}
