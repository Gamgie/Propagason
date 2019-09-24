using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatDetection : MonoBehaviour
{
    [SerializeField] bool debug;

    [Tooltip ("Buffer duration in frames for average computation")]
    [SerializeField] int samples = 1024;   // buffer duration in frames.

    [Tooltip("Value need to be higher than threshold to be considered as a beat")]
    [SerializeField, Range(0.0f, 1.0f)] float threshold = 1.0f;
    [SerializeField] BeatDetectionGraphDebug graphDebug;

    [Tooltip("Minimum time between 2 beats detection")]
    [SerializeField] float beatDetectionSleepTime;


    public float normalizedLevel { get; set; }  // normalized level received from audio Level Tracker component

    float[] normalizedLevelHistory;
    int actualIndex = 0;
    float avg = 0;
    float beatTimeStamp;

    private void Awake()
    {
        normalizedLevelHistory = new float[samples];

        beatTimeStamp = Time.fixedTime;
    }

    // Update is called once per frame
    void Update()
    {
        normalizedLevelHistory[actualIndex] = normalizedLevel;

        // Compute average
        float sum = 0.0f;
        for(int i = 0; i< normalizedLevelHistory.Length; i++)
        {
            sum += normalizedLevelHistory[i];
        }
        avg = sum / normalizedLevelHistory.Length;

        // Update GrapDebugClass
        graphDebug.UpdateGraph(avg, normalizedLevel);

        if(debug)
            Debug.Log("NormalizedLevel[" + actualIndex + "] = " + normalizedLevel + 
                      ". Actual AVG : " + avg);

        // beat detection
        /* 
         * To detect if it's a peak, we compare a centered value past to neighbours
         * windows size is the width of the comparison.
         * we first detect if next and previous are around centered value
         * then we check if extreme value of windows size are also around 
         * 
        */
        if (Time.fixedTime - beatTimeStamp > beatDetectionSleepTime)
        {
            int windowsSize = 5;
            int nleft = Mathf.Abs(((actualIndex - windowsSize * 2)+samples) % samples);
            int nPrevious = Mathf.Abs(((actualIndex - windowsSize - 1)+samples) % samples);
            int nNext = Mathf.Abs(((actualIndex - windowsSize + 1)+samples) % samples);
            int nCenter = Mathf.Abs(((actualIndex - windowsSize)+samples) % samples);
            if (normalizedLevelHistory[nPrevious] < normalizedLevelHistory[nCenter] &&
                normalizedLevelHistory[nCenter] > normalizedLevelHistory[nNext])
            {
                // we have a peak !
                // Check if it's a narrow peak
                if (normalizedLevelHistory[nleft] < normalizedLevelHistory[nCenter] &&
                normalizedLevelHistory[nCenter] > normalizedLevelHistory[actualIndex])
                {
                    // Check if it's high enough value
                    // and also if it's not a peak in the middle of a full speach.
                    if(normalizedLevelHistory[nCenter] > threshold && normalizedLevelHistory[nCenter] > avg*1.2f)
                    {
                        if(debug)
                            Debug.Log("Beat Detected at " + Time.fixedTime);
                        graphDebug.OnBeatDetected();
                        beatTimeStamp = Time.fixedTime;
                    }
                }
            } 
        }

        actualIndex++;
        actualIndex = actualIndex % samples;
    }

}
