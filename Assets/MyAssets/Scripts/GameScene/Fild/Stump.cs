using UnityEngine;

/// <summary>
/// 黄色い木のスクリプト
/// ・マーキング済みかどうかのフラグを管理
/// ・カメラに映ったら犬を呼ぶ
/// ・犬がマーキング完了を通知したらフラグを立てる
/// </summary>
public class Stump : MonoBehaviour
{
    private bool hasBeenMarked = false; // マーキング済みかどうか
    private bool hasSentDog = false;    // すでに犬を呼んだか

    private DogController dog;          // シーン内の犬への参照

    void Start()
    {
        dog = Object.FindFirstObjectByType<DogController>();
    }

    void OnBecameVisible()
    {
        if (hasBeenMarked || hasSentDog || dog == null) return;

        // 犬がすでに他の木に向かっているなら何もしない
        if (dog.IsDogBusy()) return;

        dog.GoToTarget(transform.position, this);
        hasSentDog = true;
    }


    /// <summary>
    /// 犬からのマーキング完了通知
    /// </summary>
    public void OnDogArrived()
    {
        if (!hasBeenMarked)
        {
            hasBeenMarked = true;

            MarkingEffect();
        }
    }

    /// <summary>
    /// 色を黄色に変えてマーキング済みを視覚化
    /// </summary>
    private void MarkingEffect()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = new Color(1f, 1f, 0f, 1f); // 黄色
        }
    }

    public bool IsMarked() => hasBeenMarked;
}
