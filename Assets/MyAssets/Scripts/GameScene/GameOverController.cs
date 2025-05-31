using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;

    private void Start()
    {
        if (AffinityManager.Instance != null)
        {
            AffinityManager.Instance.OnAffectionDepleted += HandleGameOver;
        }
    }

    private void OnDestroy()
    {
        if (AffinityManager.Instance != null)
        {
            AffinityManager.Instance.OnAffectionDepleted -= HandleGameOver;
        }
    }

    private void HandleGameOver()
    {
        GameScoreHolder.LastScore = scoreManager.CurrentScore;
        SceneManager.LoadScene("EndScene");
    }
}
