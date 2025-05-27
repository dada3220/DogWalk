using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float normalSpeed; // �ʏ�̈ړ����x
    private float currentSpeed; // ���ۂ̈ړ����x�i�������莞�ɕϓ��j

    private Animator animator; // �A�j���[�^�[�Q��
    private Vector2 lastMoveDir = Vector2.down; // �ŏI�I�Ȉړ������i��~���A�j���p�j

    public DogController dog; // ���̃X�N���v�g�Q�ƁiInspector�Ŋ��蓖�āj

    private Vector3 lastPosition; // �O�t���[���̈ʒu�i�ړ������v���p�j
    public ScoreManager scoreManager; // �X�R�A�Ǘ��X�N���v�g�ւ̎Q�ƁiInspector�Őݒ�j

    private float walkedDistance = 0f; // �ݐψړ�����
    public float distancePerScore = 0.5f; // ���̋������ƂɃX�R�A+1

    void Start()
    {
        animator = GetComponent<Animator>();
        currentSpeed = normalSpeed;
        lastPosition = transform.position; // �����ʒu�L�^
    }

    void Update()
    {
        // �������������Ă�����ړ����x�𐧌�
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

        // �ړ����s
        transform.Translate(movement * currentSpeed * Time.deltaTime);

        // �ړ������v���ƃX�R�A���Z
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        walkedDistance += distanceMoved;

        if (walkedDistance >= distancePerScore)
        {
            int points = Mathf.FloorToInt(walkedDistance / distancePerScore); // ���_���Z���邩
            walkedDistance -= points * distancePerScore; // �c�������J��z��
            scoreManager?.AddScore(points); // �X�R�A���Z�inull�`�F�b�N�t���j
        }

        lastPosition = transform.position; // �ʒu�X�V

        // �A�j���[�V��������
        if (animator != null)
        {
            if (movement.magnitude > 0.01f)
            {
                lastMoveDir = movement;

                // �ړ��A�j���[�V�����Đ�
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
                // ��~���͍Ō�̕����őҋ@�A�j���Đ�
                if (lastMoveDir.x > 0)
                    animator.Play("Player_rF");
                else if (lastMoveDir.x < 0)
                    animator.Play("Player_lF");
                else if (lastMoveDir.y < 0)
                    animator.Play("Player_sF");
                else if (lastMoveDir.y > 0)
                    animator.Play("Player_bF");
            }

            // �A�j���[�^�[�ɕ����Ƒ��x��n��
            animator.SetFloat("MoveX", movement.x);
            animator.SetFloat("MoveY", movement.y);
            animator.SetFloat("Speed", movement.magnitude);
            animator.SetFloat("LastMoveX", lastMoveDir.x);
            animator.SetFloat("LastMoveY", lastMoveDir.y);
        }
    }
}
