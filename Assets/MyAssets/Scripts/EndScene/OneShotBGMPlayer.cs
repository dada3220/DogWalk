using UnityEngine;

public class OneShotBGMPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip bgmClip;
    [SerializeField] private bool playOnStart = true;

    private void Start()
    {
        if (playOnStart && bgmClip != null)
        {
            var source = gameObject.AddComponent<AudioSource>();
            source.clip = bgmClip;
            source.loop = false;
            source.Play();
        }
    }
}
