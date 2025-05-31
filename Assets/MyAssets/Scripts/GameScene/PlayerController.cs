using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float normalSpeed; // 通常の移動速度
    private float currentSpeed; // 実際の移動速度（引っ張り時に変動）

    private Animator animator; // アニメーター参照
    private Vector2 lastMoveDir = Vector2.down; // 最終的な移動方向（停止中アニメ用）

    public DogController dog; // 犬のスクリプト参照（Inspectorで割り当て）

    private Vector3 lastPosition; // 前フレームの位置（移動距離計測用）
    public ScoreManager scoreManager; // スコア管理スクリプトへの参照（Inspectorで設定）

    private float walkedDistance = 0f; // 累積移動距離
    public float distancePerScore = 0.5f; // この距離ごとにスコア+1

    void Start()
    {
        animator = GetComponent<Animator>();
        currentSpeed = normalSpeed;
        lastPosition = transform.position; // 初期位置記録
    }

    void Update()
    {
        // 犬が引っ張られていたら移動速度を制限
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

        // 移動実行
        transform.Translate(movement * currentSpeed * Time.deltaTime);

        // 移動距離計測とスコア加算
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        walkedDistance += distanceMoved;

        if (walkedDistance >= distancePerScore)
        {
            int points = Mathf.FloorToInt(walkedDistance / distancePerScore); // 何点加算するか
            walkedDistance -= points * distancePerScore; // 残距離を繰り越す
            scoreManager?.AddScore(points); // スコア加算（nullチェック付き）
        }

        lastPosition = transform.position; // 位置更新

        // アニメーション制御
        if (animator != null)
        {
            if (movement.magnitude > 0.01f)
            {
                lastMoveDir = movement;

                // 移動アニメーション再生
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
                // 停止時は最後の方向で待機アニメ再生
                if (lastMoveDir.x > 0)
                    animator.Play("Player_rF");
                else if (lastMoveDir.x < 0)
                    animator.Play("Player_lF");
                else if (lastMoveDir.y < 0)
                    animator.Play("Player_sF");
                else if (lastMoveDir.y > 0)
                    animator.Play("Player_bF");
            }

            // アニメーターに方向と速度を渡す
            animator.SetFloat("MoveX", movement.x);
            animator.SetFloat("MoveY", movement.y);
            animator.SetFloat("Speed", movement.magnitude);
            animator.SetFloat("LastMoveX", lastMoveDir.x);
            animator.SetFloat("LastMoveY", lastMoveDir.y);
        }
    }
}
