using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

public class CountdownController : MonoBehaviour
{
    [SerializeField] private TMP_Text countdownText;      // カウントダウン表示用のText
    [SerializeField] private GameManager gameManager;     // GameManagerへの参照

    private void Start()
    {
        // カウントダウン処理を非同期で開始（awaitしないので Forget）
        RunCountdownAsync().Forget();
    }

    // 非同期でカウントダウン＋ゲーム開始を実行
    private async UniTask RunCountdownAsync()
    {
        await ShowCountdownAsync();       // カウントダウン表示
        gameManager.StartGame();          // ゲーム開始
    }

    // カウントダウン表示処理
    private async UniTask ShowCountdownAsync()
    {
        countdownText.gameObject.SetActive(true);          // テキスト表示

        string[] countdownWords = {"READY", "3", "2", "1", "START" };

        foreach (var word in countdownWords)
        {
            countdownText.text = word;
            await UniTask.Delay(1000, DelayType.Realtime); // 1秒待機（Time.timeScaleに関係なく）
        }

        countdownText.gameObject.SetActive(false);         // テキスト非表示
    }
}
