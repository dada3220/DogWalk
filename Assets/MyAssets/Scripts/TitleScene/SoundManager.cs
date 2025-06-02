using UnityEngine;

// �T�E���h�S�̂��Ǘ�����V���O���g���N���X
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource bgmSource;  // BGM�pAudioSource�iInspector�ŃZ�b�g�j
    [SerializeField] private AudioSource seSource;   // SE�pAudioSource�iInspector�ŃZ�b�g�j

    // BGM��SE��ON/OFF���
    public bool IsBgmOn { get; private set; } = true;
    public bool IsSeOn { get; private set; } = true;

    // BGM��ON/OFF�؂�ւ����ɒʒm����C�x���g
    public delegate void OnBgmMuteChangedHandler(bool mute);
    public event OnBgmMuteChangedHandler OnBgmMuteChanged;

    private void Awake()
    {
        // �V���O���g���̏�����
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadSettings();  // PlayerPrefs����ݒ��ǂݍ���
        ApplySettings(); // �ǂݍ��񂾐ݒ�𔽉f
    }

    // BGM��ON/OFF��ݒ肷�郁�\�b�h
    public void SetBgm(bool on)
    {
        IsBgmOn = on;
        ApplySettings();
        PlayerPrefs.SetInt("BGM_ON", on ? 1 : 0);
    }

    // SE��ON/OFF��ݒ肷�郁�\�b�h
    public void SetSe(bool on)
    {
        IsSeOn = on;
        PlayerPrefs.SetInt("SE_ON", on ? 1 : 0);
    }

    // SE�Đ��p���\�b�h
    public void PlaySe(AudioClip clip)
    {
        if (IsSeOn && seSource != null && clip != null)
        {
            seSource.PlayOneShot(clip);
        }
    }

    // ���݂̐ݒ��AudioSource�ɔ��f���ABGM��ON/OFF�C�x���g�����s
    private void ApplySettings()
    {
        if (bgmSource != null)
        {
            bgmSource.mute = !IsBgmOn;
        }

        // BGM�̃~���[�g��Ԃ��C�x���g�Œʒm
        OnBgmMuteChanged?.Invoke(!IsBgmOn);
    }

    // PlayerPrefs����ݒ��ǂݍ���
    private void LoadSettings()
    {
        IsBgmOn = PlayerPrefs.GetInt("BGM_ON", 1) == 1;
        IsSeOn = PlayerPrefs.GetInt("SE_ON", 1) == 1;
    }
}
