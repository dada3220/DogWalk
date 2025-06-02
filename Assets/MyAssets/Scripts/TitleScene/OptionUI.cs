using UnityEngine;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    [SerializeField] private Toggle bgmToggle;
    [SerializeField] private Toggle seToggle;

    private void Start()
    {
        if (SoundManager.Instance != null)
        {
            bgmToggle.isOn = SoundManager.Instance.IsBgmOn;
            seToggle.isOn = SoundManager.Instance.IsSeOn;
        }

        bgmToggle.onValueChanged.AddListener(OnBgmToggleChanged);
        seToggle.onValueChanged.AddListener(OnSeToggleChanged);
    }

    private void OnBgmToggleChanged(bool isOn)
    {
        SoundManager.Instance?.SetBgm(isOn);
    }

    private void OnSeToggleChanged(bool isOn)
    {
        SoundManager.Instance?.SetSe(isOn);
    }
}
