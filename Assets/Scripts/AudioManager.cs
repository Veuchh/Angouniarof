using UnityEngine;
using DG.Tweening;
using System.Numerics;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] AudioSource sfxAs;
    [SerializeField] AudioSource chillMusicAs;
    [SerializeField] AudioSource gameMusicAs;
    [SerializeField] float fadeMusicDuration = 1;

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

    public void Play(SFXData sfxData)
    {
        if(sfxAs.isPlaying)
            return;
        sfxAs.pitch = sfxData.GetPitch;
        sfxAs.volume = sfxData.GetVolume;
        sfxAs.clip = sfxData.GetAudioClip;
        sfxAs.Play();
    }

    public void Stop()
    {
        sfxAs.Stop();
    }

    public void ChangeMusicType(bool isInGame)
    {
        DOTween.To(() => chillMusicAs.volume, x => chillMusicAs.volume = x, isInGame ? 0 : 1, fadeMusicDuration);
        DOTween.To(() => gameMusicAs.volume, x => gameMusicAs.volume = x, isInGame ? 1 : 0, fadeMusicDuration);
    }
}
