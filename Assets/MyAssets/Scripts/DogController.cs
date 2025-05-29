using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

/// <summary>
/// 犬の行動とアニメーションをコードで制御するクラス
/// </summary>
public class DogController : MonoBehaviour
{
    [Header("移動関連")]
    public float speed = 2.0f;             // 通常移動速度
    public float pullSpeed = 0.8f;         // 引っ張り時の速度
    private float currentSpeed;            // 状態に応じた現在速度

    public Vector2 targetPosition;         // ランダム移動先
    public float changeTargetTime = 3f;    // ターゲット更新間隔
    public float wanderRadius = 3f;        // 徘徊範囲
    public Transform player;               // プレイヤー参照

    [Header("好感度関連")]
    public float affectionCheckInterval = 1f;
    public float maxDistance = 5f;         // 引っ張り状態になる距離
    public float minDistanceForBoost = 2f; // 好感度上昇距離
    public float pullAffectionLossRate = 3f;

    [Header("うんち関連")]
    public float poopInterval = 10f;
    public PoopSpawner poopSpawner;

    // 内部状態
    private float timer = 0f;
    private float affectionTimer = 0f;
    private float poopTimer = 0f;

    private Animator animator;
    private bool isPulled = false;
    private bool isMarking = false;
    private float markingTimer = 0f;
    private YellowTree currentTree = null;
    private bool isGoingToTree = false;
    private Vector2 lastMoveDir = Vector2.down;

    private CancellationTokenSource pullCancelToken;

    void Start()
    {
        SetNewTargetPosition();
        animator = GetComponent<Animator>();
        currentSpeed = speed;
    }

    void Update()
    {
        timer += Time.deltaTime;
        affectionTimer += Time.deltaTime;
        poopTimer += Time.deltaTime;

        Vector2 previousPosition = transform.position;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // 距離による引っ張り状態の管理
        if (distanceToPlayer > maxDistance)
        {
            Vector2 pullDir = (Vector2)(player.position - transform.position).normalized;
            SetPulledState(true, pullDir);
        }
        else if (IsPulled() && distanceToPlayer <= maxDistance)
        {
            SetPulledState(false);

            // 引っ張りが解除されたら、木に戻る状態を再開
            if (currentTree != null)
            {
                isGoingToTree = true;
            }
        }

        if (isMarking)
        {
            markingTimer -= Time.deltaTime;
            if (markingTimer <= 0f)
            {
                isMarking = false;
                currentTree?.OnDogArrived();
                currentTree = null;
                isGoingToTree = false;
                timer = changeTargetTime;
            }
            return;
        }

        // 木に向かっているとき
        if (isGoingToTree && currentTree != null && !IsPulled())
        {
            Vector2 treeTarget = currentTree.transform.position;

            Vector2 moveDir = (treeTarget - (Vector2)transform.position).normalized;
            transform.position += (Vector3)(moveDir * currentSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, treeTarget) < 0.3f)
            {
                StartMarking(currentTree);
            }

            lastMoveDir = moveDir;
        }
        // ランダム徘徊
        else if (!IsPulled())
        {
            if (timer >= changeTargetTime)
            {
                SetNewTargetPosition();
                timer = 0f;
            }

            Vector2 moveDir = (targetPosition - (Vector2)transform.position).normalized;
            transform.position += (Vector3)(moveDir * currentSpeed * Time.deltaTime);
            lastMoveDir = moveDir;
        }

        // 引っ張り中の移動（プレイヤー方向）
        if (IsPulled())
        {
            Vector2 pullDir = (Vector2)(player.position - transform.position).normalized;
            transform.position += (Vector3)(pullDir * currentSpeed * Time.deltaTime);
        }

        // アニメーション制御
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

        // 引っ張り中の好感度減少
        if (IsPulled())
        {
            AffinityManager.Instance?.DecreaseAffection(
                Mathf.RoundToInt(pullAffectionLossRate * Time.deltaTime));
        }

        // うんち生成
        if (poopTimer >= poopInterval)
        {
            poopSpawner?.SpawnPoop(transform.position);
            poopTimer = 0f;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Stump"))
        {
            YellowTree tree = other.GetComponent<YellowTree>();
            if (tree != null)
            {
                StartMarking(tree);
            }
        }
    }


    private void UpdateAnimation(Vector2 previousPosition)
    {
        Vector2 moveVector = (Vector2)transform.position - previousPosition;
        float moveSpeed = moveVector.magnitude / Time.deltaTime;

        if (animator == null) return;

        if (IsPulled())
        {
            SetAnimationState("pull", moveVector);
        }
        else if (isMarking)
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

    private void SetAnimationState(string state, Vector2 dir)
    {
        if (dir == Vector2.zero) dir = Vector2.down;

        string direction;
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            direction = dir.x > 0 ? "right" : "left";
        else
            direction = dir.y > 0 ? "back" : "front";

        animator.Play($"{state}_{direction}");
    }

    public void GoToTarget(Vector2 position, YellowTree tree)
    {
        targetPosition = position;
        currentTree = tree;
        isGoingToTree = true;
    }

    private void SetNewTargetPosition()
    {
        Vector2 offset = new Vector2(
            UnityEngine.Random.Range(-wanderRadius, wanderRadius),
            UnityEngine.Random.Range(-wanderRadius, wanderRadius)
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
        isPulled = pulled;

        if (pulled)
        {
            currentSpeed = pullSpeed;
            pullCancelToken?.Cancel();
            pullCancelToken = new CancellationTokenSource();
        }
        else
        {
            currentSpeed = speed;
            pullCancelToken?.Cancel();
        }
    }

    private void StartMarking(YellowTree tree)
    {
        isMarking = true;
        markingTimer = 2f;
        currentTree = tree;
        animator?.Play("dogmarking");
    }

    public bool IsPulled() => isPulled;
}
