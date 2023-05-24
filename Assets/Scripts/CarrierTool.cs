using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(SwerveMovement))]
public class CarrierTool : MonoBehaviour
{
    public static CarrierTool Instance;
    private SwerveMovement swerveMovement;
    public int collectableDefaultLayer, wallPassLayer;
    public float dropAreaWaitTime = 5f;
    public Transform sizeUpText;
    private float dropAreaWaitTimer;
    private bool canDropCollectables;
    [HideInInspector] public bool isDropAreaPassed;

    private void Awake()
    {
        Instance = this;
        swerveMovement = GetComponent<SwerveMovement>();
    }

    private void Start()
    {
        GameManager.Instance.OnLevelComplete += OnLevelComplete;
    }

    private void Update()
    {
        DropAreaCountDownTimer();
        if (sizeUpText.transform.localScale.magnitude > 0)
            CameraManager.Instance.LookAtCam(sizeUpText);
    }

    private void DropAreaCountDownTimer()
    {
        if (canDropCollectables && !isDropAreaPassed && !GameManager.Instance.isLevelFail)
        {
            dropAreaWaitTimer += Time.deltaTime;
            if (dropAreaWaitTimer >= dropAreaWaitTime)
                GameManager.Instance.StartLevelFail();
        }
    }

    private void OnCollectCollectable(Collectable collectable)
    {
        collectable.gameObject.layer = wallPassLayer;
    }

    private void OnDropCollectable(Collectable collectable)
    {
        if (!canDropCollectables)
            collectable.gameObject.layer = collectableDefaultLayer;
    }

    private void OnDropAreaEnter()
    {
        swerveMovement.Stop();
        canDropCollectables = true;
        isDropAreaPassed = false;
    }

    public void OnDropAreaPassed()
    {
        swerveMovement.Move();
        canDropCollectables = false;
        dropAreaWaitTimer = 0f;
        transform.DOScale(transform.localScale + new Vector3(0.05f, 0f, 0.05f), 0.5f);
        sizeUpText.DOScale(Vector3.one, 0.7f).SetEase(Ease.OutBounce);
        sizeUpText.DOScale(Vector3.zero, 0.3f).SetDelay(0.74f);
    }

    private void OnLevelComplete()
    {
        transform.DOMove(new Vector3(0f, transform.position.y, transform.position.z + 30f), 1f).SetUpdate(UpdateType.Fixed);
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.GetComponent<DropArea>())
            if (!coll.gameObject.GetComponent<DropArea>().platformOpened)
                OnDropAreaEnter();

        if (coll.gameObject.GetComponent<Collectable>())
            OnCollectCollectable(coll.gameObject.GetComponent<Collectable>());

        if (coll.gameObject.CompareTag("Finish"))
            GameManager.Instance.StartLevelComplete(0f);
    }

    private void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.GetComponent<Collectable>())
            OnDropCollectable(coll.gameObject.GetComponent<Collectable>());
    }

    private void OnDisable()
    {
        if (!GameManager.Instance) return;
        GameManager.Instance.OnLevelComplete -= OnLevelComplete;
    }
}
