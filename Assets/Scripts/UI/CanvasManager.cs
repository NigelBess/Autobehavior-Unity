using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private GameObject[] menus;
    private int[] canvasSequence = new int[4] {0,0,0,0};
    private int index = 0;

    public void Welcome()
    {
        Open(0);
    }
    public void HUD()
    {
        Open(1);
    }
    public void Blank()
    {
        Open(2);
    }
    public void Pause()
    {
        Open(3);
    }
    private void Open(int next)
    {
        if (index < canvasSequence.Length - 1)
        {
            index++;
            canvasSequence[index] = next;
        }
        else
        {
            for (int i = 0; i < index; i++)
            {
                canvasSequence[i] = canvasSequence[i + 1];
            }
        }
        GameFunctions.OpenMenu(menus, next);
        return;
    }
    public void Previous()
    {
        if (index <= 0) return;
        index--;
        GameFunctions.OpenMenu(menus,canvasSequence[index]);
    }

}
