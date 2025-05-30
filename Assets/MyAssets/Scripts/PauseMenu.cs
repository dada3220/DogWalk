using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public Button resumeButton;
    public Button returnToTitleButton;
    public TMP_Text hintText;

    private bool isPaused = false;
    private bool inputLock = false; // �A�����͖h�~

    private string[] hints = new string[]
    {
        "�q���g: �A�C�e���́A�v���C���[����������ƌ�����������ƂŌ��ʂ��ς���I" +
        "��{�I�ɂ̓A�C�e���͐l�Ԃ���낤�I",

        "�q���g: �쐶�̐������͊�Ȃ�����ނ�݂ɋ߂Â��Ȃ��悤�ɂ��悤�I" +
        "�����������ɐG��Ƌ������ē����������Ȃ��I",

        "�q���g: ���͂��܂Ɍ����@���I�@�ꂻ���Ȓn�ʂ�T���Ă݂悤�I",
        "�q���g: �����̌��͐؂芔�����C�ɓ���I�ڂɓ���ƈӒn�ł��߂��ɍs���ă}�[�L���O�����I"
    };

    void Start()
    {
        pauseMenuUI.SetActive(false);

        // �{�^���ɃC�x���g��ݒ�
        resumeButton.onClick.AddListener(() => ResumeGame().Forget());
        returnToTitleButton.onClick.AddListener(() => ReturnToTitle().Forget());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !inputLock)
        {
            inputLock = true;
            if (isPaused)
                ResumeGame().Forget();
            else
                PauseGame().Forget();
        }
    }

    async UniTaskVoid PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pauseMenuUI.SetActive(true);

        // �����_���q���g�\��
        int i = Random.Range(0, hints.Length);
        hintText.text = hints[i];

        // �Q�[���ĊJ�{�^����I����Ԃɂ���
        EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);

        await UniTask.Delay(200);
        inputLock = false;
    }

    async UniTaskVoid ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);
        await UniTask.Delay(200);
        inputLock = false;
    }

    async UniTaskVoid ReturnToTitle()
    {
        Time.timeScale = 1f;
        await UniTask.Delay(100); // �O�̂��ߏ����҂�
        SceneManager.LoadScene("TitleScene");
    }
}
