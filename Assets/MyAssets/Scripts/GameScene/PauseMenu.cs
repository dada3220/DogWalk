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
        "ヒント: アイテムは、プレイヤーが取った時と犬が取った時とで効果が変わるよ！基本的にはアイテムは人間が取ろう！",
        "ヒント: 野生の生き物は危ないからむやみに近づかないようにしよう！犬も生き物に触ると興奮して動きが速くなるよ！",
        "ヒント: 犬はたまに穴を掘るよ！掘れそうな地面を探してみよう！",
        "ヒント: うちの犬は切り株がお気に入り！目に入ると意地でも近くに行ってマーキングするよ！"
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
            // UIで何も選択していなくてもポーズ解除できるようにする
            if (isPaused)
            {
                ResumeGame().Forget();
            }
            else
            {
                PauseGame().Forget();
            }

            inputLock = true; // ロックをかけて二重入力防止
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
