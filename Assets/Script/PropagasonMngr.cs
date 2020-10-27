using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropagasonMngr : MonoBehaviour
{
    public bool logInputLevel;
    public KeyCode takeScreenshot;
    public int screenshotSuperSize = 2;

    [Header("Wave Settings")]
    public float waveSpeed;
    [Range(0.1f, 1.0f)]
    public float wavelength;
    [Range(0.0f, 20.0f)]
    public float maxEmissionRingValue;
    public float MaxEmissionRingValue { get => maxEmissionRingValue; set => maxEmissionRingValue = value; }

    [Header("External Links")]
    [SerializeField] private GameObject _soundRingParent;
    [SerializeField] private MainCameraMngr _mainCamera;
    [SerializeField] private AudioMngr _audioMngr;
    private SoundRingBe[] _soundRingArray;

    [Header("Ring Parameters")]
    [SerializeField] private float _minEmissionRate;
    [SerializeField] private int _rateOverTimeMultiplier = 2000;

    [Header("Camera movement")]
    [SerializeField] KeyCode _launchCameraPanning;
    [SerializeField] TimelineMngr _timelineMngr;

    [Header("Final Ember")]
    [SerializeField] ParticleSystem _finalEmbersPS;
    ParticleSystem.EmissionModule _finalEmbersEmission;
    ParticleSystem.MainModule _finalEmbersMainModule;
    [SerializeField] float _finalEmberEmiterMultiplier;

    [Header("Color Range")]
    [ColorUsageAttribute(false, true)]
    public Color[] colors;
    private Color lastColor;

    private void Awake()
    {
        _soundRingArray = _soundRingParent.GetComponentsInChildren<SoundRingBe>();
        _finalEmbersEmission = _finalEmbersPS.emission;
        _finalEmbersEmission.rateOverTime = 0.0f;
        _finalEmbersMainModule = _finalEmbersPS.main;

        LoadPlayerPrefs();

        lastColor = colors[0];
    }

    // Update is called once per frame
    void Update()
    {
        // If we receive a keyboard event
        if(Input.GetKeyUp(_launchCameraPanning))
        {
            _timelineMngr.PlayCameraPanning();
        }
        else if(Input.GetKeyUp(takeScreenshot))
        {
            ScreenCapture.CaptureScreenshot("Recordings/Propagason_"+ System.DateTime.Now.Day + System.DateTime.Now.Month + System.DateTime.Now.Hour + System.DateTime.Now.Minute + System.DateTime.Now.Second + ".png", screenshotSuperSize);
        }

        Color lerpColor = Color.Lerp(lastColor, ComputeNoteColor(), 0.2f);

        LaunchContinuouseWave(_audioMngr.Volume, lerpColor);
        lastColor = lerpColor;

        if (logInputLevel)
        {
            Debug.Log("Input volume : " + _audioMngr.Volume);
        }
    }


    public void LaunchContinuouseWave(float waveEnergy, Color waveColor)
    {
        int sequencer = 0;

        // Ensure an energy value between 0 and 1
        waveEnergy = Mathf.Clamp(waveEnergy, 0.0f, 1.0f);

        // Scale enery according to max emission
        waveEnergy = waveEnergy * maxEmissionRingValue;

        // Loop all ring to propagate the wave at wavespeed.
        foreach (SoundRingBe soundRing in _soundRingArray)
        {
            soundRing.SetupRing(_minEmissionRate, _rateOverTimeMultiplier);

            StartCoroutine(soundRing.coUpdateRing(waveColor,
                                                  waveEnergy,
                                                  sequencer / waveSpeed));
            sequencer++;
        }

        // Burst final embers for particle system cloud
        StartCoroutine(coBurstFinalEmbers(sequencer / waveSpeed, waveEnergy * maxEmissionRingValue, waveColor));
    }

    IEnumerator coBurstFinalEmbers(float delay, float waveValue, Color waveColor)
    {
        yield return new WaitForSeconds(delay);
        _finalEmbersEmission.rateOverTime = waveValue * _finalEmberEmiterMultiplier;
        _finalEmbersMainModule.startColor = waveColor;
    }

    Color ComputeNoteColor()
    {
        return colors[(int)_audioMngr.Note];
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.SetFloat("maxEmissionRingValue", maxEmissionRingValue);
    }

    private void LoadPlayerPrefs()
    {
        if(PlayerPrefs.GetFloat("maxEmissionRingValue", -100) != -100)
            maxEmissionRingValue = PlayerPrefs.GetFloat("maxEmissionRingValue",-100);
    }
}
