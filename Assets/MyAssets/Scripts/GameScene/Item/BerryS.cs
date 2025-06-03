using UnityEngine;
using Cysharp.Threading.Tasks; // ← 忘れずに！

public class BerryS : Item
{
    public int playerScoreValue = 100;       // プレイヤーが取った時のスコア
    public int dogAffectionValue = 0;       // 犬が取った時の好感度
    public float poopIntervalMultiplier = 0.05f; // うんち間隔の短縮倍率（0.1 = 10%に短縮）

    protected override void OnPlayerCollect()
    {
        var scoreManager = FindFirstObjectByType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.AddScore(playerScoreValue);
        }

        Destroy(gameObject);
    }

    protected override void OnDogCollect()
    {
        if (AffinityManager.Instance != null)
        {
            AffinityManager.Instance.IncreaseAffection(dogAffectionValue);
        }

        // 非同期でうんち間隔短縮を一時的に適用
        _ = ApplyPoopIntervalEffectAsync(); // Forget() は不要（非同期で待つ必要がなければ）

        Destroy(gameObject);
    }

    // 一時的にうんち生成間隔を短縮する処理（3秒後に戻す）
    private async UniTask ApplyPoopIntervalEffectAsync()
    {
        if (dog == null) return;

        DogController dogController = dog.GetComponent<DogController>();
        if (dogController == null) return;

        float originalInterval = dogController.poopInterval;
        dogController.poopInterval *= poopIntervalMultiplier;

        await UniTask.Delay(3000); // 3秒待つ

        if (dogController != null)
        {
            dogController.poopInterval = originalInterval;
        }
    }
}
