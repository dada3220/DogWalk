using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using TMPro;

public class EndSceneController : MonoBehaviour
{
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text bestScoreMessageText;
    [SerializeField] private TMP_Text ratingText;

    [SerializeField] private Button returnToTitleButton;
    [SerializeField] private Button retryButton;

    private void Start()
    {
        _ = HandleEndSceneAsync();

        returnToTitleButton.onClick.AddListener(() => ReturnToTitle().Forget());
        retryButton.onClick.AddListener(() => Retry().Forget());
    }

    private async UniTaskVoid HandleEndSceneAsync()
    {
        int currentScore = GameScoreHolder.LastScore;
        resultText.text = $"SCORE: {currentScore}";

        // ベストスコア保存
        bool isNewBest = await ScoreSaveSystem.SaveIfBestAsync(currentScore);

        // ベストスコア更新メッセージ
        bestScoreMessageText.text = isNewBest ? " BEST SCORE更新！" : "";

        // スコアによってランク表示
        ratingText.text = $"{GetRating(currentScore)}ランク";
    }

    private string GetRating(int score)
    {
        if (score >= 5000) return "S";
        if (score >= 3000) return "A";
        if (score >= 1000) return "B";
        if (score >= 500) return "C";
        return "D";
    }

    private async UniTaskVoid ReturnToTitle()
    {
        await SceneManager.LoadSceneAsync("TitleScene");
    }

    private async UniTaskVoid Retry()
    {
        await SceneManager.LoadSceneAsync("GameScene");
    }
}
