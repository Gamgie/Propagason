using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundRingBe : MonoBehaviour
{
    [Range(0.0f,5.0f)]
    [SerializeField] float _matEmissiveValue;
    [SerializeField] float _minEmissionRate;
    [SerializeField] int _rateOverTimeMultiplier = 2000;

    private Material mat;
    private Color initialEmissiveColor;
    private Vector3 initialScale;
    private ParticleSystem.EmissionModule _sparksPSEmission;
    private ParticleSystem.MainModule _sparksPSMain;


    private Color actualColor;
    private Color _sparksPSStartColor;

    void Awake()
    {
        mat = GetComponent<Renderer>().material;
        if(mat!=null)
        {
            initialEmissiveColor = mat.GetColor("_Color");
            mat.SetColor("_Color", initialEmissiveColor * _matEmissiveValue);
            actualColor = initialEmissiveColor;

            initialScale = transform.localScale;
        }

        _sparksPSEmission = GetComponentInChildren<ParticleSystem>().emission;
        _sparksPSEmission.rateOverTime = _minEmissionRate;
        _sparksPSMain = GetComponentInChildren<ParticleSystem>().main;

    }

    // Update is called once per frame
    void Update()
    {
        if (_matEmissiveValue != 0)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, 
                                                initialScale * (1+_matEmissiveValue/2),
                                                0.2f);
            _sparksPSEmission.rateOverTimeMultiplier = _matEmissiveValue * _rateOverTimeMultiplier;
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, initialScale, 0.8f);
            _sparksPSEmission.rateOverTimeMultiplier = Mathf.Lerp(_sparksPSEmission.rateOverTimeMultiplier, _minEmissionRate, 0.8f);
        }

    }

    IEnumerator coReceiveEnergy(float value, float maxEmissiveValue, float delay = 0)
    {
        yield return new WaitForSeconds(delay);

        
    }

    /// <summary>
    /// Update ring color and emission value.
    /// </summary>
    /// <param name="c"></param>
    /// <param name="energy"></param>
    /// <param name="maxEmission"></param>
    /// <param name="delay"></param>
    /// <returns></returns>
    public IEnumerator coUpdateRing(Color c, float energy, float delay = 0)
    {
        yield return new WaitForSeconds(delay);

        _matEmissiveValue = energy;

        // Compute new color. Adjust color with emission intensity
        actualColor = c * (Mathf.Pow(2, _matEmissiveValue) - 1);
        //actualColor = c * _matEmissiveValue;

        if (mat != null)
        {
            mat.SetColor("_Color", actualColor);
            _sparksPSMain.startColor = new Color(actualColor.r, actualColor.g, actualColor.b);
        }
    }

    public void SetupRing(float minEmissionRate, int rateOverTimeMultiplier)
    {
        _minEmissionRate = minEmissionRate;
        _rateOverTimeMultiplier = rateOverTimeMultiplier;
    }
}
