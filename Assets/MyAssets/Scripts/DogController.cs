using UnityEngine;

public class DogController : MonoBehaviour
{
    // ======= 公開パラメータ（インスペクターで設定） =======

    public float speed; // 犬の移動速度
    public Vector2 targetPosition; // ランダム移動の目標位置
    public float changeTargetTime = 3f; // 目標位置の更新間隔
    public Transform player; // プレイヤーのTransform参照
    public float wanderRadius; // プレイヤー周辺での移動半径

    public float affectionCheckInterval; // 好感度チェックの間隔
    public float maxDistance; // 引っ張りアニメーションを再生する距離
    public float minDistanceForBoost; // 近距離で好感度が上がる距離

    public GameObject poopPrefab; // うんちのプレハブ
    public float poopInterval; // うんちを出す間隔

    public float leashMaxLength; // リードの最大長（強制追従）
    public float pullAffectionLossRate; // 引っ張られている間の好感度減少速度

    // ======= 内部状態管理 =======
    private float timer = 0f; // 目標更新タイマー
    private float affectionTimer = 0f; // 好感度チェックタイマー
    private float poopTimer = 0f; // うんちタイマー

    private Animator animator; // Animatorコンポーネント
    private bool isPulled = false; // 引っ張られ状態フラグ

    void Start()
    {
        SetNewTargetPosition(); // 初期の目的地を設定
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // タイマーの更新
        timer += Time.deltaTime;
        affectionTimer += Time.deltaTime;
        poopTimer += Time.deltaTime;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        Vector2 previousPosition = transform.position;

        // ======= 移動制御 =======

        if (distanceToPlayer > leashMaxLength)
        {
            // リードを超えた → 強制追従
            Vector2 pullDir = (player.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            SetPulledState(true, pullDir);
        }
        else if (distanceToPlayer > maxDistance)
        {
            // プレイヤーと遠い → 引っ張りアニメーション
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

            // 移動方向ベクトルを正規化（斜め移動も許可）
            Vector2 moveDir = (targetPosition - (Vector2)transform.position).normalized;
            transform.position = (Vector2)transform.position + moveDir * speed * Time.deltaTime;

            SetPulledState(false);
        }

        // ======= アニメーション更新（移動時） =======
        if (!IsPulled())
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

        // ======= 好感度処理 =======
        if (affectionTimer >= affectionCheckInterval)
        {
            if (distanceToPlayer > minDistanceForBoost)
            {
                // 離れてると減少
                AffinityManager.Instance?.DecreaseAffection(4);
            }
            else
            {
                // 近くにいると増加
                AffinityManager.Instance?.IncreaseAffection(6);
            }
            affectionTimer = 0f;
        }

        // 引っ張り中の好感度減少
        if (IsPulled())
        {
            AffinityManager.Instance?.DecreaseAffection(Mathf.RoundToInt(pullAffectionLossRate * Time.deltaTime));
        }

        // ======= うんち処理 =======
        if (poopTimer >= poopInterval)
        {
            SpawnPoop();
            poopTimer = 0f;
        }
    }

    // ======= ランダムな目的地の再設定（プレイヤー周辺） =======
    void SetNewTargetPosition()
    {
        Vector2 offset = new Vector2(
            Random.Range(-wanderRadius, wanderRadius),
            Random.Range(-wanderRadius, wanderRadius)
        );
        targetPosition = (Vector2)player.position + offset;
    }

    // ======= うんちを出す処理 =======
    void SpawnPoop()
    {
        GameObject poop = Instantiate(poopPrefab, transform.position, Quaternion.identity);
        Destroy(poop, 10f); // 10秒で自動削除
    }

    // ======= プレイヤーにぶつかった時の処理 =======
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            AffinityManager.Instance?.DecreaseAffection(10); // ぶつかると好感度減少
        }
    }

    // ======= 引っ張り状態の設定とアニメーション制御 =======
    public void SetPulledState(bool pulled, Vector2 pullDirection = default)
    {
        if (animator == null) return;

        if (pulled)
        {
            isPulled = true;

            // 一旦全ての引っ張りトリガーをリセット
            animator.ResetTrigger("pull_l");
            animator.ResetTrigger("pull_r");
            animator.ResetTrigger("pull_b");
            animator.ResetTrigger("pull_s");

            // 方向に応じてトリガー設定
            if (Mathf.Abs(pullDirection.x) > Mathf.Abs(pullDirection.y))
            {
                animator.SetTrigger(pullDirection.x > 0 ? "pull_r" : "pull_l");
            }
            else
            {
                animator.SetTrigger(pullDirection.y > 0 ? "pull_b" : "pull_s");
            }

            // アニメーション方向反映
            animator.SetFloat("moveX", pullDirection.normalized.x);
            animator.SetFloat("moveY", pullDirection.normalized.y);
            animator.SetFloat("speed", speed);

            // スプライトの向き調整
            if (Mathf.Abs(pullDirection.x) > 0.5f)
                GetComponent<SpriteRenderer>().flipX = (pullDirection.x < 0);
        }
        else
        {
            isPulled = false;
        }
    }

    // ======= 引っ張り状態かどうかを返す =======
    public bool IsPulled() => isPulled;
}
