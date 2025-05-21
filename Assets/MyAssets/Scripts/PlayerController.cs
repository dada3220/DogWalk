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
        if (dog != null)
        {
            dogController = dog.GetComponent<DogController>();
        }

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleMovement();       // �ړ�����
        HandleLeashTension();   // ���[�h�ɂ��������菈��
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        Vector2 movement = new Vector2(moveX, moveY).normalized;

        float currentSpeed = speed;

        if (dogController != null && dogController.IsPulled())
        {
            currentSpeed *= 0.5f;
        }

        transform.Translate(movement * currentSpeed * Time.deltaTime);

        // �A�j���[�V��������
        if (animator != null)
        {
            if (movement.magnitude > 0.01f)
            {
                lastMoveDir = movement; // �ړ�������ꍇ�ɍX�V

                if (movement.x > 0)
                {
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Player_r"))
                        animator.Play("Player_r");
                }
                else if (movement.x < 0)
                {
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Player_l"))
                        animator.Play("Player_l");
                }
                else if (movement.y < 0)
                {
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Player_s"))
                        animator.Play("Player_s");
                }
                else if (movement.y > 0)
                {
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Player_b"))
                        animator.Play("Player_b");
                }
            }
            else
            {
                // Idle���A�Ō�Ɍ����Ă��������̑ҋ@�A�j���[�V�����Đ�
                if (lastMoveDir.x > 0)
                {
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Player_rF"))
                        animator.Play("Player_rF");
                }
                else if (lastMoveDir.x < 0)
                {
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Player_lF"))
                        animator.Play("Player_lF");
                }
                else if (lastMoveDir.y < 0)
                {
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Player_sF"))
                        animator.Play("Player_sF");
                }
                else if (lastMoveDir.y > 0)
                {
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Player_bF"))
                        animator.Play("Player_bF");
                }
            }

            // �A�j���[�V�����p�����[�^�̍X�V
            animator.SetFloat("MoveX", movement.x);
            animator.SetFloat("MoveY", movement.y);
            animator.SetFloat("Speed", movement.magnitude);
            animator.SetFloat("LastMoveX", lastMoveDir.x);
            animator.SetFloat("LastMoveY", lastMoveDir.y);
        }
    }

    void HandleLeashTension()
    {
        if (dog == null || dogController == null) return;

        float distance = Vector2.Distance(transform.position, dog.position);
        Vector2 toPlayer = (Vector2)transform.position - (Vector2)dog.position;
        Vector2 movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (distance > leashMaxLength)
        {
            float dot = Vector2.Dot(toPlayer.normalized, movementInput.normalized);

            if (dot > 0.1f && movementInput.magnitude > 0.1f)
            {
                dog.position += (Vector3)(toPlayer.normalized * pullSpeed * Time.deltaTime);
                dogController.SetPulledState(true);
            }
            else
            {
                dogController.SetPulledState(false);
            }
        }
        else
        {
            dogController.SetPulledState(false);
        }
    }
}
