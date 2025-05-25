using UnityEngine;
using Cysharp.Threading.Tasks; // UniTask ���g�����߂ɕK�v

// FavoriteWood �N���X�F�v���C���[�܂��͌����E����A�C�e��
public class F_Wood : FieldItem
{
    public int playerScoreValue = 100;      // �v���C���[���E�������ɑ�����X�R�A
    public int dogAffectionValue = 10;      // �����E�������ɑ����鈤��x
    public float runDuration = 3f;          // �����ړ��̌p�����ԁi�b�j
    public float runSpeed = 3f;             // �����ړ��̑��x

    // �v���C���[���A�C�e�����E�������̏���
    protected override void OnPlayerCollect()
    {
        var scoreManager = FindFirstObjectByType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.AddScore(playerScoreValue); // �X�R�A���Z
        }

        Destroy(gameObject); // �A�C�e�����폜
    }

    // �����A�C�e�����E�������̏����i�񓯊��j
    protected override async void OnDogCollect()
    {
        if (AffinityManager.Instance != null)
        {
            AffinityManager.Instance.IncreaseAffection(dogAffectionValue);
        }

        // �A�C�e�������͐�ɂ��Ă���
        Destroy(gameObject);

        if (dog != null && player != null)
        {
            await ForceRunAsync();
        }
    }

    // �v���C���[�ƌ�����莞�ԋ����I�ɉ������Ɉړ������鏈���iAI�����~��Transform�ړ��j
    private async UniTask ForceRunAsync()
    {
        Animator dogAnimator = dog.GetComponent<Animator>();
        Animator playerAnimator = player.GetComponent<Animator>();

        // ����AI�X�N���v�g�i��: DogAI�j�ɒu�������Ă�������
        var dogAI = dog.GetComponent<MonoBehaviour>();

        // �v���C���[�̈ړ��X�N���v�g�i��: PlayerController�j�ɒu�������Ă�������
        var playerController = player.GetComponent<MonoBehaviour>();

        // ���̑���A�j���[�V�����Đ�
        if (dogAnimator != null)
        {
            dogAnimator.Play("dog_wood");
        }

        // �v���C���[�̑���A�j���[�V�����Đ�
        if (playerAnimator != null)
        {
            playerAnimator.Play("Player_s");
        }

        // AI������Έꎞ��~
        if (dogAI != null)
        {
            dogAI.enabled = false;
        }

        // �v���C���[�ړ��X�N���v�g���ꎞ��~
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        float timer = 0f;
        Vector3 moveVector = Vector3.down * runSpeed;

        // ��莞�ԃ��[�v
        while (timer < runDuration)
        {
            dog.transform.Translate(moveVector * Time.deltaTime);
            player.transform.Translate(moveVector * Time.deltaTime);

            timer += Time.deltaTime;
            await UniTask.Yield();
        }

        // AI���ĊJ
        if (dogAI != null)
        {
            dogAI.enabled = true;
        }

        // �v���C���[�ړ��X�N���v�g���ĊJ
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        // ���̃A�j���[�V�������A�C�h���ɖ߂�
        if (dogAnimator != null)
        {
            dogAnimator.Play("dog_s");
        }

        // �v���C���[�̃A�j���[�V�������A�C�h���ɖ߂�
        if (playerAnimator != null)
        {
            playerAnimator.Play("Player_sF");
        }
    }
}
