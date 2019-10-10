using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KeyAction : MonoBehaviour
{
    [SerializeField] private KeyCode key;
    public UnityEvent action;

    private void Update()
    {
        if (Input.GetKeyDown(key))
        {
            action.Invoke();
        }
    }
}
