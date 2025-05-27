using UnityEngine;
using TMPro; // TextMeshPro �̎g�p

public class ScoreManager : MonoBehaviour
{
    public TMP_Text scoreText; // �X�R�A�\���p�e�L�X�g�iTextMeshPro�j

    private int score = 0; // ���݂̃X�R�A

    // �X�R�A���Z�����i���X�N���v�g����Ăяo���j
    public void AddScore(int amount)
    {
        score += amount;
        scoreText.text = "Score: " + score;
    }
}
