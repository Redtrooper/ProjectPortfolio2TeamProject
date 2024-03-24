using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonSoundEffects : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public AudioClip hoverSound; 
    public AudioClip clickSound; 

    private AudioSource audioSource;
    private Slider sfxVolumeSlider;

    void Start()
    {
        audioSource = GetComponentInParent<AudioSource>();
        if (audioSource == null)
        {
            // this may need testing because it is pullin null
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        sfxVolumeSlider = GameObject.FindObjectOfType<SoundManager>().sfxVolumeSlider;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSound != null)
        {
            PlaySound(hoverSound);
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickSound != null)
        {
            PlaySound(clickSound);
        }
    }
    private void PlaySound(AudioClip sound)
    {
        if (audioSource != null && sound != null && sfxVolumeSlider != null)
        {
            audioSource.PlayOneShot(sound, sfxVolumeSlider.value);
        }
    }
}
