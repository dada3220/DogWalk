using UnityEngine;

public class GameManager : MonoBehaviour
{
    private bool isPlaying = false;

    public void StartGame()
    {
        isPlaying = true;
        Time.timeScale = 1f;
        // �X�R�A�E�^�C�}�[�J�n�A�����t�Ȃ�
    }

    public bool IsPlaying => isPlaying;

}