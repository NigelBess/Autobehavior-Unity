using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GratedCircle : MonoBehaviour
{
    [SerializeField] private float simulatedDistance = 0.1f;
    RectTransform rt;
    private Vector3 worldPoint;
    private float originalHeight;
    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        Reset(1);
    }
    public void Reset(int side)
    {
        transform.position = new Vector2(Screen.width / 4 * Mathf.Sign(side),Screen.height/2);
        worldPoint = GetWorldPoint();
        originalHeight = GetWorldHeight();
    }
    private Vector3 GetWorldPoint()
    {
        Vector3 dir = Camera.main.transform.forward;
        dir.y = 0;
        Plane plane = new Plane(dir,simulatedDistance);
        Ray ray = Camera.main.ScreenPointToRay(transform.position);;
        return GameFunctions.RayToPlanePoint(ray, plane);
    }
    private float GetWorldHeight()
    {
        Vector3 dir = worldPoint- Camera.main.transform.position;
        Plane plane = new Plane(dir, worldPoint);
        Ray ray = Camera.main.ScreenPointToRay(new Vector2(transform.position.x, transform.position.y + rt.rect.height/2));
        Vector3 top = GameFunctions.RayToPlanePoint(ray, plane);
        return (top - worldPoint).magnitude*2;
    }
    private void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(worldPoint);
        float scale = originalHeight / GetWorldHeight();
        transform.localScale = Vector3.one*scale;
        if (Input.GetKeyDown(KeyCode.U)) Reset(1);
    }

}
