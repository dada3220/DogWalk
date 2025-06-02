using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

public class DogController : MonoBehaviour
{
    [Header("移動関連")]
    public float speed = 4f;
    public float pullSpeed = 1f;
    public float changeTargetTime = 3f;
    public float wanderRadius = 3f;
    public Transform player; // プレイヤーのTransform参照
    public int waitInterval = 500; // 次の移動までの待機時間（ms）

    [Header("好感度関連")]
    public float affectionCheckInterval = 1f;
    public float maxDistance = 5f; // プレイヤーから離れすぎたら引っ張られる
    public float minDistanceForBoost = 2f; // プレイヤーに近いと好感度UP
    public float pullAffectionLossRate = 3f; // 引っ張られている間の好感度減衰

    [Header("うんち関連")]
    public float poopInterval = 10f;
    public PoopSpawner poopSpawner;

    // 内部状態管理
    private float timer = 0f;
    private float affectionTimer = 0f;
    private float poopTimer = 0f;

    private Animator animator;
    private bool isPulled = false;
    private Vector2 lastMoveDir = Vector2.down;

    private bool isBoosted = false;
    private bool isWaitingAfterMove = false;

    private Vector2 targetPosition;
    private DogMarker dogMarker;

    private CancellationTokenSource moveCTS; // 非同期処理キャンセル用
    [SerializeField] private GameManager gameManager;


    void Start()
    {
        animator = GetComponent<Animator>();
        targetPosition = GetRandomTarget(); // 最初の目標座標を設定
        dogMarker = new DogMarker(this, animator);

        // マーキング完了時のコールバック登録
        dogMarker.OnMarkingFinished += HandleMarkingFinished;
    }

    void Update()
    {
        if (gameManager != null && !gameManager.IsPlaying) return;

        if (player == null) return; // プレイヤーが存在しない場合、以降をスキップ

        // タイマー更新
        timer += Time.deltaTime;
        affectionTimer += Time.deltaTime;
        poopTimer += Time.deltaTime;

        Vector2 previousPosition = transform.position;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // プレイヤーから離れすぎたら引っ張りモードへ
        if (distanceToPlayer > maxDistance)
            SetPulledState(true, (player.position - transform.position).normalized);
        else if (isPulled && distanceToPlayer <= maxDistance - 0.5f)
            SetPulledState(false);

        // マーキング中は移動しない
        if (dogMarker.IsMarking) return;

        // 木がターゲットの場合の移動
        if (dogMarker.CurrentTree != null && !isPulled)
        {
            Vector2 moveDir = ((Vector2)dogMarker.CurrentTree.transform.position - (Vector2)transform.position).normalized;
            transform.position += (Vector3)(moveDir * GetCurrentSpeed() * Time.deltaTime);

            if (Vector2.Distance(transform.position, dogMarker.CurrentTree.transform.position) < 1.0f)
                dogMarker.TryStartMarking(dogMarker.CurrentTree);

            lastMoveDir = moveDir;
        }
        // 通常のランダム移動
        else if (!isPulled)
        {
            if (isWaitingAfterMove)
            {
                // 待機中は何もしない
            }
            else
            {
                Vector2 toTarget = targetPosition - (Vector2)transform.position;
                if (toTarget.magnitude < 0.2f)
                {
                    isWaitingAfterMove = true;
                    WaitBeforeNextMove().Forget(); // 非同期で次の移動へ
                }
                else
                {
                    Vector2 moveDir = toTarget.normalized;
                    transform.position += (Vector3)(moveDir * GetCurrentSpeed() * Time.deltaTime);
                    lastMoveDir = moveDir;
                }
            }
        }

        // 引っ張られている状態の移動
        if (isPulled)
        {
            Vector2 pullDir = (player.position - transform.position).normalized;
            transform.position += (Vector3)(pullDir * GetCurrentSpeed() * Time.deltaTime);
        }

        UpdateAnimation(previousPosition);

        // 好感度処理
        if (affectionTimer >= affectionCheckInterval)
        {
            if (distanceToPlayer > minDistanceForBoost)
                AffinityManager.Instance?.DecreaseAffection(4);
            else
                AffinityManager.Instance?.IncreaseAffection(6);

            affectionTimer = 0f;
        }

        // 引っ張られている間は徐々に好感度減少
        if (isPulled)
        {
            AffinityManager.Instance?.DecreaseAffection(
                Mathf.RoundToInt(pullAffectionLossRate * Time.deltaTime));
        }

        // うんち生成タイマー処理
        if (poopTimer >= poopInterval)
        {
            poopSpawner?.SpawnPoop(transform.position);
            poopTimer = 0f;
        }
    }

