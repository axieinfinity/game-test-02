using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCamera : MonoBehaviour
{
    [SerializeField] private Camera mainCam;
    [SerializeField] private Camera miniMapCam;

    private void Update()
    {
        miniMapCam.orthographicSize = mainCam.orthographicSize + 3;
    }
}
