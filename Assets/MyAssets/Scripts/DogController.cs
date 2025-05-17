using UnityEngine;
using UnityEngine.UI;

public class DogController : MonoBehaviour
{
    public float speed = 3f;
    private Vector2 targetPosition;
    private float changeTargetTime = 1f;
    private float timer = 0f;

    public Transform player;
    public float wanderRadius = 5f;

    // 好感度関連
    public int affection = 100;
    public float affectionCheckInterval = 1f;
    private float affectionTimer = 0f;
    public float maxDistance = 5f;
    public float minDistanceForBoost = 2f;
    public Slider affectionSlider;

    // うんこ関連
    public GameObject poopPrefab;
    public float poopInterval = 10f;
    private float poopTimer = 0f;

    // リード関連
    public float leashMaxLength = 5f; // プレイヤーとの最大距離
    public float pullAffectionLossRate = 2f; // 引っ張られ時の好感度減少速度

    private Animator animator;

    void Start()
    {
        SetNewTargetPosition();

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

        //リード距離制限：これ以上離れられない
        if (distanceToPlayer > leashMaxLength)
        {
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            transform.position = player.position - (Vector3)(directionToPlayer * leashMaxLength);
        }

        //自由移動（リード内でのみ）
        if (distanceToPlayer <= leashMaxLength)
        {
            if (timer >= changeTargetTime)
            {
                SetNewTargetPosition();
                timer = 0f;
            }

            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }

        //好感度の距離チェック
        if (affectionTimer >= affectionCheckInterval)
        {
            if (distanceToPlayer > minDistanceForBoost)
            {
                affection -= 4;
            }
            else if (distanceToPlayer < minDistanceForBoost)
            {
                affection += 6;
            }

            affection = Mathf.Clamp(affection, 0, 100);
            if (affectionSlider != null) affectionSlider.value = affection;
            if (affection <= 0) GameOver();

            affectionTimer = 0f;
        }

        //引っ張られている間、好感度を減らす
        if (animator != null && animator.GetBool("isPulled"))
        {
            affection -= Mathf.RoundToInt(pullAffectionLossRate * Time.deltaTime);
            affection = Mathf.Clamp(affection, 0, 100);
            if (affectionSlider != null) affectionSlider.value = affection;
            if (affection <= 0) GameOver();
        }

        //うんこ生成
        if (poopTimer >= poopInterval)
        {
            SpawnPoop();
            poopTimer = 0f;
        }
    }

    void SetNewTargetPosition()
    {
        Vector2 randomOffset = Random.insideUnitCircle * wanderRadius;
        targetPosition = (Vector2)player.position + randomOffset;
    }

    void SpawnPoop()
    {
        GameObject poop = Instantiate(poopPrefab, transform.position, Quaternion.identity);
        Destroy(poop, 10f); // 自動削除（任意）
    }

    public void DecreaseAffection(int amount)
    {
        affection -= amount;
        affection = Mathf.Clamp(affection, 0, 100);
        if (affectionSlider != null) affectionSlider.value = affection;
        if (affection <= 0) GameOver();
    }

    void GameOver()
    {
        Debug.Log("Game Over! Dog lost trust.");
        // SceneManager.LoadScene("GameOverScene"); // 必要に応じて
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            DecreaseAffection(10);
        }
    }

    //プレイヤー側から踏ん張りアニメーションを制御
    public void SetPulledState(bool isPulled)
    {
        if (animator != null)
        {
            animator.SetBool("isPulled", isPulled);
        }
    }
}
