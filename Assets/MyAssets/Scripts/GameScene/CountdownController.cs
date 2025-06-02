using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

public class CountdownController : MonoBehaviour
{
    [SerializeField] private TMP_Text countdownText;      // �J�E���g�_�E���\���p��Text
    [SerializeField] private GameManager gameManager;     // GameManager�ւ̎Q��

    private void Start()
    {
        // �J�E���g�_�E��������񓯊��ŊJ�n�iawait���Ȃ��̂� Forget�j
        RunCountdownAsync().Forget();
    }

    // �񓯊��ŃJ�E���g�_�E���{�Q�[���J�n�����s
    private async UniTask RunCountdownAsync()
    {
        await ShowCountdownAsync();       // �J�E���g�_�E���\��
        gameManager.StartGame();          // �Q�[���J�n
    }

    // �J�E���g�_�E���\������
    private async UniTask ShowCountdownAsync()
    {
        countdownText.gameObject.SetActive(true);          // �e�L�X�g�\��

        string[] countdownWords = {"READY", "3", "2", "1", "START" };

        foreach (var word in countdownWords)
        {
            countdownText.text = word;
            await UniTask.Delay(1000, DelayType.Realtime); // 1�b�ҋ@�iTime.timeScale�Ɋ֌W�Ȃ��j
        }

        countdownText.gameObject.SetActive(false);         // �e�L�X�g��\��
    }
}
