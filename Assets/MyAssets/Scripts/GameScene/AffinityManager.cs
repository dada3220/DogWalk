using UnityEngine;
using UnityEngine.UI;

public class AffinityManager : MonoBehaviour
{
    public static AffinityManager Instance { get; private set; }

    public int affection = 100;
    public int maxAffection = 100;

    [SerializeField] private Slider affectionSlider;

    public System.Action OnAffectionDepleted; // 外部通知用イベント

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
            // イベントで通知（処理は外部に任せる）
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
