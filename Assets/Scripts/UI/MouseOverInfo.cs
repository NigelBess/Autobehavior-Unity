using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseOverInfo : MonoBehaviour
{
    [SerializeField] private GameObject Info;
    private void Awake()
    {
        Info.SetActive(false);
    }
    private void Update()
    {
        Info.SetActive(RaycastEventSystem.selected == this.gameObject);
        
    }
}
