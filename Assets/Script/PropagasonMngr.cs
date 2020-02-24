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
    [SerializeField] private GameObject _soundRingParent;
    [SerializeField] private MainCameraMngr _mainCamera;
    [SerializeField] private AudioMngr _audioMngr;
    private SoundRingBe[] _soundRingArray;


    [Header("Camera movement")]
    [SerializeField] KeyCode _launchCameraPanning;
    [SerializeField] TimelineMngr _timelineMngr;

    [Header("Final Ember")]
    [SerializeField] ParticleSystem _finalEmbersPS;
    ParticleSystem.EmissionModule _finalEmbersEmission;
    [SerializeField] float _finalEmberEmiterMultiplier;

    [Header("Color Range")]
    public Color[] colors;
    public Color result;

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

        LaunchContinuouseWave(_audioMngr.Volume, ComputeNoteColor());
        Debug.Log(_audioMngr.Note);

        // Send RMS value to camera for camera movement.
        //_mainCamera.AnimateCamera(inputLevel);
    }


    public void LaunchContinuouseWave(float waveEnergy, Color waveColor)
    {
        int sequencer = 0;
        foreach (SoundRingBe soundRing in _soundRingArray)
        {
            StartCoroutine(soundRing.coReceiveEnergy(waveEnergy * maxEmissionRingValue, maxEmissionRingValue, sequencer / waveSpeed));
            soundRing.UpdateColor(waveColor, sequencer / waveSpeed);
            sequencer++;
        }

        StartCoroutine(coBurstFinalEmbers(sequencer / waveSpeed, waveEnergy * maxEmissionRingValue));
    }

    IEnumerator coBurstFinalEmbers(float delay, float waveValue)
    {
        yield return new WaitForSeconds(delay);
        _finalEmbersEmission.rateOverTime = waveValue * _finalEmberEmiterMultiplier;
    }

    Color ComputeNoteColor()
    {
        return colors[(int)_audioMngr.Note];
    }
}
