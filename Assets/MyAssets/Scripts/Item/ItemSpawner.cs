using UnityEngine;
using System.Collections.Generic;

// ���b���Ƃɏ㉺���E��1���A�C�e�����o��������X�N���v�g
public class ItemSpawner : MonoBehaviour
{
    [Tooltip("��������A�C�e���̃v���n�u")]
    public List<GameObject> itemPrefabs;

    [Tooltip("�o���Ԋu�i�b�j")]
    public float spawnInterval;

    [Tooltip("�����ɑ��݂ł���A�C�e�����̏��")]
    public int maxItems;

    [Tooltip("�X�|�[�����ɉ�ʊO�ւǂ̂��炢�]������邩")]
    public float spawnMargin;

    private float timer = 0f;
    private List<GameObject> activeItems = new List<GameObject>();

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            // ���݂̎c��X�|�[���\��
            int spawnableCount = maxItems - activeItems.Count;

            if (spawnableCount >= 4) // �Œ�4�����ɐ���
            {
                SpawnItemInAllDirections();
            }

            timer = 0f;
        }

        // �j�����ꂽ�A�C�e�������X�g���珜�O
        activeItems.RemoveAll(item => item == null);
    }

    /// �㉺���E��1���A�C�e���𐶐�
    void SpawnItemInAllDirections()
    {
        for (int side = 0; side < 4; side++) // 0:��, 1:��, 2:��, 3:�E
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

    /// �w�肳�ꂽ�����ɑΉ������X�|�[���ʒu��Ԃ�
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
            case 0: // ��
                x = Random.Range(-width / 2, width / 2);
                y = height / 2 + spawnMargin;
                break;
            case 1: // ��
                x = Random.Range(-width / 2, width / 2);
                y = -height / 2 - spawnMargin;
                break;
            case 2: // ��
                x = -width / 2 - spawnMargin;
                y = Random.Range(-height / 2, height / 2);
                break;
            case 3: // �E
                x = width / 2 + spawnMargin;
                y = Random.Range(-height / 2, height / 2);
                break;
        }

        return new Vector3(camPos.x + x, camPos.y + y, 0f);
    }

    // �����_���ȃA�C�e���v���n�u���擾
    GameObject GetRandomPrefab()
    {
        if (itemPrefabs.Count == 0) return null;
        int index = Random.Range(0, itemPrefabs.Count);
        return itemPrefabs[index];
    }
}
