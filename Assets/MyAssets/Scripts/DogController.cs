using UnityEngine;

/// <summary>
/// 犬の行動とアニメーションを制御するクラス
/// 機能:
/// - プレイヤーの周囲をランダムに徘徊
/// - 黄色い木へのマーキング
/// - うんち（PoopSpawner）生成
/// - プレイヤーとの距離によって好感度を増減
/// - 引っ張り状態の処理（好感度減少）
/// - アニメーション（idle / walk / pull / dogmarking）
/// </summary>
public class DogController : MonoBehaviour
{
    // ======= Inspector で設定するパラメータ =======

    public float speed;                      // 移動速度
    public Vector2 targetPosition;           // 現在の移動先ターゲット（ランダム徘徊用）
    public float changeTargetTime = 3f;      // ランダム目的地の更新間隔
    public Transform player;                 // プレイヤーの Transform
    public float wanderRadius;               // プレイヤー中心に徘徊する半径

    public float affectionCheckInterval;     // 好感度の増減を行う間隔
    public float maxDistance;                // 木に行くときにプレイヤーから離れていい距離の限界
    public float minDistanceForBoost;        // プレイヤーとの距離がこの範囲以内で好感度が上がる

    public float poopInterval;               // うんちを出す間隔
    public float leashMaxLength;             // リードの長さ制限（使用予定？）
    public float pullAffectionLossRate;      // 引っ張られているときの好感度減少量（毎秒）

    public PoopSpawner poopSpawner;          // PoopSpawner コンポーネントへの参照

    // ======= 内部で使用する状態管理変数 =======

    private float timer = 0f;                // ランダム徘徊用のタイマー
    private float affectionTimer = 0f;       // 好感度処理タイマー
    private float poopTimer = 0f;            // うんち生成用タイマー

    private Animator animator;               // Animator コンポーネントへの参照
    private bool isPulled = false;           // リードで引っ張られているか
    private bool isMarking = false;          // 木にマーキング中か

    private float markingTimer = 0f;         // マーキング時間（2秒）
    private YellowTree currentTree = null;   // 現在目指している木
    private bool isGoingToTree = false;      // 木に向かっている途中か

    private Vector2 lastMoveDir = Vector2.down; // 最後の移動方向（停止中の向き保持用）

    void Start()
    {
        SetNewTargetPosition();              // 初期目的地をランダム設定
        animator = GetComponent<Animator>(); // Animator を取得
    }

    void Update()
    {
        // === マーキング中はアニメーション再生だけで他は停止 ===
        if (isMarking)
        {
            markingTimer -= Time.deltaTime;
            if (markingTimer <= 0f)
            {
                isMarking = false;
                currentTree?.OnDogArrived();     // 木にマーキング完了通知
                currentTree = null;
                timer = changeTargetTime;        // 次の徘徊のためタイマー初期化
                isGoingToTree = false;
            }
            return; // マーキング中は他の処理スキップ
        }

        // === タイマー更新 ===
        timer += Time.deltaTime;
        affectionTimer += Time.deltaTime;
        poopTimer += Time.deltaTime;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        Vector2 previousPosition = transform.position;

        // === 木へ向かう処理 ===
        if (isGoingToTree && currentTree != null)
        {
            Vector2 treeTarget = currentTree.transform.position;
            float distPlayerToTree = Vector2.Distance(player.position, treeTarget);

            // 木へ向かうとプレイヤーから離れすぎる → 引っ張り状態に
            if (distPlayerToTree > maxDistance)
            {
                SetPulledState(true, (player.position - transform.position).normalized);
                return;
            }

            // 木へ向かって移動
            Vector2 moveDir = (treeTarget - (Vector2)transform.position).normalized;
            transform.position += (Vector3)(moveDir * speed * Time.deltaTime);

            if (Vector2.Distance(transform.position, treeTarget) < 0.3f)
            {
                StartMarking(currentTree); // 木に到着したらマーキング開始
            }
        }

        // === ランダム徘徊 ===
        else if (!IsPulled())
        {
            if (timer >= changeTargetTime)
            {
                SetNewTargetPosition(); // プレイヤー中心に新しい目的地を設定
                timer = 0f;
            }

            Vector2 moveDir = (targetPosition - (Vector2)transform.position).normalized;
            transform.position += (Vector3)(moveDir * speed * Time.deltaTime);
            lastMoveDir = moveDir; // 最後の移動方向を保持
        }

        // === アニメーション更新 ===
        UpdateAnimation(previousPosition);

        // === 好感度チェック ===
        if (affectionTimer >= affectionCheckInterval)
        {
            if (distanceToPlayer > minDistanceForBoost)
                AffinityManager.Instance?.DecreaseAffection(4); // 離れすぎで減少
            else
                AffinityManager.Instance?.IncreaseAffection(6); // 近くにいて上昇

            affectionTimer = 0f;
        }

        // === 引っ張られているときは好感度毎フレーム減少 ===
        if (IsPulled())
        {
            AffinityManager.Instance?.DecreaseAffection(Mathf.RoundToInt(pullAffectionLossRate * Time.deltaTime));
        }

        // === うんち生成 ===
        if (poopTimer >= poopInterval)
        {
            poopSpawner?.SpawnPoop(transform.position); // 現在位置にうんち生成
            poopTimer = 0f;
        }
    }

