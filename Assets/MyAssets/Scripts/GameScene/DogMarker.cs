using UnityEngine;
using Cysharp.Threading.Tasks;
using System;


/// <summary>
/// ���̃}�[�L���O�s���𐧌䂷��N���X
/// </summary>
public class DogMarker
{
    private readonly MonoBehaviour owner;     // �񓯊������p�iUniTask�ŕK�v�j
    private readonly Animator animator;       // �A�j���[�^�[�ւ̎Q��
    private Stump currentTree = null;         // ���݃^�[�Q�b�g�ɂ��Ă���؂芔
    private bool isMarking = false;           // ���݃}�[�L���O�����ǂ���
    private float markingTime = 2f;           // �}�[�L���O�ɂ����鎞�ԁi�b�j
    private float markingTimer = 0f;          // ���݂̃}�[�L���O�o�߃^�C�}�[

    public bool IsMarking => isMarking;       // �O������}�[�L���O��Ԃ��擾
    public Stump CurrentTree => currentTree;  // ���݂̑ΏۂƂȂ�؂��擾
                                             
    public event Action OnMarkingFinished;     


    public DogMarker(MonoBehaviour owner, Animator animator)
    {
        this.owner = owner;
        this.animator = animator;
    }

    // �^�[�Q�b�g��Setter
    public void SetTarget(Stump stump)
    {
        if (stump == null || stump.IsMarked()) return;
        currentTree = stump;
    }


    /// <summary>
    /// �}�[�L���O�����̊J�n�����݂�i�}�[�L���O�ς݂⌻�݃}�[�L���O���̏ꍇ�͖����j
    /// </summary>
    public void TryStartMarking(Stump stump)
    {
        if (stump == null || stump.IsMarked() || isMarking) return;

        currentTree = stump;
        isMarking = true;
        markingTimer = markingTime;

        animator?.Play("dogmarking"); // �}�[�L���O�A�j���[�V�����J�n

        // �񓯊��Ń}�[�L���O����������ҋ@
        CountdownMarking().Forget();
    }

    /// <summary>
    /// �}�[�L���O�����܂ł̑ҋ@�����i�񓯊��j
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
    /// �}�[�L���O�������̏���
    /// </summary>
    private void FinishMarking()
    {
        isMarking = false;
        currentTree?.OnDogArrived();
        currentTree = null;
        OnMarkingFinished?.Invoke(); // �� �C�x���g�ʒm
    }
}
