using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name; // Name of the sound category
    public AudioClip[] clips; // Array of audio clips for the sound category
    [Range(0f, 1f)]
    public float volume = 1f; // Volume level for the sound category
}
