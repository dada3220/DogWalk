using UnityEngine;
using System.Collections.Generic;

// 毎秒ごとに上下左右に1つずつアイテムを出現させるスクリプト
public class ItemSpawner : MonoBehaviour
{
    [Tooltip("生成するアイテムのプレハブ")]
    public List<GameObject> itemPrefabs;

    [Tooltip("出現間隔（秒）")]
    public float spawnInterval;

    [Tooltip("同時に存在できるアイテム数の上限")]
    public int maxItems;

    [Tooltip("スポーン時に画面外へどのくらい余白を取るか")]
    public float spawnMargin;

    private float timer = 0f;
    private List<GameObject> activeItems = new List<GameObject>();

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            // 現在の残りスポーン可能数
            int spawnableCount = maxItems - activeItems.Count;

            if (spawnableCount >= 4) // 最低4個同時に生成
            {
                SpawnItemInAllDirections();
            }

            timer = 0f;
        }

        // 破棄されたアイテムをリストから除外
        activeItems.RemoveAll(item => item == null);
    }

    /// 上下左右に1つずつアイテムを生成
    void SpawnItemInAllDirections()
    {
        for (int side = 0; side < 4; side++) // 0:上, 1:下, 2:左, 3:右
        {
            Vector3 spawnPos = GetSpawnPositionBySide(side);
            GameObject prefab = GetRandomPrefab();
            if (prefab != null)
            {
                GameObject newItem = Instantiate(prefab, spawnPos, Quaternion.identity);
                activeItems.Add(newItem);
            }
        }
    }

    /// 指定された方向に対応したスポーン位置を返す
    Vector3 GetSpawnPositionBySide(int side)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null) return Vector3.zero;

        Vector3 camPos = mainCamera.transform.position;
        float height = 2f * mainCamera.orthographicSize;
        float width = height * mainCamera.aspect;

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

    // ランダムなアイテムプレハブを取得
    GameObject GetRandomPrefab()
    {
        if (itemPrefabs.Count == 0) return null;
        int index = Random.Range(0, itemPrefabs.Count);
        return itemPrefabs[index];
    }
}
