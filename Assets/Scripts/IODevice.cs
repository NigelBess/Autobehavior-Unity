using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public abstract class IODevice : MonoBehaviour
{
    public abstract int ReadJoystick();
}
