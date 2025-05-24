using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public Transform player;
    public Transform dog;
    public float scoringDistance = 3f;
    public TMP_Text scoreText;  // Text �ł͂Ȃ� TMP_Text ���g��

    private int score = 0;
    private float checkInterval = 0.3f;
    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= checkInterval)
        {
            float distance = Vector3.Distance(player.position, dog.position);
            if (distance <= scoringDistance)
            {
                score += 10;
                scoreText.text = "Score: " + score;
            }
            timer = 0f;
        }
    }

    public void AddScore(int amount)//�X�R�A�̉��Z
    {
        score += amount;
        scoreText.text = "Score: " + score;
    }

}
