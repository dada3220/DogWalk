using UnityEngine;

public class GameManager : MonoBehaviour
{
    private bool isPlaying = false;

    public void StartGame()
    {
        isPlaying = true;
        Time.timeScale = 1f;
    }

    public bool IsPlaying => isPlaying;

}