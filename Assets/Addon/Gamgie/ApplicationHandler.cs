using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationHandler : MonoBehaviour
{
    [SerializeField] private bool _leaveOnEscape;

    // Update is called once per frame
    void Update()
    {
        if (_leaveOnEscape && Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
            Debug.Log("Quitting application");
        }
            
    }
}
