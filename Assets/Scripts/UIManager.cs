using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public GameObject startPanel, inGamePanel, levelCompletePanel, levelFailPanel, levelProgress, levelProgressStartEnd;
    public Transform levelProgressUITransform;
    private int levelProgressState;
    private List<GameObject> levelProgresses = new List<GameObject>();
    public Color levelProgressCompleteColor;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameManager.Instance.OnLevelStart += CloseStartPanel;
        GameManager.Instance.OnLevelComplete += OpenLevelCompletePanel;
        GameManager.Instance.OnLevelFail += OpenLevelFailPanel;
        SetLevelProgressUI();
    }

    private void OpenLevelFailPanel()
    {
        levelFailPanel.SetActive(true);
        inGamePanel.SetActive(false);
    }

    private IEnumerator LevelCompleteCoroutine()
    {
        inGamePanel.SetActive(false);
        levelProgressUITransform.gameObject.SetActive(false);
        yield return new WaitForSeconds(1.5f);
        levelCompletePanel.SetActive(true);
    }

    private void OpenLevelCompletePanel()
    {
        StartCoroutine(LevelCompleteCoroutine());
    }

    private void CloseStartPanel()
    {
        startPanel.SetActive(false);
        inGamePanel.SetActive(true);
    }

    public void SetLevelProgressUI()
    {
        levelProgressState = 0;
        var lpStartUI = Instantiate(levelProgressStartEnd, levelProgressUITransform);
        lpStartUI.transform.GetChild(0).GetComponent<Text>().text = LevelManager.Instance.GetCurrentLevelForUI().ToString();
        for (int i = 0; i < LevelManager.Instance.Level.dropAreas.Count; i++)
            levelProgresses.Add(Instantiate(levelProgress, levelProgressUITransform) as GameObject);
        var lpEndUI = Instantiate(levelProgressStartEnd, levelProgressUITransform);
        lpEndUI.transform.GetChild(0).GetComponent<Text>().text = (LevelManager.Instance.GetCurrentLevelForUI() + 1).ToString();
    }

    public void IncreaseLevelProgress()
    {
        levelProgresses[levelProgressState].GetComponent<Image>().color = levelProgressCompleteColor;
        levelProgresses[levelProgressState].transform.DOPunchScale(Vector3.one * 0.3f, 0.3f, 5);
        levelProgressState++;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.OnLevelStart -= CloseStartPanel;
        GameManager.Instance.OnLevelComplete -= OpenLevelCompletePanel;
        GameManager.Instance.OnLevelFail -= OpenLevelFailPanel;
    }
}
