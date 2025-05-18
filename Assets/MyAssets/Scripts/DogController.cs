using UnityEngine;
using UnityEngine.UI;

public class DogController : MonoBehaviour
{
    public float speed = 3f; // 犬の移動速度
    private Vector2 targetPosition; // 目的地座標
    private float changeTargetTime = 1f; // 新しい目的地に切り替える時間間隔
    private float timer = 0f; // 経過時間管理

    public Transform player; // プレイヤーのTransform
    public float wanderRadius = 5f; // プレイヤーの周囲何メートル以内でうろうろするか

    // 好感度関連
    public int affection = 100; // 初期好感度
    public float affectionCheckInterval = 1f; // 好感度をチェックする間隔
    private float affectionTimer = 0f; // 好感度用タイマー
    public float maxDistance = 5f; // 好感度が下がる距離
    public float minDistanceForBoost = 2f; // 好感度が上がる距離
    public Slider affectionSlider; // UIのスライダーで好感度表示

    // うんこ関連
    public GameObject poopPrefab; // うんこプレハブ
    public float poopInterval = 10f; // うんこを出す間隔
    private float poopTimer = 0f;

    // リード関連
    public float leashMaxLength = 5f; // リードの最大距離
    public float pullAffectionLossRate = 2f; // 引っ張られたときの好感度減少速度（毎秒）

    private Animator animator; // アニメーター参照

    void Start()
    {
        SetNewTargetPosition(); // 初期移動ターゲット設定

        // UIスライダー初期化
        if (affectionSlider != null)
        {
            affectionSlider.maxValue = 100;
            affectionSlider.value = affection;
        }

        animator = GetComponent<Animator>(); // アニメーター取得
    }

    void Update()
    {
        // タイマー更新
        timer += Time.deltaTime;
        affectionTimer += Time.deltaTime;
        poopTimer += Time.deltaTime;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position); // プレイヤーとの距離

        // 犬がリードの長さ以上離れてしまった場合
        if (distanceToPlayer > leashMaxLength)
        {
            // 距離制限：犬をプレイヤーに引き戻す
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            transform.position = player.position - (Vector3)(directionToPlayer * leashMaxLength);
        }
        else
        {
            // 犬がリード内にいる場合、自由移動
            if (timer >= changeTargetTime)
            {
                SetNewTargetPosition(); // ランダムな目的地を設定
                timer = 0f;
            }

            // ターゲット位置へ移動
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }

        // 一定時間ごとにプレイヤーとの距離に基づいて好感度を変動させる
        if (affectionTimer >= affectionCheckInterval)
        {
            if (distanceToPlayer > minDistanceForBoost)
            {
                // 離れていたら好感度が減る
                DecreaseAffection(4);
            }
            else
            {
                // 近くにいたら好感度アップ
                affection += 6;
                affection = Mathf.Clamp(affection, 0, 100);
                if (affectionSlider != null) affectionSlider.value = affection;
            }

            affectionTimer = 0f;
        }

        // 犬が引っ張られている間は好感度を持続的に減らす
        if (IsPulled())
        {
            DecreaseAffection(Mathf.RoundToInt(pullAffectionLossRate * Time.deltaTime));
        }

        // 一定時間ごとにうんこを生成
        if (poopTimer >= poopInterval)
        {
            SpawnPoop();
            poopTimer = 0f;
        }
    }

    
    /// 犬の自由移動先をランダムに設定（プレイヤーの周囲）
    void SetNewTargetPosition()
    {
        Vector2 randomOffset = Random.insideUnitCircle * wanderRadius;
        targetPosition = (Vector2)player.position + randomOffset;
    }

    /// うんこを生成して10秒後に自動削除
    void SpawnPoop()
    {
        GameObject poop = Instantiate(poopPrefab, transform.position, Quaternion.identity);
        Destroy(poop, 10f);
    }


    /// 好感度を減少させ、0以下ならゲームオーバー処理
    public void DecreaseAffection(int amount)
    {
        affection -= amount;
        affection = Mathf.Clamp(affection, 0, 100);
        if (affectionSlider != null) affectionSlider.value = affection;
        if (affection <= 0) GameOver();
    }


    /// 好感度がゼロになったときの処理
    void GameOver()
    {
        Debug.Log("Game Over! Dog lost trust.");
        // SceneManager.LoadScene("GameOverScene"); 
    }


    /// プレイヤーとぶつかったとき好感度がさがる
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            DecreaseAffection(10);
        }
    }

    /// プレイヤーから踏ん張り状態を設定される
    public void SetPulledState(bool isPulled)
    {
        if (animator != null)
        {
            animator.SetBool("isPulled", isPulled);
        }
    }


    /// アニメーションの状態から「引っ張られているか」を取得
    public bool IsPulled()
    {
        return animator != null && animator.GetBool("isPulled");
    }
}
