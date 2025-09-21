using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioClip gameIntro;
    public AudioClip ghostNormal;

    public AudioSource audioSource;
    void Start()
    {
        float waitTime = Mathf.Min(gameIntro.length, 3f);
        Invoke(nameof(PlayNormalMusic), waitTime);
    }

    // Update is called once per frame
    void PlayNormalMusic()
    {
        audioSource.clip = ghostNormal;
        audioSource.loop = true;
        audioSource.Play();
    }
}