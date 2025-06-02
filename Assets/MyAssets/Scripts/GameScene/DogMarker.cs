using UnityEngine;
using Cysharp.Threading.Tasks;
using System;


/// <summary>
/// 犬のマーキング行動を制御するクラス
/// </summary>
public class DogMarker
{
    private readonly MonoBehaviour owner;     // 非同期処理用（UniTaskで必要）
    private readonly Animator animator;       // アニメーターへの参照
    private Stump currentTree = null;         // 現在ターゲットにしている切り株
    private bool isMarking = false;           // 現在マーキング中かどうか
    private float markingTime = 2f;           // マーキングにかかる時間（秒）
    private float markingTimer = 0f;          // 現在のマーキング経過タイマー

    public bool IsMarking => isMarking;       // 外部からマーキング状態を取得
    public Stump CurrentTree => currentTree;  // 現在の対象となる木を取得
                                             
    public event Action OnMarkingFinished;     


    public DogMarker(MonoBehaviour owner, Animator animator)
    {
        this.owner = owner;
        this.animator = animator;
    }

    // ターゲットのSetter
    public void SetTarget(Stump stump)
    {
        if (stump == null || stump.IsMarked()) return;
        currentTree = stump;
    }


    /// <summary>
    /// マーキング処理の開始を試みる（マーキング済みや現在マーキング中の場合は無視）
    /// </summary>
    public void TryStartMarking(Stump stump)
    {
        if (stump == null || stump.IsMarked() || isMarking) return;

        currentTree = stump;
        isMarking = true;
        markingTimer = markingTime;

        animator?.Play("dogmarking"); // マーキングアニメーション開始

        // 非同期でマーキング完了処理を待機
        CountdownMarking().Forget();
    }

    /// <summary>
    /// マーキング完了までの待機処理（非同期）
    /// </summary>
    private async UniTaskVoid CountdownMarking()
    {
        while (markingTimer > 0f)
        {
            await UniTask.Yield();
            markingTimer -= Time.deltaTime;
        }

        FinishMarking();
    }

    /// <summary>
    /// マーキング完了時の処理
    /// </summary>
    private void FinishMarking()
    {
        isMarking = false;
        currentTree?.OnDogArrived();
        currentTree = null;
        OnMarkingFinished?.Invoke(); // ← イベント通知
    }
}
