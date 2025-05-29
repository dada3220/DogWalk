using UnityEngine;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(Animator))]
public class Snake : Item
{
    public int playerScoreValue = -100;
    public int dogAffectionValue = -10;
    public int runDuration = 5000; // ƒ~ƒŠ•b

    public float speed = 2f;
    public float boostedSpeed = 12f;
    private Vector3 moveDirection;
    private bool collected = false;

    private Animator animator;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();

        if (mainCamera != null)
        {
            Vector3 target = mainCamera.transform.position;
            target.z = 0;
            moveDirection = (target - transform.position).normalized;

            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        if (animator != null)
        {
            animator.SetBool("snake", true);
        }
    }

    protected override void Update()
    {
        base.Update();

        if (collected) return;

        transform.position += moveDirection * speed * Time.deltaTime;
    }

    protected override void OnPlayerCollect()
    {
        ScoreManager scoreManager = FindFirstObjectByType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.AddScore(playerScoreValue);
        }

        Destroy(gameObject);
    }

    protected override void OnDogCollect()
    {
        if (collected) return;
        collected = true;

        BoostDog().Forget();

        Destroy(gameObject);
    }

    private async UniTaskVoid BoostDog()
    {
        DogController dog = FindFirstObjectByType<DogController>();
        if (dog != null)
        {
            dog.SetSpeed(boostedSpeed, true);

            if (AffinityManager.Instance != null)
            {
                AffinityManager.Instance.IncreaseAffection(dogAffectionValue);
            }

            await UniTask.Delay(runDuration);

            dog.ResetSpeed();
        }
    }
}
