using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float normalSpeed;           // 通常の移動速度
    private float currentSpeed;         // 実際の移動速度（犬に引っ張られている時に変化）

    private Animator animator;          // Animator コンポーネント
    private Vector2 lastMoveDir = Vector2.down; // 最後に移動した方向（停止アニメ用）

    public DogController dog;           // DogController（Inspectorで設定）
    private Vector3 lastPosition;       // 前フレームの位置
    public ScoreManager scoreManager;   // ScoreManager（Inspectorで設定）

    private float walkedDistance = 0f;  // 累積移動距離
    public float distancePerScore = 0.5f; // 指定距離ごとにスコア加算

    [SerializeField] private GameManager gameManager; // GameManager（Inspectorで設定）

    void Start()
    {
        animator = GetComponent<Animator>();
        currentSpeed = normalSpeed;
        lastPosition = transform.position;
    }

    void Update()
    {
        // ゲームがまだ開始していない場合は、処理を止める
        if (!gameManager.IsPlaying) return;

        // 移動速度調整（犬に引っ張られていたら減速）
        if (dog != null && dog.IsPulled())
        {
            currentSpeed = 1f;
        }
        else
        {
            currentSpeed = normalSpeed;
        }

        // 入力取得
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        Vector2 movement = new Vector2(moveX, moveY).normalized;

        // 移動処理
        transform.Translate(movement * currentSpeed * Time.deltaTime);

        // 移動距離からスコア加算
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        walkedDistance += distanceMoved;

        if (walkedDistance >= distancePerScore)
        {
            int points = Mathf.FloorToInt(walkedDistance / distancePerScore);
            walkedDistance -= points * distancePerScore;
            scoreManager?.AddScore(points);
        }

        lastPosition = transform.position;

        // アニメーション制御
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
