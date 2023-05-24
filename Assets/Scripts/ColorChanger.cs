using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColorType { Ground, Side, DropArea }
public class ColorChanger : MonoBehaviour
{
    public ColorType type;

    private void Start()
    {
        SetColor();
    }

    private void SetColor()
    {
        Level lvl = transform.parent.parent.parent.GetComponent<Level>();
        switch (type)
        {
            case ColorType.Ground:
                GetComponent<MeshRenderer>().material.SetColor("_Color", lvl.groundColor);
                break;
            case ColorType.Side:
                GetComponent<MeshRenderer>().material.SetColor("_Color", lvl.sideColor);
                break;
            case ColorType.DropArea:
                GetComponent<MeshRenderer>().material.SetColor("_Color", lvl.dropAreaColor);
                break;
        }
    }
}