    // 一定時間待機してから次のランダム移動地点へ
    private async UniTaskVoid WaitBeforeNextMove()
    {
        moveCTS?.Cancel(); // 前のタスクをキャンセル
        moveCTS = new CancellationTokenSource();

        try
        {
            await UniTask.Delay(waitInterval, cancellationToken: moveCTS.Token);
            if (this == null || player == null) return;

            targetPosition = GetRandomTarget();
            isWaitingAfterMove = false;
            timer = 0f;
        }
        catch (OperationCanceledException)
        {
            // タスクがキャンセルされた場合は何もしない
        }
    }

    // ランダムな目標地点を生成（playerの周囲）
    private Vector2 GetRandomTarget()
    {
        Vector2 offset = new Vector2(
            UnityEngine.Random.Range(-wanderRadius, wanderRadius),
            UnityEngine.Random.Range(-wanderRadius, wanderRadius)
        );

        Vector2 basePos = player != null ? (Vector2)player.position : (Vector2)transform.position;
        return basePos + offset;
    }

    // アニメーションの更新処理
    private void UpdateAnimation(Vector2 previousPosition)
    {
        Vector2 moveVector = (Vector2)transform.position - previousPosition;
        float moveSpeed = moveVector.magnitude / Time.deltaTime;

        if (animator == null) return;

        if (isPulled)
        {
            SetAnimationState("pull", moveVector);
        }
        else if (dogMarker.IsMarking)
        {
            animator.Play("dogmarking");
        }
        else if (moveSpeed < 0.01f)
        {
            SetAnimationState("idle", lastMoveDir);
        }
        else
        {
            lastMoveDir = moveVector;
            SetAnimationState("walk", moveVector);
        }
    }

    // アニメーションステートと方向を設定
    private void SetAnimationState(string state, Vector2 dir)
    {
        if (dir == Vector2.zero) dir = Vector2.down;

        string direction = Mathf.Abs(dir.x) > Mathf.Abs(dir.y)
            ? (dir.x > 0 ? "right" : "left")
            : (dir.y > 0 ? "back" : "front");

        animator.Play($"{state}_{direction}");
    }

    public void SetPulledState(bool pulled, Vector2 pullDirection = default)
    {
        isPulled = pulled;
    }

    public bool IsPulled() => isPulled;

    public void SetSpeed(float newSpeed, bool boosted = false)
    {
        speed = newSpeed;
        isBoosted = boosted;
    }

    public void ResetSpeed()
    {
        isBoosted = false;
    }

    // 実際の移動速度を取得
    private float GetCurrentSpeed()
    {
        return isBoosted ? speed : (isPulled ? pullSpeed : speed);
    }

    // 木のターゲットに向かわせる
    public void GoToTarget(Vector2 position, Stump stump)
    {
        if (stump == null || stump.IsMarked() || stump == dogMarker.CurrentTree) return;

        targetPosition = position;
        dogMarker.SetTarget(stump);
    }

    private void HandleMarkingFinished()
    {
        // ターゲット座標を無効にする
        targetPosition = GetRandomTarget();
    }

    // プレイヤーと衝突したときの好感度減少処理
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            AffinityManager.Instance?.DecreaseAffection(10);
        }
    }

    // オブジェクト破棄時に非同期タスクを安全にキャンセル
    void OnDestroy()
    {
        moveCTS?.Cancel();
        moveCTS?.Dispose();
    }
}
