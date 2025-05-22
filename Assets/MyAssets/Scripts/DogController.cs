using UnityEngine;
using UnityEngine.UI;

public class DogController : MonoBehaviour
{
    public float speed = 10f;
    private Vector2 targetPosition;
    private float changeTargetTime = 1f;
    private float timer = 0f;

    public Transform player;
    public float wanderRadius = 5f;

    public int affection = 100;
    public float affectionCheckInterval = 1f;
    private float affectionTimer = 0f;
    public float maxDistance = 5f;
    public float minDistanceForBoost = 2f;
    public Slider affectionSlider;

    public GameObject poopPrefab;
    public float poopInterval = 10f;
    private float poopTimer = 0f;

    public float leashMaxLength = 7f;
    public float pullAffectionLossRate = 2f;

    private Animator animator;
    private bool isPulled = false;

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
        Vector2 previousPosition = transform.position;

        if (distanceToPlayer > leashMaxLength)
        {
            Vector2 pullDir = (player.position - transform.position);

            if (Mathf.Abs(pullDir.x) > Mathf.Abs(pullDir.y))
                pullDir.y = 0;
            else
                pullDir.x = 0;

            pullDir.Normalize();

            transform.position = (Vector2)transform.position + pullDir * speed * Time.deltaTime;
            SetPulledState(true, pullDir);
        }
        else if (distanceToPlayer > maxDistance)
        {
            Vector2 pullDir = (player.position - transform.position);

            if (Mathf.Abs(pullDir.x) > Mathf.Abs(pullDir.y))
                pullDir.y = 0;
            else
                pullDir.x = 0;

            pullDir.Normalize();

            SetPulledState(true, pullDir);
        }
        else
        {
            if (timer >= changeTargetTime)
            {
                SetNewTargetPosition();
                timer = 0f;
            }

            Vector2 moveDir = (targetPosition - (Vector2)transform.position);

            if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.y))
                moveDir.y = 0;
            else
                moveDir.x = 0;

            moveDir.Normalize();

            transform.position = (Vector2)transform.position + moveDir * speed * Time.deltaTime;

            SetPulledState(false);
        }

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

        if (affectionTimer >= affectionCheckInterval)
        {
            if (distanceToPlayer > minDistanceForBoost)
            {
                DecreaseAffection(4);
            }
            else
            {
                affection += 6;
                affection = Mathf.Clamp(affection, 0, 100);
                if (affectionSlider != null) affectionSlider.value = affection;
            }

            affectionTimer = 0f;
        }

        if (IsPulled())
        {
            DecreaseAffection(Mathf.RoundToInt(pullAffectionLossRate * Time.deltaTime));
        }

        if (poopTimer >= poopInterval)
        {
            SpawnPoop();
            poopTimer = 0f;
        }
    }

    void SetNewTargetPosition()
    {
        Vector2 offset = Vector2.zero;
        if (Random.value > 0.5f)
            offset.x = Random.Range(-wanderRadius, wanderRadius);
        else
            offset.y = Random.Range(-wanderRadius, wanderRadius);

        targetPosition = (Vector2)player.position + offset;
    }

    void SpawnPoop()
    {
        GameObject poop = Instantiate(poopPrefab, transform.position, Quaternion.identity);
        Destroy(poop, 10f);
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
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            DecreaseAffection(10);
        }
    }

    public void SetPulledState(bool pulled, Vector2 pullDirection = default)
    {
        if (animator == null) return;

        if (pulled)
        {
            isPulled = true;

            animator.ResetTrigger("pull_l");
            animator.ResetTrigger("pull_r");
            animator.ResetTrigger("pull_b");
            animator.ResetTrigger("pull_s");

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

    public bool IsPulled()
    {
        return isPulled;
    }
}
