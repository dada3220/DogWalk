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
        resultText.text = $"Your Score: {currentScore}";

        // �x�X�g�X�R�A�ۑ�
        bool isNewBest = await ScoreSaveSystem.SaveIfBestAsync(currentScore);

        // �x�X�g�X�R�A�X�V���b�Z�[�W
        bestScoreMessageText.text = isNewBest ? " New Best Score!" : "";

        // �X�R�A�ɂ���ă����N�\��
        ratingText.text = $"Rank: {GetRating(currentScore)}";
    }

    private string GetRating(int score)
    {
        if (score >= 1000) return "S";
        if (score >= 700) return "A";
        if (score >= 400) return "B";
        if (score >= 200) return "C";
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
