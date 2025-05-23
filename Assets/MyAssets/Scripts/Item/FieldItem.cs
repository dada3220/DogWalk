using UnityEngine;

public abstract class FieldItem : MonoBehaviour
{
    protected Transform player;
    protected Transform dog;
    protected Camera mainCamera;

    protected float boundaryMargin = 3f; // 2枠外に出たら消す
    protected float spawnMargin = 1f; // 1枠外で出現

    protected virtual void Start()
    {
        mainCamera = Camera.main;
        player = GameObject.FindWithTag("Player")?.transform;
        dog = GameObject.FindWithTag("Dog")?.transform;

        // ランダム位置にスポーン
        transform.position = GetRandomSpawnPosition();
    }

    protected virtual void Update()
    {
        // 画面外判定：2枠外に出たら破棄
        if (IsOutOfBounds())
        {
            Destroy(gameObject);
        }
    }

    /// ランダムに1枠外に出現する位置を返す
    protected Vector3 GetRandomSpawnPosition()
    {
        Vector3 camPos = mainCamera.transform.position;
        float height = 2f * mainCamera.orthographicSize;
        float width = height * mainCamera.aspect;

        // 外周のどこかを選ぶ
        int side = Random.Range(0, 4);
        float x = 0f, y = 0f;

        switch (side)
        {
            case 0: // 上
                x = Random.Range(-width / 2, width / 2);
                y = height / 2 + spawnMargin;
                break;
            case 1: // 下
                x = Random.Range(-width / 2, width / 2);
                y = -height / 2 - spawnMargin;
                break;
            case 2: // 左
                x = -width / 2 - spawnMargin;
                y = Random.Range(-height / 2, height / 2);
                break;
            case 3: // 右
                x = width / 2 + spawnMargin;
                y = Random.Range(-height / 2, height / 2);
                break;
        }

        return new Vector3(camPos.x + x, camPos.y + y, 0f);
    }

    // 画面の2枠外に出ているか判定
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

    // プレイヤーと接触したときの動作（継承先で定義）
    protected abstract void OnPlayerCollect();

    // 犬と接触したときの動作（継承先で定義）
    protected abstract void OnDogCollect();

    // トリガー接触処理
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerCollect();
            Destroy(gameObject);
        }
        else if (other.GetComponent<DogController>() != null)
        {
            OnDogCollect();
            Destroy(gameObject);
        }
    }
}
