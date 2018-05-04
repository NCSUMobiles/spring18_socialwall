using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHolder : MonoBehaviour {

    public List<AudioClip> audio;

    public List<AudioClip> GetAudio()
    {
        return audio;
    }

    public AudioClip GetRandomAudio()
    {
        return audio[Random.Range(0, audio.Count)];
    }
}
