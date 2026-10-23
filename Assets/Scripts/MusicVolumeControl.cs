using UnityEngine;
using UnityEngine.UI;

public class MusicVolumeControl : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private Slider volumeSlider;

    private void Start()
    {
        if (musicSource != null && volumeSlider != null)
        {
            volumeSlider.value = musicSource.volume * 100f;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    public void SetVolume(float value)
    {
        if (musicSource != null)
            musicSource.volume = value / 100f;
    }
}