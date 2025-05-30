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
    private bool inputLock = false; // 連続入力防止

    private string[] hints = new string[]
    {
        "ヒント: アイテムは、プレイヤーが取った時と犬が取った時とで効果が変わるよ！" +
        "基本的にはアイテムは人間が取ろう！",

        "ヒント: 野生の生き物は危ないからむやみに近づかないようにしよう！" +
        "犬も生き物に触ると興奮して動きが速くなるよ！",

        "ヒント: 犬はたまに穴を掘るよ！掘れそうな地面を探してみよう！",
        "ヒント: うちの犬は切り株がお気に入り！目に入ると意地でも近くに行ってマーキングするよ！"
    };

    void Start()
    {
        pauseMenuUI.SetActive(false);

        // ボタンにイベントを設定
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

        // ランダムヒント表示
        int i = Random.Range(0, hints.Length);
        hintText.text = hints[i];

        // ゲーム再開ボタンを選択状態にする
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
        await UniTask.Delay(100); // 念のため少し待つ
        SceneManager.LoadScene("TitleScene");
    }
}
