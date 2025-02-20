using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] AudioSource sfxAs;

    private void Awake()
    {
        Instance = this;
    }

    public void PlaySFX(SFXData sfxData)
    {
        sfxAs.pitch = sfxData.GetPitch;
        sfxAs.volume = sfxData.GetVolume;
        sfxAs.PlayOneShot(sfxData.GetAudioClip);
    }
}
