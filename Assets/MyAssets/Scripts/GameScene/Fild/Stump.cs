using UnityEngine;

/// <summary>
/// ���F���؂̃X�N���v�g
/// �E�}�[�L���O�ς݂��ǂ����̃t���O���Ǘ�
/// �E�J�����ɉf�����猢���Ă�
/// �E�����}�[�L���O������ʒm������t���O�𗧂Ă�
/// </summary>
public class Stump : MonoBehaviour
{
    private bool hasBeenMarked = false; // �}�[�L���O�ς݂��ǂ���
    private bool hasSentDog = false;    // ���łɌ����Ă񂾂�

    private DogController dog;          // �V�[�����̌��ւ̎Q��

    void Start()
    {
        dog = Object.FindFirstObjectByType<DogController>();
    }

    void OnBecameVisible()
    {
        if (hasBeenMarked || hasSentDog || dog == null) return;

        // �������łɑ��̖؂Ɍ������Ă���Ȃ牽�����Ȃ�
        if (dog.IsDogBusy()) return;

        dog.GoToTarget(transform.position, this);
        hasSentDog = true;
    }


    /// <summary>
    /// ������̃}�[�L���O�����ʒm
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
    /// �F�����F�ɕς��ă}�[�L���O�ς݂����o��
    /// </summary>
    private void MarkingEffect()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = new Color(1f, 1f, 0f, 1f); // ���F
        }
    }

    public bool IsMarked() => hasBeenMarked;
}
