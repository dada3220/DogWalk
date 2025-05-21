using UnityEngine;
using UnityEngine.UI;

public class DogController : MonoBehaviour
{
    // ===== 移動関連 =====
    public float speed = 3f;                       // 犬の移動速度
    private Vector2 targetPosition;               // ランダムな目的地
    private float changeTargetTime = 1f;          // 次の目的地までの時間
    private float timer = 0f;                     // タイマー

    public Transform player;                      // プレイヤーのTransform
    public float wanderRadius = 5f;               // プレイヤーの周囲でランダムに移動する範囲

    // ===== 好感度関連 =====
    public int affection = 100;                   // 好感度初期値
    public float affectionCheckInterval = 1f;     // 好感度変動の間隔
    private float affectionTimer = 0f;            // 好感度タイマー
    public float maxDistance = 5f;                // 離れすぎる距離
    public float minDistanceForBoost = 2f;        // 近くにいると好感度が上がる距離
    public Slider affectionSlider;                // 好感度表示スライダー

    // ===== うんこ関連 =====
    public GameObject poopPrefab;                 // うんこのPrefab
    public float poopInterval = 10f;              // うんこを出す間隔
    private float poopTimer = 0f;                 // うんこタイマー

    // ===== リード関連 =====
    public float leashMaxLength = 5f;             // リードの最大距離
    public float pullAffectionLossRate = 2f;      // 引っ張られ中の好感度減少速度

    // ===== アニメーション関連 =====
    private Animator animator;                    // Animatorコンポーネント参照

    void Start()
    {
        SetNewTargetPosition();

        // スライダーUI初期化
        if (affectionSlider != null)
        {
            affectionSlider.maxValue = 100;
            affectionSlider.value = affection;
        }

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 各タイマー更新
        timer += Time.deltaTime;
        affectionTimer += Time.deltaTime;
        poopTimer += Time.deltaTime;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position); // プレイヤーとの距離
        Vector2 previousPosition = transform.position; // 移動前の位置を保存

        // リードの範囲を超えたらプレイヤー方向に戻す
        if (distanceToPlayer > leashMaxLength)
        {
            Vector2 pullDir = (player.position - transform.position).normalized;
            Vector2 newTarget = player.position - (Vector3)(pullDir * leashMaxLength);
            transform.position = Vector2.MoveTowards(transform.position, newTarget, speed * Time.deltaTime);

            // 引っ張り状態ON、方向を渡す
            SetPulledState(true, pullDir);
        }
        else
        {
            // ランダム移動
            if (timer >= changeTargetTime)
            {
                SetNewTargetPosition();
                timer = 0f;
            }

            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // 引っ張り状態OFF
            SetPulledState(false);
        }

        // 移動方向（アニメーション用）
        Vector2 moveDir = ((Vector2)transform.position - previousPosition).normalized;
        float moveSpeed = ((Vector2)transform.position - previousPosition).magnitude / Time.deltaTime;

        if (animator != null)
        {
            animator.SetFloat("moveX", moveDir.x);
            animator.SetFloat("moveY", moveDir.y);
            animator.SetFloat("speed", moveSpeed);
        }

        // 一定時間ごとに距離に応じて好感度を上下
        if (affectionTimer >= affectionCheckInterval)
        {
            if (distanceToPlayer > minDistanceForBoost)
            {
                DecreaseAffection(4); // 離れすぎで減少
            }
            else
            {
                affection += 6; // 近くにいて増加
                affection = Mathf.Clamp(affection, 0, 100);
                if (affectionSlider != null) affectionSlider.value = affection;
            }

            affectionTimer = 0f;
        }

        // 引っ張られている間は持続的に好感度減少
        if (IsPulled())
        {
            DecreaseAffection(Mathf.RoundToInt(pullAffectionLossRate * Time.deltaTime));
        }

        // 一定時間でうんこを生成
        if (poopTimer >= poopInterval)
        {
            SpawnPoop();
            poopTimer = 0f;
        }
    }

    /// ランダムな移動先（プレイヤーの周囲）を設定
    void SetNewTargetPosition()
    {
        Vector2 randomOffset = Random.insideUnitCircle * wanderRadius;
        targetPosition = (Vector2)player.position + randomOffset;
    }

    /// うんこ生成（10秒後に自動削除）
    void SpawnPoop()
    {
        GameObject poop = Instantiate(poopPrefab, transform.position, Quaternion.identity);
        Destroy(poop, 10f);
    }

    /// 好感度を減少し、ゼロならゲームオーバー処理
    public void DecreaseAffection(int amount)
    {
        affection -= amount;
        affection = Mathf.Clamp(affection, 0, 100);
        if (affectionSlider != null) affectionSlider.value = affection;
        if (affection <= 0) GameOver();
    }

    /// ゲームオーバー処理（好感度ゼロ）
    void GameOver()
    {
        Debug.Log("Game Over! Dog lost trust.");
        // SceneManager.LoadScene("GameOverScene");
    }

    /// プレイヤーとぶつかったら好感度減少
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            DecreaseAffection(10);
        }
    }

    /// 引っ張られ状態をAnimatorに設定（方向も渡す）
    public void SetPulledState(bool isPulled, Vector2 pullDirection = default)
    {
        if (animator != null)
        {
            animator.SetBool("isPulled", isPulled);

            if (isPulled)
            {
                Vector2 dir = pullDirection.normalized;
                animator.SetFloat("pullX", dir.x);
                animator.SetFloat("pullY", dir.y);

                // 横向きの場合はSpriteを左右反転（必要に応じて）
                if (Mathf.Abs(dir.x) > 0.5f)
                {
                    GetComponent<SpriteRenderer>().flipX = (dir.x < 0);
                }
            }
        }
    }

    /// アニメーターの状態から引っ張られ中かどうか取得
    public bool IsPulled()
    {
        return animator != null && animator.GetBool("isPulled");
    }
}

