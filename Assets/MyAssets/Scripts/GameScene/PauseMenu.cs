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
        "ヒント: 野生の蛇は危ないからむやみに近づかないようにしよう！ポイントが減るよ！犬も蛇に触ると興奮して動きが速くなるよ！",
        "ヒント: 野良猫に触ると犬が嫉妬して好感度が下がるよ！犬も猫に触ると興奮して動きが速くなるよ！",
        "ヒント: 犬はたまに穴を掘るよ！いいものが出てくるかも！掘れそうな地面を探してみよう！",
        "ヒント: うちの犬は切り株がお気に入り！目に入ると意地でも近くに行ってマーキングするよ！",
        "ヒント: 木の棒は取るとポイントになるよ！犬が取っても好感度が上がるけど、気に入った棒を拾ったら走り出すよ！",
        "ヒント: 木の実は取るとポイントになるよ！犬が取っても好感度が上がるけど、腐った木の実を食べるとお腹を壊すよ！",
        "ヒント: 飼い主として、犬のうんこはちゃんと処理しよう！長時間放置したり、そのまま立ち去ると犬の好感度が下がるよ！",
        "ヒント: 犬との距離が近いほど交換度は上がるけど、触れるほど近すぎると怒られるよ！程々の距離で！",
        "ヒント: 時にはリードを引っ張って犬を危険から遠ざけよう！好感度が下がるけど仕方がない！",
        "ヒント: イガグリを取ると、スコアは減るし好感度も下がるよ！拾わないようにしよう！"
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
