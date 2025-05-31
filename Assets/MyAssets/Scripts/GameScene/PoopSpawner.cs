using UnityEngine;

public class PoopSpawner : MonoBehaviour
{
    // プレハブをインスペクターで設定
    public GameObject poopPrefab;

    /// <summary>
    /// 指定された位置にうんちを生成する関数
    /// </summary>
    /// <param name="position">生成する位置</param>
    public void SpawnPoop(Vector2 position)
    {
        if (poopPrefab == null)
        {
            Debug.LogWarning("Poop prefab is not assigned to PoopSpawner!");
            return;
        }

        // プレハブを指定位置に生成
        GameObject poop = Instantiate(poopPrefab, position, Quaternion.identity);

        // 10秒後に自動削除
        Destroy(poop, 10f);
    }
}
