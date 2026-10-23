using UnityEngine;

public class GameOverSound : MonoBehaviour
{
    [SerializeField] private AudioClip gameOverClip;
    [SerializeField] private AudioSource musicSource;
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        if (musicSource != null)
            musicSource.Stop();

        if (_audioSource != null && gameOverClip != null)
            _audioSource.PlayOneShot(gameOverClip);
    }
}