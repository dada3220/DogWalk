using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

public class DogController : MonoBehaviour
{
    [Header("移動関連")]
    public float speed = 4f;                // 現在の移動速度
    public float pullSpeed = 1f;            // 引っ張られた時の速度
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

    private CancellationTokenSource moveCTS;
    [SerializeField] private GameManager gameManager;

    private float defaultSpeed; // 元の通常速度を保存するフィールド

    void Start()
    {
        defaultSpeed = speed; // 初期速度を記録
        animator = GetComponent<Animator>();
        targetPosition = GetRandomTarget();
        dogMarker = new DogMarker(this, animator);
        dogMarker.OnMarkingFinished += HandleMarkingFinished;
    }

    void Update()
    {
        if (gameManager != null && !gameManager.IsPlaying) return;
        if (player == null) return;

        timer += Time.deltaTime;
        affectionTimer += Time.deltaTime;
        poopTimer += Time.deltaTime;

        Vector2 previousPosition = transform.position;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // プレイヤーから離れすぎたら引っ張られる
        if (distanceToPlayer > maxDistance)
            SetPulledState(true, (player.position - transform.position).normalized);
        else if (isPulled && distanceToPlayer <= maxDistance - 0.5f)
            SetPulledState(false);

        if (dogMarker.IsMarking) return; // マーキング中は移動しない

        // 木に向かって移動
        if (dogMarker.CurrentTree != null && !isPulled)
        {
            Vector2 moveDir = ((Vector2)dogMarker.CurrentTree.transform.position - (Vector2)transform.position).normalized;
            transform.position += (Vector3)(moveDir * GetCurrentSpeed() * Time.deltaTime);

            if (Vector2.Distance(transform.position, dogMarker.CurrentTree.transform.position) < 1.0f)
                dogMarker.TryStartMarking(dogMarker.CurrentTree);

            lastMoveDir = moveDir;
        }
        // ランダム移動
        else if (!isPulled)
        {
            if (isWaitingAfterMove)
            {
                // 待機中は移動しない
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
                    transform.position += (Vector3)(moveDir * GetCurrentSpeed() * Time.deltaTime);
                    lastMoveDir = moveDir;
                }
            }
        }

        // 引っ張られ移動
        if (isPulled)
        {
            Vector2 pullDir = (player.position - transform.position).normalized;
            transform.position += (Vector3)(pullDir * GetCurrentSpeed() * Time.deltaTime);
        }

        UpdateAnimation(previousPosition);

        // 好感度処理
        if (affectionTimer >= affectionCheckInterval)
        {
            if (isPulled)
            {
                int decreaseAmount = Mathf.RoundToInt(pullAffectionLossRate * affectionCheckInterval);
                AffinityManager.Instance?.DecreaseAffection(decreaseAmount);
            }
            else if (distanceToPlayer <= minDistanceForBoost)
            {
                AffinityManager.Instance?.IncreaseAffection(6);
            }

            affectionTimer = 0f;
        }

        // うんち処理
        if (poopTimer >= poopInterval)
        {
            poopSpawner?.SpawnPoop(transform.position);
            poopTimer = 0f;
        }
    }

    private async UniTaskVoid WaitBeforeNextMove()
    {
        moveCTS?.Cancel();
        moveCTS = new CancellationTokenSource();

        try
        {
            await UniTask.Delay(waitInterval, cancellationToken: moveCTS.Token);
            if (this == null || player == null) return;

            targetPosition = GetRandomTarget();
            isWaitingAfterMove = false;
            timer = 0f;
        }
        catch (OperationCanceledException) { }
    }

    private Vector2 GetRandomTarget()
    {
        Vector2 offset = new Vector2(
            UnityEngine.Random.Range(-wanderRadius, wanderRadius),
            UnityEngine.Random.Range(-wanderRadius, wanderRadius)
        );

        Vector2 basePos = player != null ? (Vector2)player.position : (Vector2)transform.position;
        return basePos + offset;
    }

    public bool IsDogBusy()
    {
        return dogMarker.IsMarking || dogMarker.CurrentTree != null;
    }

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
        speed = defaultSpeed;  // 元の速度に戻す
        isBoosted = false;
    }

    private float GetCurrentSpeed()
    {
        return isPulled ? pullSpeed : speed;
    }

    public void GoToTarget(Vector2 position, Stump stump)
    {
        // すでにマーキング中 or ターゲットがある場合は無視
        if (stump == null || stump.IsMarked() || dogMarker.IsMarking || dogMarker.CurrentTree != null) return;

        targetPosition = position;
        dogMarker.SetTarget(stump);
    }


    private void HandleMarkingFinished()
    {
        targetPosition = GetRandomTarget();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            AffinityManager.Instance?.DecreaseAffection(10);
            SEManager.Instance?.Play("dogCry");
        }
    }

    void OnDestroy()
    {
        moveCTS?.Cancel();
        moveCTS?.Dispose();
    }
}
