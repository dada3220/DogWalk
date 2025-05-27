using UnityEngine;

public class DogController : MonoBehaviour
{
    public float speed; // 通常の移動速度
    public Vector2 targetPosition; // 犬の目的地
    public float changeTargetTime = 3f; // 移動先変更の間隔
    public float timer = 0f;

    public Transform player; // プレイヤーの位置参照
    public float wanderRadius; // プレイヤー周囲の移動範囲

    public float affectionCheckInterval; // 好感度チェック間隔
    private float affectionTimer = 0;
    public float maxDistance; // この距離を超えると「引っ張り」アニメ再生
    public float minDistanceForBoost; // 近くにいると好感度が上がる距離

    public GameObject poopPrefab; // うんちのプレハブ
    public float poopInterval; // うんち生成間隔
    private float poopTimer = 0f;

    public float leashMaxLength; // リードの最大距離（これ以上で強制追従）
    public float pullAffectionLossRate; // 引っ張られている時の好感度減少量

    private Animator animator;
    private bool isPulled = false; // 引っ張り状態フラグ

    void Start()
    {
        SetNewTargetPosition(); // 初回の移動先設定
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        affectionTimer += Time.deltaTime;
        poopTimer += Time.deltaTime;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        Vector2 previousPosition = transform.position;

        // プレイヤーからリード以上に離れたら強制追従
        if (distanceToPlayer > leashMaxLength)
        {
            Vector2 pullDir = (player.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            SetPulledState(true, pullDir);
        }
        else if (distanceToPlayer > maxDistance)
        {
            Vector2 pullDir = (player.position - transform.position).normalized;
            SetPulledState(true, pullDir);
        }
        else
        {
            // 通常のランダム移動
            if (timer >= changeTargetTime)
            {
                SetNewTargetPosition();
                timer = 0f;
            }

            Vector2 moveDir = targetPosition - (Vector2)transform.position;

            // 移動方向をXまたはYに限定（斜め回避）
            if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.y)) moveDir.y = 0;
            else moveDir.x = 0;

            moveDir.Normalize();
            transform.position = (Vector2)transform.position + moveDir * speed * Time.deltaTime;

            SetPulledState(false);
        }

        // 移動アニメーションの更新（引っ張りでないとき）
        if (!IsPulled())
        {
            Vector2 moveVector = ((Vector2)transform.position - previousPosition);
            float moveSpeed = moveVector.magnitude / Time.deltaTime;
            Vector2 moveDirNormalized = moveVector.normalized;

            if (animator != null)
            {
                animator.SetFloat("moveX", moveDirNormalized.x);
                animator.SetFloat("moveY", moveDirNormalized.y);
                animator.SetFloat("speed", moveSpeed);
            }
        }

        // 好感度の増減チェック
        if (affectionTimer >= affectionCheckInterval)
        {
            if (distanceToPlayer > minDistanceForBoost)
            {
                AffinityManager.Instance?.DecreaseAffection(4); // 離れすぎで減少
            }
            else
            {
                AffinityManager.Instance?.IncreaseAffection(6); // 近くで増加
            }
            affectionTimer = 0f;
        }

        // 引っ張り中は好感度が徐々に減少
        if (IsPulled())
        {
            AffinityManager.Instance?.DecreaseAffection(Mathf.RoundToInt(pullAffectionLossRate * Time.deltaTime));
        }

        // うんちを出す処理
        if (poopTimer >= poopInterval)
        {
            SpawnPoop();
            poopTimer = 0f;
        }
    }

    // プレイヤーの近くでランダムに目的地を再設定
    void SetNewTargetPosition()
    {
        Vector2 offset = (Random.value > 0.5f)
            ? new Vector2(Random.Range(-wanderRadius, wanderRadius), 0)
            : new Vector2(0, Random.Range(-wanderRadius, wanderRadius));

        targetPosition = (Vector2)player.position + offset;
    }

    // うんちを出す（10秒で消える）
    void SpawnPoop()
    {
        GameObject poop = Instantiate(poopPrefab, transform.position, Quaternion.identity);
        Destroy(poop, 10f);
    }

    // プレイヤーにぶつかった時の好感度減少
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            AffinityManager.Instance?.DecreaseAffection(10);
        }
    }

    // 引っ張り状態のセット（アニメーター制御）
    public void SetPulledState(bool pulled, Vector2 pullDirection = default)
    {
        if (animator == null) return;

        if (pulled)
        {
            isPulled = true;

            // トリガーリセット
            animator.ResetTrigger("pull_l");
            animator.ResetTrigger("pull_r");
            animator.ResetTrigger("pull_b");
            animator.ResetTrigger("pull_s");

            // 方向に応じたトリガーセット
            if (Mathf.Abs(pullDirection.x) > Mathf.Abs(pullDirection.y))
            {
                animator.SetTrigger(pullDirection.x > 0 ? "pull_r" : "pull_l");
            }
            else
            {
                animator.SetTrigger(pullDirection.y > 0 ? "pull_b" : "pull_s");
            }

            // スプライトの向き
            if (Mathf.Abs(pullDirection.x) > 0.5f)
                GetComponent<SpriteRenderer>().flipX = (pullDirection.x < 0);
        }
        else
        {
            isPulled = false;
        }
    }

    // 引っ張られているか判定
    public bool IsPulled() => isPulled;
}
