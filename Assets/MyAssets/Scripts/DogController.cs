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
    public float poopInterval = 6f;
    private float poopTimer = 0f;

    void Start()
    {
        SetNewTargetPosition();

        if (affectionSlider != null)
        {
            affectionSlider.maxValue = 100;
            affectionSlider.value = affection;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        affectionTimer += Time.deltaTime;
        poopTimer += Time.deltaTime;

        // 犬の移動
        if (timer >= changeTargetTime)
        {
            SetNewTargetPosition();
            timer = 0f;
        }

        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // 好感度更新
        if (affectionTimer >= affectionCheckInterval)
        {
            float distance = Vector2.Distance(transform.position, player.position);

            if (distance > maxDistance)
            {
                affection -= 5;
            }
            else if (distance < minDistanceForBoost)
            {
                affection += 2;
            }

            affection = Mathf.Clamp(affection, 0, 100);

            if (affectionSlider != null)
            {
                affectionSlider.value = affection;
            }

            if (affection <= 0)
            {
                GameOver();
            }

            affectionTimer = 0f;
        }

        // うんこ生成
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
        Instantiate(poopPrefab, transform.position, Quaternion.identity);
    }

    public void DecreaseAffection(int amount)
    {
        affection -= amount;
        affection = Mathf.Clamp(affection, 0, 100);
        if (affectionSlider != null)
        {
            affectionSlider.value = affection;
        }

        if (affection <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        Debug.Log("Game Over! Dog lost trust.");
        // SceneManager.LoadScene("GameOverScene");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            affection -= 10;
            affection = Mathf.Clamp(affection, 0, 100);
            if (affectionSlider != null)
            {
                affectionSlider.value = affection;
            }

            if (affection <= 0)
            {
                GameOver();
            }
        }
    }
}
