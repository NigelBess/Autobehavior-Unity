using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GratedCircle : MonoBehaviour
{
    [SerializeField] private float simulatedDistance = 0.1f;// how far away do we want to simulate the position of the circle?
    [SerializeField] private int pixelThresholdFromCenter = 20;//how close can the circle be to the center to count as a success?
    private bool worldScale = true;//should we scale the circle to simulate it as a 3d object in space
    RectTransform rt;
    private Vector3 worldPoint;//simulated 3d point in space where the grated circle exists
    private float originalHeight;//world space height if the grated circle was at the world point
    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        Reset(1);
    }
    public void SetScalingMode(bool scale3d)//called to determine if 3d scaling is enabled
    {
        worldScale = scale3d;
    }
    public void Reset(int side)
    {
        transform.position = new Vector3(Screen.width *(1 + Mathf.Sign(side)/2)/2,Screen.height/2,0);//sets position so that distance to fail and distance to success are equal
        worldPoint = GetWorldPoint();
        originalHeight = GetWorldHeight();
    }
    private Vector3 GetWorldPoint()//returns point in 3d space that the circle is simulated to be at
    {
        Vector3 dir = Camera.main.transform.forward;//vector in direction of camera view
        dir.y = 0;//ignore any up/down tilt of the camera
        Plane plane = new Plane(dir,simulatedDistance);//create a plane at the simulated distance, perpendicular to camera view
        Ray ray = Camera.main.ScreenPointToRay(transform.position);//define a ray from the camera towards the grated circle
        return GameFunctions.RayToPlanePoint(ray, plane);//get the intersection point of the ray on the plane
    }
    private float GetWorldHeight()//returns simulated height in worldspace units of the grated circle
    {
        Vector3 dir = worldPoint- Camera.main.transform.position;//get the direction from the camera to the grated circle
        Plane plane = new Plane(dir, worldPoint);// create a plane perpendicular to the camera at the worldpoint of the circle
        Ray ray = Camera.main.ScreenPointToRay(new Vector2(transform.position.x, transform.position.y + rt.rect.height/2));//define a ray from the camera to the top of the grated circle
        Vector3 top = GameFunctions.RayToPlanePoint(ray, plane);//get the intersection of the ray to the plane. This gives the world point of the top of the grated circle
        return (top - worldPoint).magnitude*2;//get distance in worldspace unit from top to center (times 2 for total height)
    }
    private void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(worldPoint);//turn world point into a screen point
        if (!worldScale) return;//the remaining code in this function scale the rect height to match the simulated world height
        float scale = originalHeight / GetWorldHeight();//get ratio between original world height and current world height
        transform.localScale = Vector3.one*scale;//scale accordingly
    }

    public bool AtCenter()//is the stimulus at the position where the mouse has succeeded?
    {
        return Mathf.Abs((transform.position.x - Screen.width / 2)) < pixelThresholdFromCenter;
    }
    public bool OutOfBounds()//is the stimulus touching the edge of the screen?
    {
        if ((transform.position.x + rt.rect.width) > Screen.width) return true;
        if ((transform.position.x - rt.rect.width) < 0) return true;
        return false;
    }
    public Vector3 GetWorldPos()
    {
        return worldPoint;
    }
}
