using UnityEngine;

/// <summary>
/// ���F���؂̃X�N���v�g
/// �E�}�[�L���O�ς݂��ǂ����̃t���O���Ǘ�
/// �E�J�����ɉf�����猢���Ă�
/// �E�����}�[�L���O������ʒm������t���O�𗧂Ă�
/// </summary>
public class YellowTree : MonoBehaviour
{
    private bool hasBeenMarked = false; // �}�[�L���O�ς݃t���O
    private bool hasSentDog = false;    // �����Ăяo�������ǂ���

    private DogController dog;          // �V�[�����̌��Q��

    void Start()
    {
        dog = Object.FindFirstObjectByType<DogController>(); // �V�[����̌���T��
    }

    /// <summary>
    /// �J�����ɉf�����Ƃ��Ă΂��Unity�W���R�[���o�b�N
    /// </summary>
    void OnBecameVisible()
    {
        // �܂��}�[�L���O����Ă��Ȃ��āA�����Ă�ł��Ȃ����
        if (!hasBeenMarked && !hasSentDog && dog != null)
        {
            dog.GoToTarget(transform.position, this); // �����Ă�
            hasSentDog = true;
        }
    }

    /// <summary>
    /// ������}�[�L���O�����ʒm���󂯂�
    /// </summary>
    public void OnDogArrived()
    {
        if (!hasBeenMarked)
        {
            hasBeenMarked = true;
            hasSentDog = false;

            Debug.Log("�؂��}�[�L���O����܂����I");

            // �}�[�L���O�ς݂̉��o�����s
            MarkingEffect();
        }
    }

    /// <summary>
    /// �}�[�L���O�ς݂̉��o�i�F�����F���ς���j
    /// </summary>
    private void MarkingEffect()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = new Color(1f, 1f, 0f, 1f); // ���F���ύX
        }
    }

    /// <summary>
    /// �}�[�L���O�ς݂��ǂ����擾�p
    /// </summary>
    public bool IsMarked()
    {
        return hasBeenMarked;
    }
}
