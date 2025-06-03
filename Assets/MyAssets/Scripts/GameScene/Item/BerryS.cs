using UnityEngine;
using Cysharp.Threading.Tasks; // �� �Y�ꂸ�ɁI

public class BerryS : Item
{
    public int playerScoreValue = 100;       // �v���C���[����������̃X�R�A
    public int dogAffectionValue = 0;       // ������������̍D���x
    public float poopIntervalMultiplier = 0.05f; // ���񂿊Ԋu�̒Z�k�{���i0.1 = 10%�ɒZ�k�j

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

        // �񓯊��ł��񂿊Ԋu�Z�k���ꎞ�I�ɓK�p
        _ = ApplyPoopIntervalEffectAsync(); // Forget() �͕s�v�i�񓯊��ő҂K�v���Ȃ���΁j

        Destroy(gameObject);
    }

    // �ꎞ�I�ɂ��񂿐����Ԋu��Z�k���鏈���i3�b��ɖ߂��j
    private async UniTask ApplyPoopIntervalEffectAsync()
    {
        if (dog == null) return;

        DogController dogController = dog.GetComponent<DogController>();
        if (dogController == null) return;

        float originalInterval = dogController.poopInterval;
        dogController.poopInterval *= poopIntervalMultiplier;

        await UniTask.Delay(3000); // 3�b�҂�

        if (dogController != null)
        {
            dogController.poopInterval = originalInterval;
        }
    }
}
