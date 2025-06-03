using UnityEngine;

public class Wood : Item
{
    public int playerScoreValue = 100;     // プレイヤーが取った時のスコア
    public int dogAffectionValue = 0;    // 犬が取った時の好感度

    protected override void OnPlayerCollect()
    {
        // ScoreManager にスコア加算（シングルトンまたは FindObjectOfType）
        ScoreManager scoreManager = FindFirstObjectByType<ScoreManager>();// シーン上のScoreManagerを検索

        if (scoreManager != null)
        {
            scoreManager.AddScore(playerScoreValue);
        }

        // 自身を削除
        Destroy(gameObject);
    }

    protected override void OnDogCollect()
    {

        // AffinityManager に好感度加算
        if (AffinityManager.Instance != null)
        {
            AffinityManager.Instance.IncreaseAffection(dogAffectionValue);
        }

        // 自身を削除
        Destroy(gameObject);
    }
}