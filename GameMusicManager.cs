using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMusicManager : MonoBehaviour {

	//object need an AudioSource component (Obviously)
    public AudioClip[] musicList;
    public AudioSource audio;

    void Start()
    {
        PlayNextSong();
    }

	//waits until the current music ends and randomly starts playing an audioclip from the array 
    void PlayNextSong()
    {
        audio.clip = musicList[Random.Range(0, musicList.Length)];
        audio.Play();
        Invoke("PlayNextSong", audio.clip.length+1);
    }
}
