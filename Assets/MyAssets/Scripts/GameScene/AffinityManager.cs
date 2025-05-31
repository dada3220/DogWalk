using UnityEngine;
using UnityEngine.UI;

public class AffinityManager : MonoBehaviour
{
    public static AffinityManager Instance { get; private set; }

    public int affection = 100;
    public int maxAffection = 100;

    [SerializeField] private Slider affectionSlider;

    public System.Action OnAffectionDepleted; // �O���ʒm�p�C�x���g

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (affectionSlider != null)
        {
            affectionSlider.maxValue = maxAffection;
            affectionSlider.value = affection;
        }
    }

    public void IncreaseAffection(int amount)
    {
        affection += amount;
        affection = Mathf.Clamp(affection, 0, maxAffection);
        UpdateSlider();
    }

    public void DecreaseAffection(int amount)
    {
        affection -= amount;
        affection = Mathf.Clamp(affection, 0, maxAffection);
        UpdateSlider();

        if (affection <= 0)
        {
            // �C�x���g�Œʒm�i�����͊O���ɔC����j
            OnAffectionDepleted?.Invoke();
        }
    }

    private void UpdateSlider()
    {
        if (affectionSlider != null)
        {
            affectionSlider.value = affection;
        }
    }

    public int GetAffection() => affection;
}
