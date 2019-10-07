using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraController : IOInteractor
{
    
    [SerializeField] [Range(0f,200f)] private float speed = 100;
    private void Update()
    {
        if (io == null) return;
        transform.Rotate(Vector3.up, speed * Time.deltaTime * io.ReadJoystick());
    }
    
    public float GetSpeed()
    {
        return speed;
    }
    public void SnapTo(Vector3 worldPoint)
    {
        transform.LookAt(new Vector3(-worldPoint.x, transform.position.y, -worldPoint.z));
    }
}
