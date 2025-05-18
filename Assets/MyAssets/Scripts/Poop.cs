using UnityEngine;

public class Poop : MonoBehaviour
{
    public float lifeTime = 10f; // うんこオブジェクトの寿命（秒）
    private float timer = 0f; // 経過時間を記録するためのタイマー
    private bool touchedByPlayer = false; // プレイヤーが踏んだかどうかのフラグ

    void Update()
    {
        timer += Time.deltaTime; // 毎フレーム、時間を加算

        // カメラのビューポート座標に変換（0〜1の範囲）
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);

        // ビューポート外に出たか確認
        bool isOutOfScreen = viewPos.x < 0 || viewPos.x > 1 || viewPos.y < 0 || viewPos.y > 1;

        // プレイヤーに触れられていない かつ 画面外に出た or 寿命超過
        if (!touchedByPlayer && (isOutOfScreen || timer >= lifeTime))
        {
            // 犬の好感度を減らす（放置したペナルティ）
            DogController dog = FindObjectOfType<DogController>(); // シーン上のDogControllerを検索
            if (dog != null)
            {
                dog.DecreaseAffection(10); // 好感度を10減少
            }

            Destroy(gameObject); // うんこオブジェクトを削除
        }
    }

    /// プレイヤーがうんこに触れたときの処理
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            touchedByPlayer = true; // プレイヤーに触れられたことを記録
            Destroy(gameObject);    // うんこを消す（回収成功）
        }
    }
}
