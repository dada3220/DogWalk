using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using TMPro;
using System.Collections.Generic;

public class TitleManager : MonoBehaviour
{
    // ==== ロゴ関連 ====
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform logoRect;
    [SerializeField] private Vector2 logoTargetPosition;
    [SerializeField] private Vector2 logoTargetScale = new Vector2(0.5f, 0.5f);
    [SerializeField] private float moveDuration = 0.5f;

    // ==== UI要素 ====
    [SerializeField] private Button startButton;
    [SerializeField] private TMP_Text bestScoreText;

    // ==== メニュー関連 ====
    [SerializeField] private GameObject rightMenu;
    [SerializeField] private Button gameStartButton;
    [SerializeField] private Button ruleButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button quitButton; 
    // ==== オプション画面関連 ====
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private Button closeOptionButton;

    // ==== ルール画面関連 ====
    [SerializeField] private GameObject rulePanel;         
    [SerializeField] private Button closeRuleButton; 

    // ==== 終了確認ダイアログ ====
    [SerializeField] private GameObject quitDialog;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    private void Start()
    {
        // 初期状態で非表示にする要素
        rightMenu.SetActive(false);
        bestScoreText.gameObject.SetActive(false);
        quitDialog.SetActive(false);
        optionPanel.SetActive(false);
        rulePanel.SetActive(false); 

        // ロゴの移動先を計算
        logoTargetPosition = CalculateLogoTargetPosition();

        // 各種ボタンのイベント登録
        startButton.onClick.AddListener(() => OnStartButtonClicked().Forget());
        gameStartButton.onClick.AddListener(() => LoadGame().Forget());
        quitButton.onClick.AddListener(ShowQuitDialog);
        yesButton.onClick.AddListener(QuitGame);
        noButton.onClick.AddListener(HideQuitDialog);
        optionButton.onClick.AddListener(ShowOptionPanel);
        closeOptionButton.onClick.AddListener(HideOptionPanel);
        ruleButton.onClick.AddListener(ShowRulePanel);        
        closeRuleButton.onClick.AddListener(HideRulePanel);  
    }

    // ロゴの目標位置（左上1/4中央）を画面サイズから計算
    private Vector2 CalculateLogoTargetPosition()
    {
        if (logoRect == null || canvas == null)
        {
            return Vector2.zero;
        }

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        return new Vector2(-canvasRect.rect.width / 5f, canvasRect.rect.height / 5f);
    }

    // スタートボタン押下時の処理
    private async UniTaskVoid OnStartButtonClicked()
    {
        startButton.interactable = false;
        startButton.gameObject.SetActive(false); // ボタンを非表示

        // ロゴ移動＋縮小
        await MoveAndScaleLogo();

        // ベストスコア取得・表示
        int bestScore = await ScoreSaveSystem.GetBestScoreAsync();
        bestScoreText.text = $"BEST SCORE \n{bestScore}";
        bestScoreText.gameObject.SetActive(true);

        // メニューをフェード表示
        await ShowRightMenuAsync();
    }

    // ロゴの移動＋スケール変更アニメーション
    private async UniTask MoveAndScaleLogo()
    {
        Vector2 startPos = logoRect.anchoredPosition;
        Vector2 startScale = logoRect.localScale;
        float time = 0f;

        while (time < moveDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / moveDuration);
            float eased = EaseInOutQuad(t);

            logoRect.anchoredPosition = Vector2.Lerp(startPos, logoTargetPosition, eased);
            logoRect.localScale = Vector2.Lerp(startScale, logoTargetScale, eased);
            await UniTask.Yield();
        }

        // 最終値をしっかり設定
        logoRect.anchoredPosition = logoTargetPosition;
        logoRect.localScale = logoTargetScale;
    }

    // メニューの各ボタンを順番にフェード表示
    private async UniTask ShowRightMenuAsync()
    {
        rightMenu.SetActive(true);

        // 全ての子にCanvasGroupを設定し、初期状態を0にする
        var canvasGroups = new List<CanvasGroup>();

        foreach (Transform child in rightMenu.transform)
        {
            var cg = child.GetComponent<CanvasGroup>() ?? child.gameObject.AddComponent<CanvasGroup>();
            cg.alpha = 0f;
            cg.interactable = false;
            cg.blocksRaycasts = false;
            child.gameObject.SetActive(true);
            canvasGroups.Add(cg);
        }

        // 全体を一括でフェードイン
        float fadeTime = 0.3f;
        float time = 0f;
        while (time < fadeTime)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Clamp01(time / fadeTime);

            foreach (var cg in canvasGroups)
            {
                cg.alpha = alpha;
            }

            await UniTask.Yield();
        }

        // 最終的に全て有効化
        foreach (var cg in canvasGroups)
        {
            cg.alpha = 1f;
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }
    }

    // ゲームシーンへ遷移
    private async UniTaskVoid LoadGame()
    {
        await SceneManager.LoadSceneAsync("GameScene");
    }

    // オプション表示・非表示
    private void ShowOptionPanel()
    {
        optionPanel.SetActive(true);
    }

    private void HideOptionPanel()
    {
        optionPanel.SetActive(false);
    }

    // ルールパネル表示
    private void ShowRulePanel()
    {
        rulePanel.SetActive(true);
    }

    // ルールパネル非表示
    private void HideRulePanel()
    {
        rulePanel.SetActive(false);
    }

    // 終了確認ダイアログを表示
    private void ShowQuitDialog()
    {
        quitDialog.SetActive(true);
    }

    // ダイアログを非表示（キャンセル）
    private void HideQuitDialog()
    {
        quitDialog.SetActive(false);
    }

    // 実際にゲームを終了
    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // エディタで停止
#else
        Application.Quit(); // 実機でアプリ終了
#endif
    }

    // 緩急つけたイージング関数
    private float EaseInOutQuad(float t)
    {
        return t < 0.5f ? 2f * t * t : -1f + (4f - 2f * t) * t;
    }
}
