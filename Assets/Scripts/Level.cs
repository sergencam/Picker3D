using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [HideInInspector] public List<PlatformType> platforms = new List<PlatformType>();
    [HideInInspector] public List<Platform> platformObjects = new List<Platform>();
    [HideInInspector] public List<int> dropAreaCollectableCountsToPass = new List<int>();
    [HideInInspector] public List<DropArea> dropAreas = new List<DropArea>();
    [HideInInspector] public Color groundColor, sideColor, dropAreaColor;
    public Transform platformsParent;

    private void Awake()
    {
        for (int i = 0; i < platformsParent.childCount; i++)
        {
            if (platformsParent.GetChild(i).GetComponent<DropArea>())
                dropAreas.Add(platformsParent.GetChild(i).GetComponent<DropArea>());
        }
    }
}
