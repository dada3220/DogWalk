using UnityEngine;

/// <summary>
/// �Q�[���S�̂̃T�E���h�iBGM/SE�j��ON/OFF�ƍĐ����Ǘ�����V���O���g���N���X
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource bgmSource;  // BGM �p AudioSource
    [SerializeField] private AudioSource seSource;   // SE �p AudioSource�i���g�p�����c���Ă����j

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

        LoadSettings();  // �ۑ��f�[�^�̓ǂݍ���
        ApplySettings(); // ��Ԃ𔽉f
    }

    /// <summary>
    /// BGM �� ON/OFF ��؂�ւ���
    /// </summary>
    public void SetBgm(bool on)
    {
        IsBgmOn = on;
        PlayerPrefs.SetInt("BGM_ON", on ? 1 : 0);
        ApplySettings();
    }

    /// <summary>
    /// SE �� ON/OFF ��؂�ւ���iSEManager �ɂ����f�j
    /// </summary>
    public void SetSe(bool on)
    {
        IsSeOn = on;
        PlayerPrefs.SetInt("SE_ON", on ? 1 : 0);

        // SEManager �Ƀ~���[�g��Ԃ�ʒm
        if (SEManager.Instance != null)
        {
            SEManager.Instance.SetMuteFromManager(!on);
        }
    }

    /// <summary>
    /// SE �𒼐ڍĐ��������Ƃ��p�iSEManager�����j
    /// </summary>
    public void PlaySe(AudioClip clip)
    {
        if (IsSeOn && seSource != null && clip != null)
        {
            seSource.PlayOneShot(clip);
        }
    }

    /// <summary>
    /// ���݂̐ݒ�� AudioSource �� SEManager �ɔ��f����
    /// </summary>
    private void ApplySettings()
    {
        if (bgmSource != null)
        {
            bgmSource.mute = !IsBgmOn;
        }

        OnBgmMuteChanged?.Invoke(!IsBgmOn);

        // SEManager �Ƀ~���[�g��Ԃ�n��
        if (SEManager.Instance != null)
        {
            SEManager.Instance.SetMuteFromManager(!IsSeOn);
        }
    }

    /// <summary>
    /// �ۑ�����Ă���ݒ�� PlayerPrefs ����ǂݍ���
    /// </summary>
    private void LoadSettings()
    {
        IsBgmOn = PlayerPrefs.GetInt("BGM_ON", 1) == 1;
        IsSeOn = PlayerPrefs.GetInt("SE_ON", 1) == 1;
    }
}
