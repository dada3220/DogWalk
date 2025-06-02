using UnityEngine;
using UnityEngine.SceneManagement;

// �e�V�[�����Ƃ�BGM���Ǘ�����N���X
public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;

    private AudioSource audioSource;  // ���ۂ�BGM���Đ�����AudioSource

    // �V�[������BGM�N���b�v�̃y�A
    [System.Serializable]
    public class BGMData
    {
        public string sceneName;
        public AudioClip bgmClip;
    }

    public BGMData[] bgms;  // Inspector�Őݒ�

    private void Awake()
    {
        // �V���O���g����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;

            // �V�[���ǂݍ��݊������̃C�x���g�o�^
            SceneManager.sceneLoaded += OnSceneLoaded;

            // SoundManager��BGM�~���[�g�ؑփC�x���g���w��
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
        // �C�x���g�o�^�����i�I�u�W�F�N�g�j�����j
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.OnBgmMuteChanged -= OnBgmMuteChanged;
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // �V�[���؂�ւ����ɌĂ΂��
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayBGMForScene(scene.name);
    }

    // �V�[�����ɑΉ�����BGM���Đ�
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

        // �Ή�BGM���Ȃ���Β�~
        audioSource.Stop();
    }

    // SoundManager��BGM�~���[�g��ԕύX���ɌĂ΂��R�[���o�b�N
    private void OnBgmMuteChanged(bool mute)
    {
        if (audioSource != null)
        {
            audioSource.mute = mute;
        }
    }
}
