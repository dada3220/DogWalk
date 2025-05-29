using UnityEngine;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(Animator))]
public class Cat : Item
{
    public int dogAffectionValueDown = -10;     // プレイヤーが取った時の好感度ダウン
    public int dogAffectionValue = 10;    // 犬が取った時の好感度アップ
    public int runDuration = 5000;    // 強制移動の継続時間（秒）

    public float speed = 2f;                // 猫の移動速度
    public float boostedSpeed = 8f;         // 犬のブースト時速度
    private Vector3 moveDirection;
    private bool collected = false;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    protected override void Start()
    {
        base.Start();

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (mainCamera != null)
        {
            Vector3 target = mainCamera.transform.position;
            target.z = 0;
            moveDirection = (target - transform.position).normalized;

            // 向きに応じて反転（左向きならflipX）
            if (moveDirection.x < 0 && spriteRenderer != null)
            {
                spriteRenderer.flipX = true;
            }
        }

        // アニメーション開始（常に歩き）
        if (animator != null)
        {
            animator.SetBool("cat", true);
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
        // AffinityManager に好感度減算
        if (AffinityManager.Instance != null)
        {
            AffinityManager.Instance.IncreaseAffection(dogAffectionValueDown);
        }

        // 自身を削除
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
