using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundRingBe : MonoBehaviour
{
    [Range(0.0f,5.0f)]
    [SerializeField] float matEmissiveValue;
    [SerializeField] float minEmissionRate;

    private Material mat;
    private Color initialEmissiveColor;
    private Vector3 initialScale;
    private ParticleSystem.EmissionModule _sparksPSEmission;

    void Awake()
    {
        mat = GetComponent<Renderer>().material;
        if(mat!=null)
        {
            initialEmissiveColor = mat.GetColor("_EmissionColor");
            mat.SetColor("_EmissionColor", initialEmissiveColor * matEmissiveValue);

            initialScale = transform.localScale;
        }

        _sparksPSEmission = GetComponentInChildren<ParticleSystem>().emission;
        _sparksPSEmission.rateOverTime = minEmissionRate;
    }

    // Update is called once per frame
    void Update()
    {
        if(mat!=null && mat.GetColor("_EmissionColor") != initialEmissiveColor * matEmissiveValue)
        {
            mat.SetColor("_EmissionColor", initialEmissiveColor * matEmissiveValue);
        }

        if(matEmissiveValue != 0)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, 
                                                new Vector3(transform.localScale.x + matEmissiveValue, 
                                                            transform.localScale.y + matEmissiveValue, 
                                                            transform.localScale.z + matEmissiveValue),
                                                0.4f);
            _sparksPSEmission.rateOverTimeMultiplier = matEmissiveValue * 2000;
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, initialScale, 0.8f);
            _sparksPSEmission.rateOverTimeMultiplier = Mathf.Lerp(_sparksPSEmission.rateOverTimeMultiplier, minEmissionRate, 0.8f);
        }

        
    }

    public void Blink(float duration, float maxEmissiveValue, float delay = 0)
    {
        DOTween.Kill(this);
        DOTween.To(() => matEmissiveValue, x => matEmissiveValue = x, maxEmissiveValue, duration)
            .SetDelay(delay)
            .OnComplete(()=>BlinkEnd(duration));
    }

    public IEnumerator coLightTo(float value, float maxEmissiveValue, float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        Mathf.Clamp(value, 0.0f, maxEmissiveValue);
        matEmissiveValue = value;
    }

    private void BlinkEnd(float speed)
    {
        DOTween.To(() => matEmissiveValue, x => matEmissiveValue = x, 0.0f, speed);
    }
}
