using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropagasonMngr : MonoBehaviour
{
    public bool logInputLevel;

    [Header("Wave Settings")]
    public float waveSpeed;
    [Range(0.1f, 1.0f)]
    public float wavelength;
    [Range(0.0f, 5.0f)]
    public float maxEmissionRingValue;

    [Header("External Links")]
    [SerializeField]  private GameObject _soundRingParent;
    [SerializeField] private MainCameraMngr _mainCamera;
    private SoundRingBe[] _soundRingArray;

    
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
    }

    // Update is called once per frame
    void Update()
    {
        // If we receive a keyboard event
        if(Input.GetKeyUp(_launchCameraPanning))
        {
            _timelineMngr.PlayCameraPanning();
        }

        float inputLevel = normalizedLevel;

        if (logInputLevel)
            Debug.Log("Normalized level : " + normalizedLevel);
    
        LaunchContinuouseWave(inputLevel);

        // Send RMS value to camera for camera movement.
        _mainCamera.AnimateCamera(inputLevel);
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
