using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSEPlayer : MonoBehaviour
{
    [SerializeField] private string seName = "click";

    private void Awake()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(PlaySE);
    }

    private void PlaySE()
    {
        SEManager.Instance?.Play(seName);
    }
}
