using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float normalSpeed = 4f; // �ʏ�̃v���C���[�ړ����x
    private float currentSpeed; // ���ۂ̈ړ����x�i�������莞�ɕύX�����j

    private Animator animator; // �A�j���[�V��������p
    private Vector2 lastMoveDir = Vector2.down; // �Ō�̈ړ�����

    public DogController dog; // ���̃X�N���v�g�Q�Ɓi�C���X�y�N�^�[�Ŋ��蓖�āj

    void Start()
    {
        animator = GetComponent<Animator>();
        currentSpeed = normalSpeed;
    }

    void Update()
    {
        // �������������Ă���ꍇ�A�v���C���[�̑��x��1f�ɐ���
        if (dog != null && dog.IsPulled())
        {
            currentSpeed = 1f;
        }
        else
        {
            currentSpeed = normalSpeed;
        }

        // ���͎擾
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(moveX, moveY).normalized;

        // �ړ�
        transform.Translate(movement * currentSpeed * Time.deltaTime);

        if (animator != null)
        {
            if (movement.magnitude > 0.01f)
            {
                lastMoveDir = movement;

                if (movement.x > 0)
                    animator.Play("Player_r");
                else if (movement.x < 0)
                    animator.Play("Player_l");
                else if (movement.y < 0)
                    animator.Play("Player_s");
                else if (movement.y > 0)
                    animator.Play("Player_b");
            }
            else
            {
                if (lastMoveDir.x > 0)
                    animator.Play("Player_rF");
                else if (lastMoveDir.x < 0)
                    animator.Play("Player_lF");
                else if (lastMoveDir.y < 0)
                    animator.Play("Player_sF");
                else if (lastMoveDir.y > 0)
                    animator.Play("Player_bF");
            }

            // �A�j���[�^�[�X�V
            animator.SetFloat("MoveX", movement.x);
            animator.SetFloat("MoveY", movement.y);
            animator.SetFloat("Speed", movement.magnitude);
            animator.SetFloat("LastMoveX", lastMoveDir.x);
            animator.SetFloat("LastMoveY", lastMoveDir.y);
        }
    }
}
