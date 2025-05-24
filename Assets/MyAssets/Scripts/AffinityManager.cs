using UnityEngine;
using UnityEngine.UI;

public class AffinityManager : MonoBehaviour
{
    public static AffinityManager Instance { get; private set; }

    public int affection = 100; // ���݂̍D���x
    public int maxAffection = 100; // �D���x�̍ő�l

    public Slider affectionSlider; // UI�X���C�_�[�ւ̎Q��

    private void Awake()
    {
        // �V���O���g���ݒ�
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        // �X���C�_�[������
        if (affectionSlider != null)
        {
            affectionSlider.maxValue = maxAffection;
            affectionSlider.value = affection;
        }
    }

    // �D���x����
    public void IncreaseAffection(int amount)
    {
        affection += amount;
        affection = Mathf.Clamp(affection, 0, maxAffection);
        UpdateSlider();
    }

    // �D���x�����i0�ȉ��Ȃ�Q�[���I�[�o�[�j
    public void DecreaseAffection(int amount)
    {
        affection -= amount;
        affection = Mathf.Clamp(affection, 0, maxAffection);
        UpdateSlider();

        if (affection <= 0)
        {
            GameOver();
        }
    }

    // UI�X�V
    private void UpdateSlider()
    {
        if (affectionSlider != null)
        {
            affectionSlider.value = affection;
        }
    }

    // �Q�[���I�[�o�[�����i�D���x0�j
    private void GameOver()
    {
        Debug.Log("Game Over! Dog lost trust.");
        // ����̊g����UI��~��J�ڒǉ���
    }

    // �O������D���x���擾
    public int GetAffection() => affection;
}
