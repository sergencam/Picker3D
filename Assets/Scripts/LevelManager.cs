using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public Level[] levels;
    [Space]
    public Transform levelsParent;
    public Platform flatPlatform;

    [Header("Level Testing")]
    [SerializeField] int testLevel = 0;

    private Level _level, _nextLevel;
    public Level Level { get { return _level; } }

    private void Awake()
    {
        Instance = this;
        if (testLevel != 0)
            PlayerPrefs.SetInt("CurrentLevel", testLevel);
        GetLevelValues();
    }

    private void Start()
    {
        GameManager.Instance.OnLevelComplete += IncreaseLevel;
    }

    void GetLevelValues()
    {
        if (!PlayerPrefs.HasKey("CurrentLevel"))
            PlayerPrefs.SetInt("CurrentLevel", 1);

        if (!PlayerPrefs.HasKey("NextLevel"))
            PlayerPrefs.SetInt("NextLevel", 2);

        if (!PlayerPrefs.HasKey("CurrentLevelForUI"))
            PlayerPrefs.SetInt("CurrentLevelForUI", 1);

        _level = Instantiate(levels[GetCurrentLevel()], Vector3.zero, Quaternion.identity, levelsParent);
        Platform startPlatform = Instantiate(flatPlatform, Vector3.zero, Quaternion.identity, _level.platformsParent);
        startPlatform.transform.eulerAngles = new Vector3(0f, 180f, 0f);
        _nextLevel = Instantiate(levels[GetNextLevel()], _level.platformObjects.Find(x => x.platformType == PlatformType.Finish).endPoint.position, Quaternion.identity, levelsParent);
    }

    public void IncreaseLevel()
    {
        var currentLevel = PlayerPrefs.GetInt("CurrentLevel");
        var currentLevelForUI = PlayerPrefs.GetInt("CurrentLevelForUI");

        if (currentLevel == levels.Length && !PlayerPrefs.HasKey("LoopLevels"))
            PlayerPrefs.SetInt("LoopLevels", 5);

        if (PlayerPrefs.HasKey("LoopLevels"))
        {
            PlayerPrefs.SetInt("CurrentLevel", GetNextLevel() + 1);
            List<int> potentialNextLevels = new List<int>();
            for (int i = 1; i < levels.Length + 1; i++)
                if (i != GetCurrentLevel() + 1)
                    potentialNextLevels.Add(i);
            var randomNextLevel = potentialNextLevels[Random.Range(0, potentialNextLevels.Count)];
            PlayerPrefs.SetInt("NextLevel", randomNextLevel);
        }
        else
        {
            currentLevel++;
            PlayerPrefs.SetInt("CurrentLevel", currentLevel);
            List<int> potentialNextLevels = new List<int>();
            if (currentLevel == levels.Length)
            {
                for (int i = 1; i < levels.Length + 1; i++)
                    if (i != currentLevel)
                        potentialNextLevels.Add(i);
                var randomNextLevel = potentialNextLevels[Random.Range(0, potentialNextLevels.Count)];
                PlayerPrefs.SetInt("NextLevel", randomNextLevel);
            }
            else
                PlayerPrefs.SetInt("NextLevel", currentLevel + 1);
        }

        currentLevelForUI++;
        PlayerPrefs.SetInt("CurrentLevelForUI", currentLevelForUI);
    }

    public int GetCurrentLevel()
    {
        return PlayerPrefs.GetInt("CurrentLevel") - 1;
    }

    public int GetNextLevel()
    {
        return PlayerPrefs.GetInt("NextLevel") - 1;
    }

    public int GetCurrentLevelForUI()
    {
        return PlayerPrefs.GetInt("CurrentLevelForUI");
    }

    public float LevelProgress()
    {
        return 0;
    }

    private void OnDisable()
    {
        if (!GameManager.Instance) return;
        GameManager.Instance.OnLevelComplete -= IncreaseLevel;
    }
}