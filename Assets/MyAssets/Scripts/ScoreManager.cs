using UnityEngine;
using TMPro; // TextMeshPro の使用

public class ScoreManager : MonoBehaviour
{
    public TMP_Text scoreText; // スコア表示用テキスト（TextMeshPro）

    private int score = 0; // 現在のスコア

    // スコア加算処理（他スクリプトから呼び出し）
    public void AddScore(int amount)
    {
        score += amount;
        scoreText.text = "Score: " + score;
    }
}
