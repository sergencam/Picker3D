using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

[System.Serializable]
public class CamSettings : ISerializationCallbackReceiver
{
    [HideInInspector] public string name;
    public CamTypes camType;
    public Transform target;
    public Vector3 offset;
    public Vector3 constantRotation;
    public bool canRotate;
    public float smoothSpeed;
    public bool lockX, lockY, lockZ;


    public void OnAfterDeserialize() { }
    public void OnBeforeSerialize() => name = camType.ToString() + " Cam Settings";
}
public enum CamTypes { Standart, LevelComplete, LevelFail, EndGame, HRUpgrade, BossUpgrade, Upgrade, WorkArea }
public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    private bool isCameraShaking;
    public List<CamSettings> camSettings = new List<CamSettings>();
    private CamSettings currentCam;
    float sSpeed;
    Camera mainCam;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        mainCam = GetComponent<Camera>();
        OnLevelStart();
        // GameManager.Instance.OnLevelStart += OnLevelStart;
        // GameManager.Instance.OnEndGame += EndGameCam;
        // GameManager.Instance.OnLevelComplete += OnLevelComplete;
        // GameManager.Instance.OnLevelFail += OnLevelFail;
    }

    void FixedUpdate()
    {
        if (currentCam == null) return;
        if (currentCam.target == null) return;
        Vector3 dPos = currentCam.target.position + currentCam.offset;
        Vector3 sPos = Vector3.Lerp(transform.position, dPos, sSpeed * Time.deltaTime);
        if (currentCam.lockX)
            sPos.x = 0f;
        else if (currentCam.lockY)
            sPos.y = transform.position.y;
        else if (currentCam.lockZ)
            sPos.z = transform.position.z;
        transform.position = sPos;

        Quaternion dRot = currentCam.canRotate ? currentCam.target.rotation : Quaternion.Euler(currentCam.constantRotation);
        Quaternion sRot = Quaternion.Slerp(transform.rotation, dRot, sSpeed * Time.deltaTime);
        transform.rotation = sRot;
    }

    public void ChangeTargets(CamTypes type)
    {
        currentCam = camSettings.Find(x => x.camType == type);

        float _sSpeed = currentCam.smoothSpeed;
        if (_sSpeed != 0f)
            sSpeed = _sSpeed;
    }

    public void ShakeCam()
    {
        // if (isCameraShaking || GameManager.Instance.isLevelEnd) return;
        transform.DOShakePosition(0.2f, 2f, 10).OnComplete(() => isCameraShaking = false);
        isCameraShaking = true;
    }

    public void OnLevelStart()
    {
        // LevelEndMoveTarget = LevelManager.Instance.endGameCam
        StandartCam();
    }

    public void OnLevelComplete()
    {
        ChangeTargets(CamTypes.LevelComplete);
    }

    public void OnLevelFail()
    {
        ChangeTargets(CamTypes.LevelFail);
    }

    public void StandartCam()
    {
        sSpeed = camSettings.Find(x => x.camType == CamTypes.Standart).smoothSpeed;
        ChangeTargets(CamTypes.Standart);
    }

    public void EndGameCam()
    {
        ChangeTargets(CamTypes.EndGame);
    }

    public void LookAtCam(Transform _transform)
    {
        _transform.LookAt(_transform.position + mainCam.transform.rotation * Vector3.forward, mainCam.transform.rotation * Vector3.up);
    }

    private void OnDisable()
    {
        // if (!GameManager.Instance) return;
        // GameManager.Instance.OnLevelStart -= OnLevelStart;
        // GameManager.Instance.OnEndGame -= EndGameCam;
        // GameManager.Instance.OnLevelComplete -= OnLevelComplete;
        // GameManager.Instance.OnLevelFail -= OnLevelFail;
    }
}