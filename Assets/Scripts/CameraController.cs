﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraController : MonoBehaviour
{
    [SerializeField] private IODevice io;
    [SerializeField] [Range(0f,100f)] private float speed = 100;
    private void Update()
    {
        transform.Rotate(Vector3.up, speed * Time.deltaTime * io.ReadJoystick());
    }
}
