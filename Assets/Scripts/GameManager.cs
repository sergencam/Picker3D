// using ElephantSDK;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public delegate void onLevelStart();
    public event onLevelStart OnLevelStart;
    public delegate void onLevelComplete();
    public event onLevelComplete OnLevelComplete;
    public delegate void onLevelFail();
    public event onLevelFail OnLevelFail;

    [HideInInspector] public bool isLevelStart;
    [HideInInspector] public bool isLevelEnd;
    [HideInInspector] public bool isEndGame;
    [HideInInspector] public bool isLevelFail;
    [HideInInspector] public bool isLevelComplete;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        OnLevelStart += OnLevelStartAction;
        OnLevelFail += OnLevelFailAction;
        OnLevelComplete += OnLevelCompleteAction;
    }

    private void Update()
    {
        CheckForLevelEnd();
        CheckForStart();
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void TryAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator LevelCompleteCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        OnLevelComplete?.Invoke();
    }

    public void StartLevelComplete(float delay)
    {
        if (isLevelComplete) return;
        OnLevelCompleteAction();
        StartCoroutine(LevelCompleteCoroutine(delay));
    }

    public void StartLevelFail()
    {
        OnLevelFail?.Invoke();
    }

    public void StartLevel()
    {
        OnLevelStart?.Invoke();
    }

    private void OnLevelStartAction()
    {
        isLevelStart = true;
    }

    private void OnLevelFailAction()
    {
        isLevelFail = true;
        isLevelEnd = true;
    }

    public void OnLevelCompleteAction()
    {
        if (isLevelComplete) return;
        isLevelComplete = true;
        isLevelEnd = true;
    }

    private void OnEndGameAction()
    {
        isEndGame = true;
    }

    private void CheckForLevelEnd()
    {
    }

    private void CheckForStart()
    {
        if (Input.GetMouseButtonDown(0) && !isLevelStart)
            StartLevel();
    }

    private void OnDisable()
    {
        OnLevelStart -= OnLevelStartAction;
        OnLevelFail -= OnLevelFailAction;
        OnLevelComplete -= OnLevelCompleteAction;
    }
}