using UnityEngine;

// アイテムの基本クラス。プレイヤーや犬と接触時の動作を定義するための抽象クラス。

public abstract class Item : MonoBehaviour
{
    protected Transform player;
    protected Transform dog;
    protected Camera mainCamera;

    protected float boundaryMargin = 3f; // 画面の2枠外に出たら自動的に消去される

    protected virtual void Start()
    {
        mainCamera = Camera.main;
        player = GameObject.FindWithTag("Player")?.transform;
        dog = GameObject.FindWithTag("Dog")?.transform;

    }

    protected virtual void Update()
    {
        // 画面外（2枠以上外）に出たらオブジェクトを破棄
        if (IsOutOfBounds())
        {
            Destroy(gameObject);
        }
    }

    // カメラの表示範囲より2枠分外側に出たかどうかをチェック
    protected bool IsOutOfBounds()
    {
        Vector3 camPos = mainCamera.transform.position;
        float height = 2f * mainCamera.orthographicSize;
        float width = height * mainCamera.aspect;

        float minX = camPos.x - width / 2 - boundaryMargin;
        float maxX = camPos.x + width / 2 + boundaryMargin;
        float minY = camPos.y - height / 2 - boundaryMargin;
        float maxY = camPos.y + height / 2 + boundaryMargin;

        Vector3 pos = transform.position;

        return (pos.x < minX || pos.x > maxX || pos.y < minY || pos.y > maxY);
    }

    // プレイヤーがこのアイテムを取得した時の処理（継承クラスで実装）
    protected abstract void OnPlayerCollect();

    // 犬がこのアイテムを取得した時の処理（継承クラスで実装）
    protected abstract void OnDogCollect();

    // 他のオブジェクトとのトリガー接触処理
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerCollect();
            Destroy(gameObject);
        }
        else if (other.CompareTag("Dog") && other.GetComponent<DogController>() != null)
        {
            OnDogCollect();
            Destroy(gameObject);
        }
    }
}
