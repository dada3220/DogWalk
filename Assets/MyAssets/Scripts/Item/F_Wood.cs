using UnityEngine;
using Cysharp.Threading.Tasks; // UniTask を使うために必要

// FavoriteWood クラス：プレイヤーまたは犬が拾えるアイテム
public class F_Wood : FieldItem
{
    public int playerScoreValue = 100;      // プレイヤーが拾った時に増えるスコア
    public int dogAffectionValue = 10;      // 犬が拾った時に増える愛情度
    public float runDuration = 3f;          // 強制移動の継続時間（秒）
    public float runSpeed = 3f;             // 強制移動の速度

    // プレイヤーがアイテムを拾った時の処理
    protected override void OnPlayerCollect()
    {
        var scoreManager = FindFirstObjectByType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.AddScore(playerScoreValue); // スコア加算
        }

        Destroy(gameObject); // アイテムを削除
    }

    // 犬がアイテムを拾った時の処理（非同期）
    protected override async void OnDogCollect()
    {
        if (AffinityManager.Instance != null)
        {
            AffinityManager.Instance.IncreaseAffection(dogAffectionValue);
        }

        // アイテム消去は先にしておく
        Destroy(gameObject);

        if (dog != null && player != null)
        {
            await ForceRunAsync();
        }
    }

    // プレイヤーと犬を一定時間強制的に下方向に移動させる処理（AI制御停止＆Transform移動）
    private async UniTask ForceRunAsync()
    {
        Animator dogAnimator = dog.GetComponent<Animator>();
        Animator playerAnimator = player.GetComponent<Animator>();

        // 犬のAIスクリプト（例: DogAI）に置き換えてください
        var dogAI = dog.GetComponent<MonoBehaviour>();

        // プレイヤーの移動スクリプト（例: PlayerController）に置き換えてください
        var playerController = player.GetComponent<MonoBehaviour>();

        // 犬の走るアニメーション再生
        if (dogAnimator != null)
        {
            dogAnimator.Play("dog_wood");
        }

        // プレイヤーの走るアニメーション再生
        if (playerAnimator != null)
        {
            playerAnimator.Play("Player_s");
        }

        // AIがあれば一時停止
        if (dogAI != null)
        {
            dogAI.enabled = false;
        }

        // プレイヤー移動スクリプトを一時停止
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        float timer = 0f;
        Vector3 moveVector = Vector3.down * runSpeed;

        // 一定時間ループ
        while (timer < runDuration)
        {
            dog.transform.Translate(moveVector * Time.deltaTime);
            player.transform.Translate(moveVector * Time.deltaTime);

            timer += Time.deltaTime;
            await UniTask.Yield();
        }

        // AIを再開
        if (dogAI != null)
        {
            dogAI.enabled = true;
        }

        // プレイヤー移動スクリプトを再開
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        // 犬のアニメーションをアイドルに戻す
        if (dogAnimator != null)
        {
            dogAnimator.Play("dog_s");
        }

        // プレイヤーのアニメーションをアイドルに戻す
        if (playerAnimator != null)
        {
            playerAnimator.Play("Player_sF");
        }
    }
}
