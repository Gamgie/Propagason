using Reaktion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropagasonMngr : MonoBehaviour
{
    public bool debug;

    [Header("Wave Settings")]
    public KeyCode launchOneWaveKeyCode;
    public float waveSpeed;
    [Range(0.1f, 3.0f)]
    public float wavelength;
    [Range(0.0f, 5.0f)]
    public float maxEmissionRingValue;
    //[Space(20)]

    [Header("External Links")]
    [SerializeField]
    private GameObject _soundRingParent;
    private SoundRingBe[] soundRingArray;

    [Header("Sound Reactive Settings")]
    public ReaktorLink reaktor;
    public Trigger soundBurst;
    [Range(0,1f)]
    public float soundVolumeThreshold;

    // To avoid sending a black wave for infinite time 
    // I save the last lit time stamp. If no wave is send for a second means, we are back to silence.
    float litTimeStamp = 0.0f;

    private void Awake()
    {
        soundRingArray = _soundRingParent.GetComponentsInChildren<SoundRingBe>();

        // Init reaktor component
        reaktor.Initialize(this);
    }

    // Update is called once per frame
    void Update()
    {
        // If we receive a keyboard event, send a sound wave
        if (Input.GetKeyUp(launchOneWaveKeyCode))
        {
            LaunchWave();
        }
        // if it's a sound peak, send a wave too.
        // Next step : add reaktor parameter to the wave.
        else if (soundBurst.Update(reaktor.Output))
        {
            LaunchWave();
        }
        else if(reaktor.Output > soundVolumeThreshold)
        {
            float outputRemap = reaktor.Output;
            // if threshold is not 0, we want to remap reaktor.output between 0 and 1
            // to ensure the functionning of the wave. If not the minimum emissive value will be the 
            // sound volume threshold.
            // ref : https://forum.unity.com/threads/re-map-a-number-from-one-range-to-another.119437/
            if (soundVolumeThreshold != 0)
            {
                float normal = Mathf.InverseLerp(soundVolumeThreshold, 1.0f, outputRemap);
                outputRemap = Mathf.Lerp(0.0f, 1.0f, normal);
                if(debug)
                    Debug.Log("output : "+ reaktor.Output + " || outputremap : " + outputRemap);   
            }
            LaunchContinuouseWave(outputRemap);
            litTimeStamp = Time.fixedTime;
        }
        else if(reaktor.Output <= soundVolumeThreshold && Time.fixedTime - litTimeStamp < 1.0f)
        {
            LaunchContinuouseWave(0.0f);
        }
    }

    // Launch a wave in rings
    void LaunchWave()
    {
        int sequencer = 0;
        foreach (SoundRingBe soundRing in soundRingArray)
        {
            soundRing.Blink(wavelength, maxEmissionRingValue, sequencer/waveSpeed);
            sequencer++;
        }
    }

    void LaunchContinuouseWave(float wave)
    {
        int sequencer = 0;
        foreach (SoundRingBe soundRing in soundRingArray)
        {
            StartCoroutine(soundRing.coLightTo(wave * maxEmissionRingValue, maxEmissionRingValue, sequencer / waveSpeed));
            sequencer++;
        }
    }
}
