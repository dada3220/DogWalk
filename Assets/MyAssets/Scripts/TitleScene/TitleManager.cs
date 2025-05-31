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

        // �񓯊��x�X�g�X�R�A�ǂݍ��݊J�n
        _ = ShowBestScoreAsync();
    }

    // �񓯊��Ńx�X�g�X�R�A���擾�E�\��
    private async UniTaskVoid ShowBestScoreAsync()
    {
        int best = await ScoreSaveSystem.GetBestScoreAsync();
        bestScoreText.text = $"Best Score: {best}";
    }

    // �X�^�[�g�{�^�����������Ƃ�
    private async UniTaskVoid OnStartButtonClicked()
    {
        await FadeOut();
        await SceneManager.LoadSceneAsync("GameScene");
    }

    // �t�F�[�h�A�E�g���o
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
