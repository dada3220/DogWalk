using UnityEngine;

public class Wood : Item
{
    public int playerScoreValue = 100;     // �v���C���[����������̃X�R�A
    public int dogAffectionValue = 0;    // ������������̍D���x

    protected override void OnPlayerCollect()
    {
        // ScoreManager �ɃX�R�A���Z�i�V���O���g���܂��� FindObjectOfType�j
        ScoreManager scoreManager = FindFirstObjectByType<ScoreManager>();// �V�[�����ScoreManager������

        if (scoreManager != null)
        {
            scoreManager.AddScore(playerScoreValue);
        }

        // ���g���폜
        Destroy(gameObject);
    }

    protected override void OnDogCollect()
    {

        // AffinityManager �ɍD���x���Z
        if (AffinityManager.Instance != null)
        {
            AffinityManager.Instance.IncreaseAffection(dogAffectionValue);
        }

        // ���g���폜
        Destroy(gameObject);
    }
}