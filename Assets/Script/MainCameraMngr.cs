using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MainCameraMngr : MonoBehaviour
{

    [SerializeField] Klak.Motion.BrownianMotion _brownianMotion;

    Vector3 _initialPosition;
    Quaternion _initialRotation;
    float _initialPositionAmplitude;
    float _initialRotationAmplitude;

    private void Awake()
    {
        _initialPosition = this.transform.position;
        _initialRotation = this.transform.rotation;

        _initialPositionAmplitude = _brownianMotion.positionAmplitude;
        _initialRotationAmplitude = _brownianMotion.rotationAmplitude;
    }

    void ResetTransform()
    {
        if (transform.position != _initialPosition && !DOTween.IsTweening(transform))
        {
            transform.DOMove(_initialPosition, 1.0f).SetId(transform);
            transform.DORotate(_initialRotation.eulerAngles, 1.0f);
            Debug.Log("Start reset transform tween");
        }
    }

    public void AnimateCamera(float soundAmplitude)
    {

        _brownianMotion.positionAmplitude = Mathf.Lerp(_initialPositionAmplitude, _initialPositionAmplitude + 2.0f*(soundAmplitude * _initialPositionAmplitude), 0.4f);
        _brownianMotion.rotationAmplitude = Mathf.Lerp(_initialRotationAmplitude, _initialRotationAmplitude + 2.0f * (soundAmplitude * _initialRotationAmplitude), 0.4f);
    }
}
