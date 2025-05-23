using UnityEngine;
using UnityEngine.UI;

public class DogController : MonoBehaviour
{
    public float speed = 1f; // 通常の移動速度
    private Vector2 targetPosition; // 犬の次の目的地
    private float changeTargetTime = 1f; // 目的地を変更する間隔
    private float timer = 0f;

    public Transform player; // プレイヤーの Transform 参照
    public float wanderRadius = 6f; // プレイヤー周辺での犬の移動範囲

    public int affection = 100; // 好感度（0〜100）
    public float affectionCheckInterval = 1f; // 好感度チェックの間隔
    private float affectionTimer = 0f;
    public float maxDistance = 6.5f; // プレイヤーから離れすぎた場合の距離（引っ張りアニメ開始）
    public float minDistanceForBoost = 2f; // プレイヤーと近いと好感度が上がる距離
    public Slider affectionSlider; // UIで好感度を表示

    public GameObject poopPrefab; // うんちのプレハブ
    public float poopInterval = 10f; // うんちを出す間隔
    private float poopTimer = 0f;

    public float leashMaxLength = 7f; // リードの最大長さ（これ以上離れると強制追従）
    public float pullAffectionLossRate = 2f; // 引っ張られているときの好感度減少率

    private Animator animator; // Animator コンポーネント参照
    private bool isPulled = false; // 現在引っ張られているかどうか

    void Start()
    {
        SetNewTargetPosition(); // 最初の目的地を設定

        if (affectionSlider != null)
        {
            affectionSlider.maxValue = 100;
            affectionSlider.value = affection;
        }

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        affectionTimer += Time.deltaTime;
        poopTimer += Time.deltaTime;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        Vector2 previousPosition = transform.position;

        // プレイヤーからリード範囲以上に離れていたら、速度1fでプレイヤーへ直線追従
        if (distanceToPlayer > leashMaxLength)
        {
            Vector2 pullDir = (player.position - transform.position).normalized;
            float pullSpeed = 1f; // 引っ張られているときは常に速度1f

            // MoveTowards でスムーズに追従
            transform.position = Vector2.MoveTowards(transform.position, player.position, pullSpeed * Time.deltaTime);

            SetPulledState(true, pullDir); // アニメーターに「引っ張られている」状態を通知
        }
        // プレイヤーから少し離れているがリード範囲内：引っ張りアニメだけ再生
        else if (distanceToPlayer > maxDistance)
        {
            Vector2 pullDir = (player.position - transform.position).normalized;
            SetPulledState(true, pullDir); // 位置はそのままでアニメーションのみ切り替え
        }
        // 通常のランダム移動状態
        else
        {
            if (timer >= changeTargetTime)
            {
                SetNewTargetPosition(); // 新しい目的地を設定
                timer = 0f;
            }

            Vector2 moveDir = (targetPosition - (Vector2)transform.position);

            // 移動方向を x または y のみに限定（斜め移動を避ける）
            if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.y))
                moveDir.y = 0;
            else
                moveDir.x = 0;

            moveDir.Normalize();

            transform.position = (Vector2)transform.position + moveDir * speed * Time.deltaTime;

            SetPulledState(false); // 通常移動中なので引っ張りフラグをオフ
        }

        // アニメーションの移動方向と速度を更新（引っ張られていない場合）
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

        // 一定間隔で好感度を増減
        if (affectionTimer >= affectionCheckInterval)
        {
            if (distanceToPlayer > minDistanceForBoost)
            {
                DecreaseAffection(4); // 離れすぎ：好感度低下
            }
            else
            {
                affection += 6; // 近くにいる：好感度上昇
                affection = Mathf.Clamp(affection, 0, 100);
                if (affectionSlider != null) affectionSlider.value = affection;
            }

            affectionTimer = 0f;
        }

        // 引っ張られている場合、好感度が徐々に下がる
        if (IsPulled())
        {
            DecreaseAffection(Mathf.RoundToInt(pullAffectionLossRate * Time.deltaTime));
        }

        // 一定時間ごとにうんちを出す
        if (poopTimer >= poopInterval)
        {
            SpawnPoop();
            poopTimer = 0f;
        }
    }

    // プレイヤー周辺のランダムな位置を新たな目的地にする
    void SetNewTargetPosition()
    {
        Vector2 offset = Vector2.zero;
        if (Random.value > 0.5f)
            offset.x = Random.Range(-wanderRadius, wanderRadius);
        else
            offset.y = Random.Range(-wanderRadius, wanderRadius);

        targetPosition = (Vector2)player.position + offset;
    }

    // うんちを生成して10秒後に消す
    void SpawnPoop()
    {
        GameObject poop = Instantiate(poopPrefab, transform.position, Quaternion.identity);
        Destroy(poop, 10f);
    }

    // 好感度を減らす処理
    public void DecreaseAffection(int amount)
    {
        affection -= amount;
        affection = Mathf.Clamp(affection, 0, 100);
        if (affectionSlider != null) affectionSlider.value = affection;
        if (affection <= 0) GameOver(); // 0以下でゲームオーバー
    }

    // ゲームオーバー時の処理
    void GameOver()
    {
        Debug.Log("Game Over! Dog lost trust.");
    }

    // プレイヤーとぶつかったら好感度を減らす
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            DecreaseAffection(10);
        }
    }

    // 引っ張られている状態をアニメーターに伝える
    public void SetPulledState(bool pulled, Vector2 pullDirection = default)
    {
        if (animator == null) return;

        if (pulled)
        {
            isPulled = true;

            // すべてのトリガーをリセット
            animator.ResetTrigger("pull_l");
            animator.ResetTrigger("pull_r");
            animator.ResetTrigger("pull_b");
            animator.ResetTrigger("pull_s");

            // 移動方向に応じた引っ張りアニメーションを再生
            if (Mathf.Abs(pullDirection.x) > Mathf.Abs(pullDirection.y))
            {
                if (pullDirection.x > 0)
                    animator.SetTrigger("pull_r");
                else if (pullDirection.x < 0)
                    animator.SetTrigger("pull_l");
            }
            else
            {
                if (pullDirection.y > 0)
                    animator.SetTrigger("pull_b");
                else if (pullDirection.y < 0)
                    animator.SetTrigger("pull_s");
            }

            // スプライトの向きを調整
            if (Mathf.Abs(pullDirection.x) > 0.5f)
            {
                GetComponent<SpriteRenderer>().flipX = (pullDirection.x < 0);
            }
        }
        else
        {
            isPulled = false;
        }
    }

    // 現在引っ張られているかどうかを返す
    public bool IsPulled()
    {
        return isPulled;
    }
}
