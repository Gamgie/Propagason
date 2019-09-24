using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropagasonMngr : MonoBehaviour
{
    public bool logInputLevel;

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
    [Tooltip("Decide if you use raw RMS or Audio Level Tracker (normalized) value.")]
    public bool useNormalizedLevel;
    [SerializeField] Lasp.FilterType _filterType;
    [Tooltip("In case, you wanna filter sound input to avoid external noise for example. Use threshold.")]
    [SerializeField, Range(0.0f, 1.0f)] float soundThreshold = 0.0f;
    
    public float normalizedLevel { get; set; }

    [Header("Camera movement")]
    [SerializeField] KeyCode _launchCameraPanning;
    [SerializeField] TimelineMngr _timelineMngr;

    [Header("Final Ember")]
    [SerializeField] ParticleSystem _finalEmbersPS;
    ParticleSystem.EmissionModule _finalEmbersEmission;
    [SerializeField] float _finalEmberEmiterMultiplier;



    private void Awake()
    {
        _soundRingArray = _soundRingParent.GetComponentsInChildren<SoundRingBe>();
        _finalEmbersEmission = _finalEmbersPS.emission;
        _finalEmbersEmission.rateOverTime = 0.0f;

        GetComponent<Lasp.AudioLevelTracker>().filterType = _filterType;
    }

    // Update is called once per frame
    void Update()
    {
        // If we receive a keyboard event, send a sound wave
        if (Input.GetKeyUp(launchOneWaveKeyCode))
        {
            LaunchWave();
        }
        else if(Input.GetKeyUp(_launchCameraPanning))
        {
            _timelineMngr.PlayCameraPanning();
        }

        float inputLevel = 0.0f;
        if(!useNormalizedLevel)
        {
            // We use raw RMS decibel value
            inputLevel = Lasp.MasterInput.CalculateRMSDecibel(_filterType);
            inputLevel = Mathf.Clamp01(1 - inputLevel / (-40)); // clamp it considering -40Db is silence level.

            if (logInputLevel)
                Debug.Log("RMS : " + logInputLevel);
        }
        else
        {
            inputLevel = normalizedLevel;

            if (logInputLevel)
                Debug.Log("Normalized level : " + normalizedLevel);
        }
    
        // Remap RMS value according to threshold
        if(soundThreshold != 0)
        {
            float normal = Mathf.InverseLerp(soundThreshold, 1.0f, inputLevel);
            inputLevel = Mathf.Lerp(0.0f, 1.0f, normal);
        }

        LaunchContinuouseWave(inputLevel);

        // Send RMS value to camera for camera movement.
        _mainCamera.AnimateCamera(inputLevel);
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

        StartCoroutine(coBurstFinalEmbers(sequencer / waveSpeed, wave*maxEmissionRingValue));
    }

    IEnumerator coBurstFinalEmbers(float delay, float waveValue)
    {
        yield return new WaitForSeconds(delay);
        _finalEmbersEmission.rateOverTime = waveValue * _finalEmberEmiterMultiplier;
    }
}
