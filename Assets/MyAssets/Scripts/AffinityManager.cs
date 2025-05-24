using UnityEngine;
using UnityEngine.UI;

public class AffinityManager : MonoBehaviour
{
    public static AffinityManager Instance { get; private set; }

    public int affection = 100; // 現在の好感度
    public int maxAffection = 100; // 好感度の最大値

    public Slider affectionSlider; // UIスライダーへの参照

    private void Awake()
    {
        // シングルトン設定
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        // スライダー初期化
        if (affectionSlider != null)
        {
            affectionSlider.maxValue = maxAffection;
            affectionSlider.value = affection;
        }
    }

    // 好感度増加
    public void IncreaseAffection(int amount)
    {
        affection += amount;
        affection = Mathf.Clamp(affection, 0, maxAffection);
        UpdateSlider();
    }

    // 好感度減少（0以下ならゲームオーバー）
    public void DecreaseAffection(int amount)
    {
        affection -= amount;
        affection = Mathf.Clamp(affection, 0, maxAffection);
        UpdateSlider();

        if (affection <= 0)
        {
            GameOver();
        }
    }

    // UI更新
    private void UpdateSlider()
    {
        if (affectionSlider != null)
        {
            affectionSlider.value = affection;
        }
    }

    // ゲームオーバー処理（好感度0）
    private void GameOver()
    {
        Debug.Log("Game Over! Dog lost trust.");
        // 今後の拡張でUI停止や遷移追加可
    }

    // 外部から好感度を取得
    public int GetAffection() => affection;
}
