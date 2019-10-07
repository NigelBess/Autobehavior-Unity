using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraController : IOInteractor
{
    
    [SerializeField] [Range(0f,200f)] private float speed = 100;
    private void Update()
    {
        transform.Rotate(Vector3.up, speed * Time.deltaTime * io.ReadJoystick());
    }
    
    public float GetSpeed()
    {
        return speed;
    }
}
