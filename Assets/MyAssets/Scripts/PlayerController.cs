using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f; // �v���C���[�̊�{�ړ����x

    public Transform dog; // DogController�ɐڑ�
    public float pullSpeed = 2f; // ��������������Ƃ��̈ړ����x
    public float leashMaxLength = 5f; // ���[�h�̍ő咷��

    private DogController dogController; // DogController�̎Q��
    private Animator animator; // �v���C���[��Animator�i�A�j���[�V��������p�j
    private Vector2 lastMoveDir = Vector2.down; // �Ō�̈ړ������iIdle���Ɍ������ێ��j

    void Start()
    {
        // �����ݒ肳��Ă���΁ADogController���擾
        if (dog != null)
        {
            dogController = dog.GetComponent<DogController>();
        }

        // �v���C���[��Animator���擾
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleMovement();       // �ړ�����
        HandleLeashTension();   // ���[�h�ɂ��������菈��
    }

    /// �v���C���[�̈ړ�����������
    void HandleMovement()
    {
        // �L�[�{�[�h���͂��擾�i-1, 0, 1�̂����ꂩ�j
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        Vector2 movement = new Vector2(moveX, moveY).normalized;

        float currentSpeed = speed;

        // �������񒣂��Ă���i���������Ă�j�Ƃ��A�v���C���[�̑��x�𗎂Ƃ�
        if (dogController != null && dogController.IsPulled())
        {
            currentSpeed *= 0.5f; // �ړ����x�𔼕���
        }

        // ���ۂɈړ�
        transform.Translate(movement * currentSpeed * Time.deltaTime);

        // �A�j���[�V��������i������Idle������ݒ�j
        if (animator != null)
        {
            animator.SetFloat("MoveX", movement.x);
            animator.SetFloat("MoveY", movement.y);
            animator.SetFloat("Speed", movement.magnitude);

            // �����ł������Ă���Ȃ�Ō�̈ړ��������L��
            if (movement.magnitude > 0.01f)
            {
                lastMoveDir = movement;
                animator.SetFloat("LastMoveX", lastMoveDir.x);
                animator.SetFloat("LastMoveY", lastMoveDir.y);
            }
        }
    }

    /// ���[�h�������Ă����Ԃ��ǂ������m�F���A������������
    void HandleLeashTension()
    {
        // �������݂��Ȃ���Ή������Ȃ�
        if (dog == null || dogController == null) return;

        float distance = Vector2.Distance(transform.position, dog.position); // �v���C���[�ƌ��̋���
        Vector2 toPlayer = (Vector2)transform.position - (Vector2)dog.position; // ������v���C���[�ւ̕���
        Vector2 movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); // �v���C���[�̓��̓x�N�g��

        // ���[�h�̍ő勗���𒴂��Ă��邩�`�F�b�N
        if (distance > leashMaxLength)
        {
            float dot = Vector2.Dot(toPlayer.normalized, movementInput.normalized); // �ړ������������猩�ăv���C���[�����ƈ�v���Ă��邩�H

            // �v���C���[���u�������������Ă�������ɓ����Ă���v�������Ă���Ƃ�
            if (dot > 0.1f && movementInput.magnitude > 0.1f)
            {
                // �����v���C���[�����ɏ�������������
                dog.position += (Vector3)(toPlayer.normalized * pullSpeed * Time.deltaTime);

                // �����u���������Ă���v�A�j���[�V������Ԃɓ���
                dogController.SetPulledState(true);
            }
            else
            {
                // �v���C���[�̓������ア�E�������Ⴄ�ꍇ�͓��񒣂����
                dogController.SetPulledState(false);
            }
        }
        else
        {
            // �������\���߂��̂œ��񒣂����
            dogController.SetPulledState(false);
        }
    }
}
