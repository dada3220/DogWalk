using UnityEngine;

public class GameManager : MonoBehaviour
{
    private bool isPlaying = false;

    public void StartGame()
    {
        isPlaying = true;
        Time.timeScale = 1f;
        // スコア・タイマー開始、操作受付など
    }

    public bool IsPlaying => isPlaying;

}