using UnityEngine;

/// <summary>
/// 犬の移動・アニメーション制御を行うクラス
/// ・ランダム徘徊やプレイヤー追従
/// ・黄色い木へ向かいマーキングアニメーション再生
/// ・PoopSpawnerによるうんち生成
/// </summary>
public class DogController : MonoBehaviour
{
    // ======= 公開パラメータ（インスペクターで設定） =======

    public float speed;                      // 犬の移動速度
    public Vector2 targetPosition;           // ランダム移動の目標位置
    public float changeTargetTime = 3f;      // 目的地更新間隔
    public Transform player;                 // プレイヤーTransform
    public float wanderRadius;               // プレイヤー周辺での徘徊半径

    public float affectionCheckInterval;     // 好感度チェック間隔
    public float maxDistance;                // プレイヤーからの最大距離（引っ張りアニメ）
    public float minDistanceForBoost;        // 近距離で好感度上昇距離

    public float poopInterval;               // うんち生成間隔

    public float leashMaxLength;             // リードの最大長
    public float pullAffectionLossRate;      // 引っ張り時の好感度減少速度

    public PoopSpawner poopSpawner;          // PoopSpawner の参照（新規）

    // ======= 内部状態管理 =======

    private float timer = 0f;                // 目的地更新タイマー
    private float affectionTimer = 0f;       // 好感度チェックタイマー
    private float poopTimer = 0f;            // うんち生成タイマー

    private Animator animator;               // Animatorコンポーネント
    private bool isPulled = false;           // 引っ張られているか
    private bool isMarking = false;          // マーキングアニメーション中か

    private float markingTimer = 0f;         // マーキング用タイマー
    private YellowTree currentTree = null;   // マーキング対象の木
    private bool isGoingToTree = false;      // 木へ向かっているか

    void Start()
    {
        SetNewTargetPosition();              // 初期の目的地設定
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // マーキング中は移動停止＆アニメーション再生のみ
        if (isMarking)
        {
            markingTimer -= Time.deltaTime;
            if (markingTimer <= 0f)
            {
                isMarking = false;
                currentTree?.OnDogArrived(); // 木にマーキング完了通知
                currentTree = null;
                timer = changeTargetTime;    // ランダム徘徊に戻る
                isGoingToTree = false;
            }
            return;
        }

        // タイマー更新
        timer += Time.deltaTime;
        affectionTimer += Time.deltaTime;
        poopTimer += Time.deltaTime;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        Vector2 previousPosition = transform.position;

        // 木に向かう処理
        if (isGoingToTree && currentTree != null)
        {
            Vector2 treeTarget = currentTree.transform.position;

            // プレイヤーから木への距離をチェック
            float distPlayerToTree = Vector2.Distance(player.position, treeTarget);

            // 犬が木に近づくとプレイヤーからmaxDistance以上になる場合、近づかない
            if (distPlayerToTree > maxDistance)
            {
                // プレイヤーから離れすぎるので待機（orランダム徘徊に戻るなど）
                SetPulledState(true, (player.position - transform.position).normalized);
                return;
            }

            Vector2 moveDir = (treeTarget - (Vector2)transform.position).normalized;
            transform.position = (Vector2)transform.position + moveDir * speed * Time.deltaTime;

            if (Vector2.Distance(transform.position, treeTarget) < 0.3f)
            {
                StartMarking(currentTree);
            }
        }


        // アニメーション更新
        if (!IsPulled() && !isMarking)
        {
            Vector2 moveVector = (Vector2)transform.position - previousPosition;
            float moveSpeed = moveVector.magnitude / Time.deltaTime;

            if (animator != null)
            {
                animator.SetFloat("moveX", moveVector.normalized.x);
                animator.SetFloat("moveY", moveVector.normalized.y);
                animator.SetFloat("speed", moveSpeed);
            }
        }

        // 好感度処理
        if (affectionTimer >= affectionCheckInterval)
        {
            if (distanceToPlayer > minDistanceForBoost)
            {
                AffinityManager.Instance?.DecreaseAffection(4);
            }
            else
            {
                AffinityManager.Instance?.IncreaseAffection(6);
            }
            affectionTimer = 0f;
        }

        // 引っ張り中の好感度減少
        if (IsPulled())
        {
            AffinityManager.Instance?.DecreaseAffection(Mathf.RoundToInt(pullAffectionLossRate * Time.deltaTime));
        }

        // うんち処理（PoopSpawner を使って生成）
        if (poopTimer >= poopInterval)
        {
            if (poopSpawner != null)
            {
                poopSpawner.SpawnPoop(transform.position);
            }
            poopTimer = 0f;
        }
    }

    public void GoToTarget(Vector2 position, YellowTree tree)
    {
        targetPosition = position;
        currentTree = tree;
        isGoingToTree = true;
    }

    void SetNewTargetPosition()
    {
        Vector2 offset = new Vector2(
            Random.Range(-wanderRadius, wanderRadius),
            Random.Range(-wanderRadius, wanderRadius)
        );
        targetPosition = (Vector2)player.position + offset;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            AffinityManager.Instance?.DecreaseAffection(10);
        }
    }

    public void SetPulledState(bool pulled, Vector2 pullDirection = default)
    {
        if (animator == null) return;

        if (pulled)
        {
            isPulled = true;

            animator.ResetTrigger("pull_l");
            animator.ResetTrigger("pull_r");
            animator.ResetTrigger("pull_b");
            animator.ResetTrigger("pull_s");

            if (Mathf.Abs(pullDirection.x) > Mathf.Abs(pullDirection.y))
            {
                animator.SetTrigger(pullDirection.x > 0 ? "pull_r" : "pull_l");
            }
            else
            {
                animator.SetTrigger(pullDirection.y > 0 ? "pull_b" : "pull_s");
            }

            animator.SetFloat("moveX", pullDirection.normalized.x);
            animator.SetFloat("moveY", pullDirection.normalized.y);
            animator.SetFloat("speed", speed);

            if (Mathf.Abs(pullDirection.x) > 0.5f)
                GetComponent<SpriteRenderer>().flipX = (pullDirection.x < 0);
        }
        else
        {
            isPulled = false;
        }
    }

    private void StartMarking(YellowTree tree)
    {
        isMarking = true;
        markingTimer = 2f;
        currentTree = tree;

        if (animator != null)
        {
            animator.SetTrigger("dogmarking");
        }
    }

    public bool IsPulled()
    {
        return isPulled;
    }
}
