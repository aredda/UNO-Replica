using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager 
    : Manager
{
    [Header("Audio Settings")]
    public AudioSource audioSource;

    [Header("Draw Card Sounds")]
    public List<AudioClip> drawClips;

    [Header("Set Card Sounds")]
    public List<AudioClip> setClips;

    public void PlayDrawSound()
    {
        audioSource.PlayOneShot(GetRandomClip(drawClips));
    }

    public void PlaySetSound()
    {
        audioSource.PlayOneShot(GetRandomClip(setClips));
    }

    public AudioClip GetRandomClip(List<AudioClip> from)
    {
        if(from == null)
            throw new System.Exception("AudioManager.GetRandomClip#Exception: the source list is missing");

        if(from.Count == 0)
            throw new System.Exception("AudioManager.GetRandomClip#Exception: the source list is empty");

        return from[Random.Range(0, from.Count)];
    }
}
