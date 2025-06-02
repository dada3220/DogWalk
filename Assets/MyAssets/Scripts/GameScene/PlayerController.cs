using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float normalSpeed;           // �ʏ�̈ړ����x
    private float currentSpeed;         // ���ۂ̈ړ����x�i���Ɉ��������Ă��鎞�ɕω��j

    private Animator animator;          // Animator �R���|�[�l���g
    private Vector2 lastMoveDir = Vector2.down; // �Ō�Ɉړ����������i��~�A�j���p�j

    public DogController dog;           // DogController�iInspector�Őݒ�j
    private Vector3 lastPosition;       // �O�t���[���̈ʒu
    public ScoreManager scoreManager;   // ScoreManager�iInspector�Őݒ�j

    private float walkedDistance = 0f;  // �ݐψړ�����
    public float distancePerScore = 0.5f; // �w�苗�����ƂɃX�R�A���Z

    [SerializeField] private GameManager gameManager; // GameManager�iInspector�Őݒ�j

    void Start()
    {
        animator = GetComponent<Animator>();
        currentSpeed = normalSpeed;
        lastPosition = transform.position;
    }

    void Update()
    {
        // �Q�[�����܂��J�n���Ă��Ȃ��ꍇ�́A�������~�߂�
        if (!gameManager.IsPlaying) return;

        // �ړ����x�����i���Ɉ��������Ă����猸���j
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

        // �ړ�����
        transform.Translate(movement * currentSpeed * Time.deltaTime);

        // �ړ���������X�R�A���Z
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        walkedDistance += distanceMoved;

        if (walkedDistance >= distancePerScore)
        {
            int points = Mathf.FloorToInt(walkedDistance / distancePerScore);
            walkedDistance -= points * distancePerScore;
            scoreManager?.AddScore(points);
        }

        lastPosition = transform.position;

        // �A�j���[�V��������
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

            animator.SetFloat("MoveX", movement.x);
            animator.SetFloat("MoveY", movement.y);
            animator.SetFloat("Speed", movement.magnitude);
            animator.SetFloat("LastMoveX", lastMoveDir.x);
            animator.SetFloat("LastMoveY", lastMoveDir.y);
        }
    }
}
