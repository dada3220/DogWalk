using UnityEngine;
using Cysharp.Threading.Tasks;

public class DigGround : MonoBehaviour
{
    public float digDuration = 3f; // 掘る時間

    private DogController dog;
    private ScoreManager scoreManager;

    void Start()
    {
        dog = Object.FindFirstObjectByType<DogController>();
        scoreManager = Object.FindFirstObjectByType<ScoreManager>(); // ScoreManager を探す
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Dog"))
        {
            dog = other.GetComponent<DogController>();
            if (dog != null)
            {
                ForceRunAsync().Forget();
            }
        }
        else if (other.CompareTag("Player"))
        {
            // プレイヤーが触れたら無視
        }
    }

    private async UniTask ForceRunAsync()
    {
        // 犬のAIスクリプト
        Animator dogAnimator = dog.GetComponent<Animator>();
        var dogAI = dog.GetComponent<MonoBehaviour>();

        // 掘りアニメーション再生
        if (dogAnimator != null)
        {
            dogAnimator.Play("dogdig");
        }

        // AI を一時停止
        if (dogAI != null)
        {
            dogAI.enabled = false;
        }

        float timer = 0f;

        while (timer < digDuration)
        {
            timer += Time.deltaTime;
            await UniTask.Yield();
        }

        // AI 再開
        if (dogAI != null)
        {
            dogAI.enabled = true;
        }

        // アニメーションを戻す
        if (dogAnimator != null)
        {
            dogAnimator.Play("walk_front");
        }

        // スコア加算（ランダムで 100～500）
        if (scoreManager != null)
        {
            int randomScore = Random.Range(100, 501); // 上限は500
            scoreManager.AddScore(randomScore);
        }

        Destroy(gameObject); // 掘り終わったら消える
    }
}
