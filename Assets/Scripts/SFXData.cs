using NaughtyAttributes;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

[CreateAssetMenu(fileName = "SFXData", menuName = "Scriptable Objects/SFXData")]
public class SFXData : ScriptableObject
{
    [SerializeField]    bool pickFromRandomList = false;
    [HideIf(nameof(pickFromRandomList))]
    [SerializeField] AudioClip audioClip;
    [ShowIf(nameof(pickFromRandomList))]
    [SerializeField] List<AudioClip> randomAudioClipList;
    [Space]
    [SerializeField] float volume = 1f;
    [Space]
    [SerializeField] bool randomizePitch = false;
    [ShowIf(nameof(randomizePitch))]
    [SerializeField, MinMaxRangeSlider(0f,2f)] Vector2 minMaxPitch = Vector2.one;

    public AudioClip GetAudioClip=>
         pickFromRandomList 
            ? randomAudioClipList[Random.Range(0, randomAudioClipList.Count)] 
            : audioClip;

    public float GetVolume => volume;
    public float GetPitch => 
        randomizePitch 
        ? Random.Range(minMaxPitch.x, minMaxPitch.y) 
        : 1;
}