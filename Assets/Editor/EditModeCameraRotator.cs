using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EditModeCameraRotator : MonoBehaviour
{
    [SerializeField] [Range(0f, 360f)] private float angle;
    private void Awake()
    {
        angle = 0;
        ApplyRotation();
    }
    private void Update()
    {
        if (Application.isPlaying)
        {
            enabled = false;
            return;
        } 
        ApplyRotation();
    }
    private void ApplyRotation()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, angle, transform.rotation.eulerAngles.z);
    }
}
