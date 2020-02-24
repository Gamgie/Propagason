using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundRingBe : MonoBehaviour
{
    [Range(0.0f,5.0f)]
    [SerializeField] float matEmissiveValue;
    [SerializeField] float minEmissionRate;
    [SerializeField] int rateOverTimeMultiplier = 2000;

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
            initialEmissiveColor = mat.GetColor("_EmissionColor");
            mat.SetColor("_EmissionColor", initialEmissiveColor * matEmissiveValue);
            actualColor = initialEmissiveColor;

            initialScale = transform.localScale;
        }

        _sparksPSEmission = GetComponentInChildren<ParticleSystem>().emission;
        _sparksPSEmission.rateOverTime = minEmissionRate;
        _sparksPSMain = GetComponentInChildren<ParticleSystem>().main;

    }

    // Update is called once per frame
    void Update()
    {
        // Update ring emissive color
        if(mat!=null && mat.GetColor("_EmissionColor") != actualColor * matEmissiveValue)
        {
            mat.SetColor("_EmissionColor", actualColor * matEmissiveValue);
        }

        if(matEmissiveValue != 0)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, 
                                                new Vector3(transform.localScale.x + matEmissiveValue, 
                                                            transform.localScale.y + matEmissiveValue, 
                                                            transform.localScale.z + matEmissiveValue),
                                                0.4f);
            _sparksPSEmission.rateOverTimeMultiplier = matEmissiveValue * rateOverTimeMultiplier;
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, initialScale, 0.8f);
            _sparksPSEmission.rateOverTimeMultiplier = Mathf.Lerp(_sparksPSEmission.rateOverTimeMultiplier, minEmissionRate, 0.8f);
        }

    }

    public IEnumerator coReceiveEnergy(float value, float maxEmissiveValue, float delay = 0)
    {
        yield return new WaitForSeconds(delay);

        Mathf.Clamp(value, 0.0f, maxEmissiveValue);
        matEmissiveValue = value;
    }

    public void UpdateColor(Color c, float delay = 0)
    {
        StartCoroutine(coChangeColor(c, delay));
    }

    IEnumerator coChangeColor(Color c, float delay = 0)
    {
        yield return new WaitForSeconds(delay);

        actualColor = c;
        if (mat != null && mat.GetColor("_EmissionColor") != actualColor * matEmissiveValue)
        {
            mat.SetColor("_EmissionColor", actualColor * matEmissiveValue);
            _sparksPSMain.startColor = new Color(actualColor.r, actualColor.g, actualColor.b);
        }
    }
}
