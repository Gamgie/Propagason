using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MainCameraMngr : MonoBehaviour
{

    [SerializeField] Klak.Motion.BrownianMotion _brownianMotion;
    [SerializeField] bool _animateCamera;

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

        _brownianMotion.enablePositionNoise = _animateCamera;
        _brownianMotion.enableRotationNoise = _animateCamera;
    }

    void Update()
    {
        _brownianMotion.enablePositionNoise = _animateCamera;
        _brownianMotion.enableRotationNoise = _animateCamera;
    }

    void ResetTransform()
    {
        if (DOTween.IsTweening(4454))
            return;

        if (Vector3.Distance(transform.position,_initialPosition) > 0.1f)
        {
            transform.DOMove(_initialPosition, 1.0f).SetId(4454);
            transform.DORotate(_initialRotation.eulerAngles, 1.0f);
            Debug.Log("Start reset transform tween ");
        }
    }

    public void AnimateCamera(float soundAmplitude)
    {
        if (!_animateCamera)
        {
            ResetTransform();
            return;
        }

        _brownianMotion.positionAmplitude = Mathf.Lerp(_initialPositionAmplitude, _initialPositionAmplitude + 2.0f*(soundAmplitude * _initialPositionAmplitude), 0.4f);
        _brownianMotion.rotationAmplitude = Mathf.Lerp(_initialRotationAmplitude, _initialRotationAmplitude + 2.0f * (soundAmplitude * _initialRotationAmplitude), 0.4f);
    }
}
