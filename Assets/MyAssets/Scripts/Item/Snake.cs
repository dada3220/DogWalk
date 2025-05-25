using UnityEngine;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(Animator))]
public class Snake : FieldItem
{
    public int playerScoreValue = -100;     // �v���C���[����������̃X�R�A
    public int dogAffectionValue = -10;     // ������������̍D���x
    public int runDuration = 3000;          // �u�[�X�g�p�����ԁi�~���b�j

    public float speed = 2f;                // �ւ̈ړ����x
    public float boostedSpeed = 10f;        // �u�[�X�g���̌��̑��x
    private Vector3 moveDirection;
    private bool collected = false;

    private Animator animator;

    protected override void Start()
    {
        base.Start();

        animator = GetComponent<Animator>();

        if (mainCamera != null)
        {
            Vector3 target = mainCamera.transform.position;
            target.z = 0;
            moveDirection = (target - transform.position).normalized;

            // �����ɉ����ăX�v���C�g����]������i�A�j���[�V���������L����j
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        // �A�j���[�V�����J�n�i��ɕ����j
        if (animator != null)
        {
            animator.SetBool("snake", true);
        }
    }

    protected override void Update()
    {
        base.Update();

        if (collected) return;

        // �J�������S�Ɍ������Ĉړ�
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    protected override void OnPlayerCollect()
    {
        ScoreManager scoreManager = FindFirstObjectByType<ScoreManager>();

        if (scoreManager != null)
        {
            scoreManager.AddScore(playerScoreValue);
        }

        Destroy(gameObject);
    }

    protected override void OnDogCollect()
    {
        if (collected) return;
        collected = true;

        BoostDog().Forget();

        Destroy(gameObject);
    }

    private async UniTaskVoid BoostDog()
    {
        DogController dog = FindFirstObjectByType<DogController>();
        if (dog != null)
        {
            float originalSpeed = dog.speed;

            dog.speed = boostedSpeed;

            if (AffinityManager.Instance != null)
            {
                AffinityManager.Instance.IncreaseAffection(dogAffectionValue);
            }

            await UniTask.Delay(runDuration);

            dog.speed = originalSpeed;
        }
    }
}
