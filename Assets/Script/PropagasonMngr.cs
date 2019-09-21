using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropagasonMngr : MonoBehaviour
{
    public bool logRMSValue;

    [Header("Wave Settings")]
    public KeyCode launchOneWaveKeyCode;
    public float waveSpeed;
    [Range(0.1f, 3.0f)]
    public float wavelength;
    [Range(0.0f, 5.0f)]
    public float maxEmissionRingValue;
    //[Space(20)]

    [Header("External Links")]
    [SerializeField]  private GameObject _soundRingParent;
    [SerializeField] private MainCameraMngr _mainCamera;
    private SoundRingBe[] _soundRingArray;

    [Header("[LASP] Sound Reactive Settings")]
    [SerializeField] Lasp.FilterType _filterType;

    

    private void Awake()
    {
        _soundRingArray = _soundRingParent.GetComponentsInChildren<SoundRingBe>();
    }

    // Update is called once per frame
    void Update()
    {
        // If we receive a keyboard event, send a sound wave
        if (Input.GetKeyUp(launchOneWaveKeyCode))
        {
            LaunchWave();
        }

        float rms = Lasp.MasterInput.CalculateRMSDecibel(_filterType);
        rms = Mathf.Clamp01(1 - rms / (-40));

        LaunchContinuouseWave(rms);

        // Send RMS value to camera for camera movement.
        _mainCamera.AnimateCamera(rms);

        if (logRMSValue)
            Debug.Log("RMS : " + rms);
    }

    // Launch one wave in rings
    void LaunchWave()
    {
        int sequencer = 0;
        foreach (SoundRingBe soundRing in _soundRingArray)
        {
            soundRing.Blink(wavelength, maxEmissionRingValue, sequencer/waveSpeed);
            sequencer++;
        }
    }

    void LaunchContinuouseWave(float wave)
    {
        int sequencer = 0;
        foreach (SoundRingBe soundRing in _soundRingArray)
        {
            StartCoroutine(soundRing.coLightTo(wave * maxEmissionRingValue, maxEmissionRingValue, sequencer / waveSpeed));
            sequencer++;
        }
    }
}
