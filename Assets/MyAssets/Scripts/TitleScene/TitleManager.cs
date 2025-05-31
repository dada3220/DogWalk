using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using TMPro;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Image fadePanel;
    [SerializeField] private float fadeDuration = 1f;

    [SerializeField] private TMP_Text bestScoreText;

    private void Start()
    {
        startButton.onClick.AddListener(() => OnStartButtonClicked().Forget());

        // 非同期ベストスコア読み込み開始
        _ = ShowBestScoreAsync();
    }

    // 非同期でベストスコアを取得・表示
    private async UniTaskVoid ShowBestScoreAsync()
    {
        int best = await ScoreSaveSystem.GetBestScoreAsync();
        bestScoreText.text = $"Best Score: {best}";
    }

    // スタートボタンを押したとき
    private async UniTaskVoid OnStartButtonClicked()
    {
        await FadeOut();
        await SceneManager.LoadSceneAsync("GameScene");
    }

    // フェードアウト演出
    private async UniTask FadeOut()
    {
        fadePanel.gameObject.SetActive(true);
        var color = fadePanel.color;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            color.a = Mathf.Clamp01(time / fadeDuration);
            fadePanel.color = color;
            await UniTask.Yield();
        }
    }
}
