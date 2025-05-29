using UnityEngine;

/// <summary>
/// 黄色い木のスクリプト
/// ・マーキング済みかどうかのフラグを管理
/// ・カメラに映ったら犬を呼ぶ
/// ・犬がマーキング完了を通知したらフラグを立てる
/// </summary>
public class YellowTree : MonoBehaviour
{
    private bool hasBeenMarked = false; // マーキング済みフラグ
    private bool hasSentDog = false;    // 犬を呼び出したかどうか

    private DogController dog;          // シーン内の犬参照

    void Start()
    {
        dog = Object.FindFirstObjectByType<DogController>(); // シーン上の犬を探す
    }

    /// <summary>
    /// カメラに映ったとき呼ばれるUnity標準コールバック
    /// </summary>
    void OnBecameVisible()
    {
        // まだマーキングされていなくて、犬を呼んでいなければ
        if (!hasBeenMarked && !hasSentDog && dog != null)
        {
            dog.GoToTarget(transform.position, this); // 犬を呼ぶ
            hasSentDog = true;
        }
    }

    /// <summary>
    /// 犬からマーキング完了通知を受ける
    /// </summary>
    public void OnDogArrived()
    {
        if (!hasBeenMarked)
        {
            hasBeenMarked = true;
            hasSentDog = false;

            Debug.Log("木がマーキングされました！");

            // マーキング済みの演出を実行
            MarkingEffect();
        }
    }

    /// <summary>
    /// マーキング済みの演出（色を黄色く変える）
    /// </summary>
    private void MarkingEffect()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = new Color(1f, 1f, 0f, 1f); // 黄色く変更
        }
    }

    /// <summary>
    /// マーキング済みかどうか取得用
    /// </summary>
    public bool IsMarked()
    {
        return hasBeenMarked;
    }
}
