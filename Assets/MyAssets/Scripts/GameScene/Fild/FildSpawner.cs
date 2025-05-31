using UnityEngine;
using System.Collections.Generic;

// ���b���Ƃɏ㉺���E��1���A�C�e�����o��������X�N���v�g
public class FildSpawner : MonoBehaviour
{
    [Tooltip("���������Q���̃v���n�u")]
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
            int spawnableCount = maxItems - activeItems.Count;

            if (spawnableCount >= 4) // �Œ�4�X�|�[������]�T������Ƃ��̂�
            {
                SpawnItemInAllDirections();
            }

            timer = 0f;
        }

        // �����ȁiDestroy���ꂽ�j�A�C�e�������X�g���珜��
        activeItems.RemoveAll(item => item == null);

        // �J�������痣�ꂽ�A�C�e�����폜
        CullOffscreenItems();
    }

    /// �㉺���E��1����Q���𐶐�
    void SpawnItemInAllDirections()
    {
        for (int side = 0; side < 4; side++) // 0:��, 1:��, 2:��, 3:�E
        {
            Vector3 spawnPos = GetSpawnPositionBySide(side);
            GameObject prefab = GetRandomPrefab();
            if (prefab != null)
            {
                // �d�Ȃ�`�F�b�N
                if (!IsPositionOccupied(spawnPos, 0.5f)) // ���a0.5�͈̔͂�Collider������ΐ������Ȃ�
                {
                    GameObject newItem = Instantiate(prefab, spawnPos, Quaternion.identity);
                    activeItems.Add(newItem);
                }
            }
        }
    }

    /// �d�Ȃ�`�F�b�N
    bool IsPositionOccupied(Vector2 position, float checkRadius)
    {
        Collider2D hit = Physics2D.OverlapCircle(position, checkRadius);
        return hit != null;
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

    /// �����_���ȃA�C�e���v���n�u���擾
    GameObject GetRandomPrefab()
    {
        if (itemPrefabs.Count == 0) return null;
        int index = Random.Range(0, itemPrefabs.Count);
        return itemPrefabs[index];
    }

    /// ��ʂ���2��ʕ��ȏ㗣�ꂽ�A�C�e�����폜
    void CullOffscreenItems()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null) return;

        Vector3 camPos = mainCamera.transform.position;
        float height = 2f * mainCamera.orthographicSize;
        float width = height * mainCamera.aspect;

        float cullWidth = width * 2f;   // ��2��ʕ�
        float cullHeight = height * 2f; // �c2��ʕ�

        for (int i = activeItems.Count - 1; i >= 0; i--)
        {
            GameObject item = activeItems[i];
            if (item == null) continue;

            Vector3 pos = item.transform.position;
            if (Mathf.Abs(pos.x - camPos.x) > cullWidth / 2f ||
                Mathf.Abs(pos.y - camPos.y) > cullHeight / 2f)
            {
                Destroy(item);
                activeItems.RemoveAt(i);
            }
        }
    }
}
