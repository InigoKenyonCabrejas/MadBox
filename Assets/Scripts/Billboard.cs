using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera worldCamera;
    
    private void Awake()
    {
        worldCamera = Camera.main;
    }
    
    private void LateUpdate()
    {
        transform.LookAt(transform.position + worldCamera.transform.forward);
    }
}