using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f; // プレイヤーの基本移動速度

    public Transform dog; // DogControllerに接続
    public float pullSpeed = 2f; // 犬が引っ張られるときの移動速度
    public float leashMaxLength = 5f; // リードの最大長さ

    private DogController dogController; // DogControllerの参照
    private Animator animator; // プレイヤーのAnimator（アニメーション制御用）
    private Vector2 lastMoveDir = Vector2.down; // 最後の移動方向（Idle時に向きを維持）

    void Start()
    {
        // 犬が設定されていれば、DogControllerを取得
        if (dog != null)
        {
            dogController = dog.GetComponent<DogController>();
        }

        // プレイヤーのAnimatorを取得
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleMovement();       // 移動処理
        HandleLeashTension();   // リードによる引っ張り処理
    }

    /// プレイヤーの移動を処理する
    void HandleMovement()
    {
        // キーボード入力を取得（-1, 0, 1のいずれか）
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        Vector2 movement = new Vector2(moveX, moveY).normalized;

        float currentSpeed = speed;

        // 犬が踏ん張っている（引っ張られてる）とき、プレイヤーの速度を落とす
        if (dogController != null && dogController.IsPulled())
        {
            currentSpeed *= 0.5f; // 移動速度を半分に
        }

        // 実際に移動
        transform.Translate(movement * currentSpeed * Time.deltaTime);

        // アニメーション制御（方向やIdle向きを設定）
        if (animator != null)
        {
            animator.SetFloat("MoveX", movement.x);
            animator.SetFloat("MoveY", movement.y);
            animator.SetFloat("Speed", movement.magnitude);

            // 少しでも動いているなら最後の移動方向を記憶
            if (movement.magnitude > 0.01f)
            {
                lastMoveDir = movement;
                animator.SetFloat("LastMoveX", lastMoveDir.x);
                animator.SetFloat("LastMoveY", lastMoveDir.y);
            }
        }
    }

    /// リードが張っている状態かどうかを確認し、犬を引っ張る
    void HandleLeashTension()
    {
        // 犬が存在しなければ何もしない
        if (dog == null || dogController == null) return;

        float distance = Vector2.Distance(transform.position, dog.position); // プレイヤーと犬の距離
        Vector2 toPlayer = (Vector2)transform.position - (Vector2)dog.position; // 犬からプレイヤーへの方向
        Vector2 movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); // プレイヤーの入力ベクトル

        // リードの最大距離を超えているかチェック
        if (distance > leashMaxLength)
        {
            float dot = Vector2.Dot(toPlayer.normalized, movementInput.normalized); // 移動方向が犬から見てプレイヤー方向と一致しているか？

            // プレイヤーが「犬を引っ張っている方向に動いている」かつ動いているとき
            if (dot > 0.1f && movementInput.magnitude > 0.1f)
            {
                // 犬をプレイヤー方向に少しずつ引っ張る
                dog.position += (Vector3)(toPlayer.normalized * pullSpeed * Time.deltaTime);

                // 犬が「引っ張られている」アニメーション状態に入る
                dogController.SetPulledState(true);
            }
            else
            {
                // プレイヤーの動きが弱い・向きが違う場合は踏ん張り解除
                dogController.SetPulledState(false);
            }
        }
        else
        {
            // 距離が十分近いので踏ん張り解除
            dogController.SetPulledState(false);
        }
    }
}
