using UnityEngine;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(Animator))]
public class Cat : Item
{
    public int dogAffectionValueDown = -10;     // �v���C���[����������̍D���x�_�E��
    public int dogAffectionValue = 10;    // ������������̍D���x�A�b�v
    public int runDuration = 5000;    // �����ړ��̌p�����ԁi�b�j

    public float speed = 2f;                // �L�̈ړ����x
    public float boostedSpeed = 8f;         // ���̃u�[�X�g�����x
    private Vector3 moveDirection;
    private bool collected = false;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    protected override void Start()
    {
        base.Start();

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (mainCamera != null)
        {
            Vector3 target = mainCamera.transform.position;
            target.z = 0;
            moveDirection = (target - transform.position).normalized;

            // �����ɉ����Ĕ��]�i�������Ȃ�flipX�j
            if (moveDirection.x < 0 && spriteRenderer != null)
            {
                spriteRenderer.flipX = true;
            }
        }

        // �A�j���[�V�����J�n�i��ɕ����j
        if (animator != null)
        {
            animator.SetBool("cat", true);
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
        // AffinityManager �ɍD���x���Z
        if (AffinityManager.Instance != null)
        {
            AffinityManager.Instance.IncreaseAffection(dogAffectionValueDown);
        }

        // ���g���폜
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
            dog.SetSpeed(boostedSpeed, true);

            if (AffinityManager.Instance != null)
            {
                AffinityManager.Instance.IncreaseAffection(dogAffectionValue);
            }

            await UniTask.Delay(runDuration);

            dog.ResetSpeed();
        }
    }
}
