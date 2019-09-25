using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIMngr : MonoBehaviour
{

    [SerializeField] bool showUI;
    [SerializeField] KeyCode uiKeyCode;
    [SerializeField] Image beatDetectionImage;
    [SerializeField] Color beatDetectionStartColor;
    [SerializeField] Color beatDetectionEndColor;

    GameObject child;

    // Start is called before the first frame update
    void Start()
    {
        child = gameObject.GetComponentInChildren<Image>().gameObject;
        child.SetActive(showUI);
        beatDetectionImage.color = beatDetectionStartColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (child.activeSelf != showUI) child.SetActive(showUI);

        if(Input.GetKeyUp(uiKeyCode))
        {
            showUI = !showUI;
        }
    }

    public void OnBeatDetected()
    {
        beatDetectionImage.DOBlendableColor(beatDetectionEndColor, 0.1f).SetLoops(2, LoopType.Yoyo);
    }
}
