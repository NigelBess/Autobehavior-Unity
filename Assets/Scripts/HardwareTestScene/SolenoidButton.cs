using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SolenoidButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private TestSceneManager manager;
    [SerializeField] private Text solenoidStateText;
   private bool open = false;
    private void Awake()
    {
       open = false;
        solenoidStateText.text = "Closed";
    }
    public void OnPointerDown(PointerEventData ped)
    {
        open = true;
        manager.OpenSolenoid();
        solenoidStateText.text = "Open";
    }
    public void UnClick()
    {
        open = false;
        manager.CloseSolenoid();
        solenoidStateText.text = "Closed";
    }
    private void Update()
    {
        if (!open) return;
        if (!Input.GetMouseButton(0)) UnClick();
    }
}
