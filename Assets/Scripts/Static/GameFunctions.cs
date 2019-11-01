using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameFunctions : MonoBehaviour
{
    public static void OpenMenu(GameObject[] menus, int num)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i] != null) menus[i].SetActive(i == num);

        }
    }
    public static void OpenMenu(List<GameObject> menus, int num)
    {
        for (int i = 0; i < menus.Count(); i++)
        {
            if (menus[i] != null) menus[i].SetActive(i == num);
        }
    }
    public static void LogBytes(byte[] msg)
    {
        string str = "";
        for (int i = 0; i < msg.Length; i++)
        {
            str += (msg[i]).ToString() + " ";
        }
        Debug.Log(str);
    }
    public static Vector3 RayToPlanePoint(Ray ray, Plane plane)
    {
        float enter;
        plane.Raycast(ray, out enter);
        return ray.GetPoint(enter);
    }
    public static int BoolToInt(bool val)
    {
        if (val) return 1;
        return 0;
    }
    public static int Sign(float val)
    {
        if (val < 0) return -1;
        if (val > 0) return 1;
        return 0;
    }
    public static int Sign(int val)
    {
        return Sign((float)val);
    }
}
