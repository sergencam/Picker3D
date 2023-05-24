using UnityEngine;

public class SwerveMovement : MonoBehaviour
{
    public float swerweSpeed, forwardSpeed, maxX;
    private float lastXPos, xMoveDir;
    private bool stop;
    private Rigidbody rgd;

    private void Awake()
    {
        rgd = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Stop();
        GameManager.Instance.OnLevelStart += Move;
        GameManager.Instance.OnLevelComplete += Stop;
        GameManager.Instance.OnLevelFail += Stop;
    }

    private void Update()
    {
        rgd.position = new Vector3(Mathf.Clamp(rgd.position.x, -maxX, maxX), rgd.position.y, rgd.position.z);
    }

    private void FixedUpdate()
    {
        CheckInput();
        if (stop && (GameManager.Instance.isLevelComplete || GameManager.Instance.isLevelFail))
            rgd.velocity = Vector3.zero;
        else if (stop)
            rgd.velocity = new Vector3(xMoveDir * swerweSpeed * Time.deltaTime, 0f, 0f);
        else
            rgd.velocity = new Vector3(xMoveDir * swerweSpeed * Time.deltaTime, 0f, forwardSpeed * Time.deltaTime);
    }

    private void CheckInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastXPos = Input.mousePosition.x;
            xMoveDir = 0f;
        }
        else if (Input.GetMouseButton(0))
            xMoveDir = Input.mousePosition.x - lastXPos;
        else
            xMoveDir = 0f;
        lastXPos = Input.mousePosition.x;
    }

    public void Move()
    {
        stop = false;
    }

    public void Stop()
    {
        stop = true;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.OnLevelStart -= Move;
        GameManager.Instance.OnLevelComplete -= Stop;
        GameManager.Instance.OnLevelFail -= Stop;
    }
}
