using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject confirmDialogUI;
    public Button resumeButton;
    public Button returnToTitleButton;
    public Button confirmYesButton;
    public Button confirmNoButton;
    public TMP_Text hintText;

    private bool isPaused = false;
    private bool inputLock = false;

    private string[] hints = new string[]
    {
        "�q���g: �A�C�e���́A�v���C���[����������ƌ�����������ƂŌ��ʂ��ς���I��{�I�ɂ̓A�C�e���͐l�Ԃ���낤�I",
        "�q���g: �쐶�̐������͊�Ȃ�����ނ�݂ɋ߂Â��Ȃ��悤�ɂ��悤�I�����������ɐG��Ƌ������ē����������Ȃ��I",
        "�q���g: ���͂��܂Ɍ����@���I�@�ꂻ���Ȓn�ʂ�T���Ă݂悤�I",
        "�q���g: �����̌��͐؂芔�����C�ɓ���I�ڂɓ���ƈӒn�ł��߂��ɍs���ă}�[�L���O�����I"
    };

    void Start()
    {
        pauseMenuUI.SetActive(false);
        confirmDialogUI.SetActive(false);

        resumeButton.onClick.AddListener(() => ResumeGame().Forget());
        returnToTitleButton.onClick.AddListener(() => ShowConfirmDialog());

        confirmYesButton.onClick.AddListener(() => ConfirmReturnToTitle().Forget());
        confirmNoButton.onClick.AddListener(() => CancelConfirmDialog());
    }

    void Update()
    {
       
        if (Input.GetKeyDown(KeyCode.Space) && !inputLock && !confirmDialogUI.activeSelf)
        {
            // UI�ŉ����I�����Ă��Ȃ��Ă��|�[�Y�����ł���悤�ɂ���
            if (isPaused)
            {
                ResumeGame().Forget();
            }
            else
            {
                PauseGame().Forget();
            }

            inputLock = true; // ���b�N�������ē�d���͖h�~
        }
    }


    async UniTaskVoid PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pauseMenuUI.SetActive(true);
        hintText.text = hints[Random.Range(0, hints.Length)];
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

    void ShowConfirmDialog()
    {
        confirmDialogUI.SetActive(true);
        EventSystem.current.SetSelectedGameObject(confirmNoButton.gameObject);
    }

    void CancelConfirmDialog()
    {
        confirmDialogUI.SetActive(false);
    }

    async UniTaskVoid ConfirmReturnToTitle()
    {
        Time.timeScale = 1f;
        confirmDialogUI.SetActive(false);
        await UniTask.Delay(100);
        SceneManager.LoadScene("TitleScene");
    }
}
