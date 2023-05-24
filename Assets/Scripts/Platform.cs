using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum PlatformType { Drop, Finish, Flat }
public class Platform : MonoBehaviour
{
    public PlatformType platformType;
    public Transform endPoint;
}
