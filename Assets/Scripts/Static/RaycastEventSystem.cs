using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RaycastEventSystem : MonoBehaviour
{
    private static RaycastEventSystem Instance;
    public static GameObject selected;
    public static RaycastEventSystem current
    {
        get
        {
            if (Instance == null)
            {
                Instance = GameObject.FindObjectOfType<RaycastEventSystem>();
            }
            return Instance;
        }
        set
        {
            if (Instance != null)
            {
                return;
            }
            Instance = current;
        }

    }
    private void Update()
    {
        selected = UIRaycast();
    }
    public static GameObject UIRaycast()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        if (results != null && results.Count > 0)
        {
            return results[0].gameObject;
        }
        else
        {
            return null;
        }
    }
}
