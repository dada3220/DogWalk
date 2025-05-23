using UnityEngine;
using System.Collections.Generic;

public class ItemSpawner : MonoBehaviour
{
    [Tooltip("生成するアイテムのプレハブ（複数可）")]
    public List<GameObject> itemPrefabs;

    [Tooltip("出現間隔（秒）")]
    public float spawnInterval = 1f;

    [Tooltip("同時に存在できるアイテム数の上限")]
    public int maxItems = 10;

    private float timer = 0f;

    private List<GameObject> activeItems = new List<GameObject>();

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval && activeItems.Count < maxItems)
        {
            SpawnRandomItem();
            timer = 0f;
        }

        // 画面外でDestroyされたアイテムをリストから除外
        activeItems.RemoveAll(item => item == null);
    }

    /// アイテムをランダムに生成
    void SpawnRandomItem()
    {
        if (itemPrefabs.Count == 0) return;

        int index = Random.Range(0, itemPrefabs.Count);
        GameObject prefab = itemPrefabs[index];

        GameObject newItem = Instantiate(prefab);
        activeItems.Add(newItem);
    }
}
