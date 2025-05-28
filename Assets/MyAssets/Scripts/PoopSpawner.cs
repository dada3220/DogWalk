using UnityEngine;

public class PoopSpawner : MonoBehaviour
{
    // �v���n�u���C���X�y�N�^�[�Őݒ�
    public GameObject poopPrefab;

    /// <summary>
    /// �w�肳�ꂽ�ʒu�ɂ��񂿂𐶐�����֐�
    /// </summary>
    /// <param name="position">��������ʒu</param>
    public void SpawnPoop(Vector2 position)
    {
        if (poopPrefab == null)
        {
            Debug.LogWarning("Poop prefab is not assigned to PoopSpawner!");
            return;
        }

        // �v���n�u���w��ʒu�ɐ���
        GameObject poop = Instantiate(poopPrefab, position, Quaternion.identity);

        // 10�b��Ɏ����폜
        Destroy(poop, 10f);
    }
}
