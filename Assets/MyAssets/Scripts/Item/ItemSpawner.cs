using UnityEngine;
using System.Collections.Generic;

public class ItemSpawner : MonoBehaviour
{
    [Tooltip("��������A�C�e���̃v���n�u�i�����j")]
    public List<GameObject> itemPrefabs;

    [Tooltip("�o���Ԋu�i�b�j")]
    public float spawnInterval = 1f;

    [Tooltip("�����ɑ��݂ł���A�C�e�����̏��")]
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

        // ��ʊO��Destroy���ꂽ�A�C�e�������X�g���珜�O
        activeItems.RemoveAll(item => item == null);
    }

    /// �A�C�e���������_���ɐ���
    void SpawnRandomItem()
    {
        if (itemPrefabs.Count == 0) return;

        int index = Random.Range(0, itemPrefabs.Count);
        GameObject prefab = itemPrefabs[index];

        GameObject newItem = Instantiate(prefab);
        activeItems.Add(newItem);
    }
}
