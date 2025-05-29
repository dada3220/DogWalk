using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

/// <summary>
/// 犬の行動とアニメーションを制御するクラス
/// </summary>
public class DogController : MonoBehaviour
{
    [Header("移動関連")]
    public float speed = 4f;
    public float pullSpeed = 1f;
    private float currentSpeed;
    private float normalSpeed;

    public Vector2 targetPosition;
    public float changeTargetTime = 3f;
    public float wanderRadius = 3f;
    public Transform player;
    public int waitInterval = 500;

    [Header("好感度関連")]
    public float affectionCheckInterval = 1f;
    public float maxDistance = 5f;
    public float minDistanceForBoost = 2f;
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
    private Stump currentTree = null;
    private bool isGoingToTree = false;
    private Vector2 lastMoveDir = Vector2.down;

    private bool isBoosted = false;
    private bool isWaitingAfterMove = false;
    private CancellationTokenSource pullCancelToken;

    void Start()
    {
        SetNewTargetPosition();
        animator = GetComponent<Animator>();
        currentSpeed = speed;
        normalSpeed = speed;
    }

    void Update()
    {
        timer += Time.deltaTime;
        affectionTimer += Time.deltaTime;
        poopTimer += Time.deltaTime;

        Vector2 previousPosition = transform.position;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > maxDistance)
        {
            Vector2 pullDir = (player.position - transform.position).normalized;
            SetPulledState(true, pullDir);
        }
        else if (IsPulled() && distanceToPlayer <= (maxDistance) - 0.5f)
        {
            SetPulledState(false);
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
        else if (!IsPulled())
        {
            if (isWaitingAfterMove)
            {
                // 到達後待機中
            }
            else
            {
                Vector2 toTarget = targetPosition - (Vector2)transform.position;
                if (toTarget.magnitude < 0.2f)
                {
                    isWaitingAfterMove = true;
                    WaitBeforeNextMove().Forget();
                }
                else
                {
                    Vector2 moveDir = toTarget.normalized;
                    transform.position += (Vector3)(moveDir * currentSpeed * Time.deltaTime);
                    lastMoveDir = moveDir;
                }
            }
        }

        if (IsPulled())
        {
            Vector2 pullDir = (player.position - transform.position).normalized;
            transform.position += (Vector3)(pullDir * currentSpeed * Time.deltaTime);
        }

        UpdateAnimation(previousPosition);

        if (affectionTimer >= affectionCheckInterval)
        {
            if (distanceToPlayer > minDistanceForBoost)
                AffinityManager.Instance?.DecreaseAffection(4);
            else
                AffinityManager.Instance?.IncreaseAffection(6);

            affectionTimer = 0f;
        }

        if (IsPulled())
        {
            AffinityManager.Instance?.DecreaseAffection(
                Mathf.RoundToInt(pullAffectionLossRate * Time.deltaTime));
        }

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
            Stump stump = other.GetComponent<Stump>();

            // すでにマーキング済みまたはマーキング中ならスキップ
            if (stump == null || stump.IsMarked() || isMarking) return;

            StartMarking(stump);
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

    // 木に向かって移動開始（マーキング済み防止付き）
    public void GoToTarget(Vector2 position, Stump stump)
    {
        // マーキング済み or 同じ木をターゲットにしている場合は無視
        if (stump == null || stump.IsMarked() || stump == currentTree) return;

        targetPosition = position;
        currentTree = stump;
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
            if (!isBoosted)
                currentSpeed = pullSpeed;
        }
        else
        {
            if (!isBoosted)
                currentSpeed = speed;
        }
    }

    // マーキング開始（マーキング済み木は無視）
    private void StartMarking(Stump stump)
    {
        if (stump == null || stump.IsMarked()) return;

        isMarking = true;
        markingTimer = 2f;
        currentTree = stump;
        animator?.Play("dogmarking");
    }

    public bool IsPulled() => isPulled;

    public void SetSpeed(float newSpeed, bool boosted = false)
    {
        currentSpeed = newSpeed;
        isBoosted = boosted;
    }

    public void ResetSpeed()
    {
        currentSpeed = speed;
        isBoosted = false;
    }

    public bool IsBoosted() => isBoosted;

    private async UniTaskVoid WaitBeforeNextMove()
    {
        await UniTask.Delay(waitInterval);
        SetNewTargetPosition();
        isWaitingAfterMove = false;
        timer = 0f;
    }
}