    /// <summary>
    /// アニメーションの状態を移動量から判断して設定
    /// </summary>
    private void UpdateAnimation(Vector2 previousPosition)
    {
        Vector2 moveVector = (Vector2)transform.position - previousPosition;
        float moveSpeed = moveVector.magnitude / Time.deltaTime;

        if (animator == null) return;

        if (IsPulled())
        {
            SetAnimationState("pull", moveVector); // 引っ張りアニメ
        }
        else if (isMarking)
        {
            animator.SetTrigger("dogmarking");     // マーキングアニメ（単発）
        }
        else if (moveSpeed < 0.01f)
        {
            SetAnimationState("idle", lastMoveDir); // 停止アニメ（向きは最後の方向）
        }
        else
        {
            lastMoveDir = moveVector;
            SetAnimationState("walk", moveVector);  // 通常移動アニメ
        }
    }

    /// <summary>
    /// アニメーションを「状態_方向」で再生する
    /// 例: walk_front, pull_left
    /// </summary>
    private void SetAnimationState(string state, Vector2 dir)
    {
        if (dir == Vector2.zero) dir = Vector2.down; // 無方向時は下向きとする

        string direction;
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            direction = dir.x > 0 ? "right" : "left";
        else
            direction = dir.y > 0 ? "back" : "front";

        animator.Play($"{state}_{direction}");

        // 左向き時はスプライトを左右反転（pull中は反転しない）
        GetComponent<SpriteRenderer>().flipX = (state != "pull" && direction == "left");
    }

    /// <summary>
    /// 木に向かって移動開始（YellowTreeから呼び出される）
    /// </summary>
    public void GoToTarget(Vector2 position, YellowTree tree)
    {
        targetPosition = position;
        currentTree = tree;
        isGoingToTree = true;
    }

    /// <summary>
    /// プレイヤー中心に新しいランダムな目的地を設定
    /// </summary>
    void SetNewTargetPosition()
    {
        Vector2 offset = new Vector2(
            Random.Range(-wanderRadius, wanderRadius),
            Random.Range(-wanderRadius, wanderRadius)
        );
        targetPosition = (Vector2)player.position + offset;
    }

    /// <summary>
    /// プレイヤーと衝突時、好感度ダウン
    /// </summary>
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            AffinityManager.Instance?.DecreaseAffection(10);
        }
    }

    /// <summary>
    /// 引っ張られているかどうかの状態を切り替える
    /// </summary>
    public void SetPulledState(bool pulled, Vector2 pullDirection = default)
    {
        isPulled = pulled;

        if (!pulled && animator != null)
        {
            // 必要であれば引っ張りアニメのトリガーをリセット
            animator.ResetTrigger("pull_l");
            animator.ResetTrigger("pull_r");
            animator.ResetTrigger("pull_b");
            animator.ResetTrigger("pull_s");
        }
    }

    /// <summary>
    /// マーキング状態に入り、2秒間アニメ再生
    /// </summary>
    private void StartMarking(YellowTree tree)
    {
        isMarking = true;
        markingTimer = 2f;
        currentTree = tree;
        animator?.SetTrigger("dogmarking");
    }

    /// <summary>
    /// 引っ張り状態かどうかを返す
    /// </summary>
    public bool IsPulled()
    {
        return isPulled;
    }
}
