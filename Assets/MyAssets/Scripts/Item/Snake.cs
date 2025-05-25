using UnityEngine;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(Animator))]
public class Snake : FieldItem
{
    public int playerScoreValue = -100;     // プレイヤーが取った時のスコア
    public int dogAffectionValue = -10;     // 犬が取った時の好感度
    public int runDuration = 3000;          // ブースト継続時間（ミリ秒）

    public float speed = 2f;                // 蛇の移動速度
    public float boostedSpeed = 10f;        // ブースト時の犬の速度
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

            // 向きに応じてスプライトを回転させる（アニメーションを共有する）
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        // アニメーション開始（常に歩き）
        if (animator != null)
        {
            animator.SetBool("snake", true);
        }
    }

    protected override void Update()
    {
        base.Update();

        if (collected) return;

        // カメラ中心に向かって移動
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
            float originalSpeed = dog.speed;

            dog.speed = boostedSpeed;

            if (AffinityManager.Instance != null)
            {
                AffinityManager.Instance.IncreaseAffection(dogAffectionValue);
            }

            await UniTask.Delay(runDuration);

            dog.speed = originalSpeed;
        }
    }
}
