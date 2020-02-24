﻿using System.Collections;
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
    public float Volume { get => _volume; set => _volume = value; }

    [SerializeField] private int _pitch;
    [SerializeField] private float _frequency;
    [SerializeField] private NoteType _note;
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
    }

    private void Update()
    {
        if (Volume < volumeThreshold) Volume = 0;

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
}
