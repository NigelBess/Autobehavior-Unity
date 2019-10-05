using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectEnabler : MonoBehaviour
{
    public void DisableForSeconds(MonoBehaviour obj, float time)
    {
        obj.enabled = false;
        StartCoroutine(EnableObject(obj, time));
    }
    IEnumerator EnableObject(MonoBehaviour obj, float time)
    {
        yield return new WaitForSeconds(time);
        obj.enabled = true;
    }
}
