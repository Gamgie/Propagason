using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMngr : MonoBehaviour
{
    public enum NoteType { C, CS, D, DS, E, F, FS, G, GS, A, AS, B };

    //[Header("Sound Properties")]
    public int Pitch { get => _pitch;
        set
        {
            _pitch = value;
            Note = (NoteType)(_pitch%12);
        }
    }
    public float Frequency { get => _frequency; set => _frequency = value; }
    public NoteType Note { get => _note; set => _note = value; }
    public float Volume
    {
        get
        {
            return  Remap(_volume, volumeThreshold, 1, 0, 1);
        }
        set
        {
            if (value >= volumeThreshold)
                _volume = value;
        }
    }

    public float VolumeThreshold { get => volumeThreshold; set => volumeThreshold = value; }


    [SerializeField] private int _pitch;
    [SerializeField] private float _frequency;
    [SerializeField] private NoteType _note;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float _volume;

    public float volumeThreshold = 0.1f;
    int[] pitchHistory;
    int pitchHistoryIndex;
    int pitchHistorySamples = 64;
    float pitchAverage = 0.0f;

    private void Awake()
    {
        pitchHistory = new int[pitchHistorySamples];
        pitchHistoryIndex = 0;

        LoadPlayerPref();

        _volume = volumeThreshold;
    }

    private void Update()
    {
        // Add pitch value to history
        pitchHistory[pitchHistoryIndex%pitchHistorySamples] = Pitch;
        pitchHistoryIndex++;
        pitchHistoryIndex = pitchHistoryIndex%pitchHistorySamples;

        int sum = 0;
        foreach(int i in pitchHistory)
        {
            sum += i;
        }
        pitchAverage = sum / pitchHistorySamples;
    }


    public float GetPitchAverage()
    {
        return pitchAverage;
    }

    public NoteType GetNoteAverage()
    {
        return (NoteType)(pitchAverage % 12);
    }

    public float Remap(float value, float from1, float to1, float from2, float to2)
    {

        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;

    }

    public void PlayAmbiantMusic(int playMusic)
    {
        AudioSource audioSource = GetComponent<AudioSource>();

        if(!audioSource.isPlaying && playMusic == 1)
        {
            audioSource.Play();
        }
        else if(playMusic == 0)
        {
            audioSource.Stop();

        }
        
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.SetFloat("volumeThreshold", volumeThreshold);
    }

    void LoadPlayerPref()
    {
        if (PlayerPrefs.GetFloat("volumeThreshold", -100) != -100)
            volumeThreshold = PlayerPrefs.GetFloat("volumeThreshold");
    }

}
