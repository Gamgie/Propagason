using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
public class TimelineMngr : MonoBehaviour
{
    private PlayableDirector _director;

    [SerializeField] int beatCount = 3;
    [SerializeField] float beatDurationDetection = 3.0f;

    int beat = 0;
    float firstBeatTimestamp = 0.0f;

    private void Awake()
    {
        _director = GetComponent<PlayableDirector>();
    }

    public void PlayCameraPanning()
    {
        _director.Play();
    }

    public void OnBeatDetected(float timeStamp)
    {
        beat++;

        if(beat == 1)
        {
            firstBeatTimestamp = timeStamp;
        }

        if(Time.fixedTime - firstBeatTimestamp >= beatDurationDetection)
        {
            beat = 0;
            return;
        }

        if(beat == beatCount)
        {
            PlayCameraPanning();
        }


    }
}
