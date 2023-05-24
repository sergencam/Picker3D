#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class LevelEditor : MonoBehaviour
{
    public List<Platform> platforms = new List<Platform>();
    public Level baseLevel;
    public bool createNewLevel;
    private bool levelSetted;
    private int levelPlatformCount, dropAreaCount;
    [Header("EDIT LEVEL")]
    public Level level;
    public List<PlatformType> levelPlatforms = new List<PlatformType>();
    public List<int> levelDropAreaCollectableCountsToPass = new List<int>();
    public Color levelGroundColor, levelSideColor, levelDropAreaColor;

    private void Update()
    {
        CheckIsThereALevel();
        CheckPlatformChanges();
        CheckColorChanges();
        CheckForCreateNewLevel();
    }

    public void CheckForCreateNewLevel()
    {
        if (level == null && createNewLevel)
        {
            level = Instantiate(baseLevel.gameObject).GetComponent<Level>();
            level.gameObject.name = "BaseLevel";
            createNewLevel = false;
        }
    }

    private void CheckIsThereALevel()
    {
        if (level && !levelSetted)
        {
            if (PrefabUtility.IsAnyPrefabInstanceRoot(level.gameObject))
                PrefabUtility.UnpackPrefabInstance(level.gameObject, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
            levelPlatforms = new List<PlatformType>(level.platforms);
            levelPlatformCount = levelPlatforms.Count;
            dropAreaCount = level.platformObjects.FindAll(x => x.platformType == PlatformType.Drop).Count;
            foreach (var item in level.platformObjects.FindAll(x => x.platformType == PlatformType.Drop))
                levelDropAreaCollectableCountsToPass.Add(item.GetComponent<DropArea>().collectableCountToPass);
            level.dropAreaCollectableCountsToPass = new List<int>(levelDropAreaCollectableCountsToPass);
            levelSetted = true;

            levelGroundColor = level.groundColor;
            levelSideColor = level.sideColor;
            levelDropAreaColor = level.dropAreaColor;
        }
        if (!level && levelSetted)
        {
            levelPlatforms.Clear();
            levelDropAreaCollectableCountsToPass.Clear();
            levelPlatformCount = 0;
            levelSetted = false;
        }
    }

    private void SetDropAreasCollectableCounts()
    {
        if (dropAreaCount < level.platformObjects.FindAll(x => x.platformType == PlatformType.Drop).Count)
            levelDropAreaCollectableCountsToPass.Add(0);
        if (dropAreaCount > level.platformObjects.FindAll(x => x.platformType == PlatformType.Drop).Count)
            levelDropAreaCollectableCountsToPass.Remove(levelDropAreaCollectableCountsToPass[levelDropAreaCollectableCountsToPass.Count - 1]);
        int passIndex = 0;
        foreach (var item in level.platformObjects.FindAll(x => x.platformType == PlatformType.Drop))
        {
            item.GetComponent<DropArea>().collectableCountToPass = levelDropAreaCollectableCountsToPass[passIndex];
            passIndex++;
        }
        level.dropAreaCollectableCountsToPass = new List<int>(levelDropAreaCollectableCountsToPass);
        dropAreaCount = level.platformObjects.FindAll(x => x.platformType == PlatformType.Drop).Count;
    }

    private void CheckColorChanges()
    {
        if (level && levelSetted)
        {
            if (levelGroundColor != level.groundColor)
                level.groundColor = levelGroundColor;
            if (levelSideColor != level.sideColor)
                level.sideColor = levelSideColor;
            if (levelDropAreaColor != level.dropAreaColor)
                level.dropAreaColor = levelDropAreaColor;
        }
    }

    private void CheckPlatformChanges()
    {
        if (level && levelSetted)
        {
            if (levelPlatforms.Count > levelPlatformCount)
            {
                level.platforms = new List<PlatformType>(levelPlatforms);
                levelPlatformCount = levelPlatforms.Count;
                for (int i = 0; i < levelPlatforms.Count; i++)
                {
                    if (i < level.platformObjects.Count)
                    {
                        Vector3 _psp = (i <= 0 ? Vector3.zero : level.platformObjects[i - 1].endPoint.position);
                        Platform _platform = SpawnPlatformPrefab(platforms.Find(x => x.platformType == levelPlatforms[i]).gameObject, _psp).GetComponent<Platform>();
                        DestroyImmediate(level.platformObjects[i].gameObject);
                        level.platformObjects[i] = _platform;
                    }
                    else
                    {
                        Vector3 _psp = (i <= 0 ? Vector3.zero : level.platformObjects[i - 1].endPoint.position);
                        Platform _platform = SpawnPlatformPrefab(platforms.Find(x => x.platformType == levelPlatforms[i]).gameObject, _psp).GetComponent<Platform>();
                        level.platformObjects.Add(_platform);
                    }
                }

                SetDropAreasCollectableCounts();
            }

            if (levelPlatforms.Count < levelPlatformCount || !CompareLists(levelPlatforms, level.platforms))
            {
                int platformParentsChildCount = level.platformsParent.childCount;
                for (int i = 0; i < platformParentsChildCount; i++)
                    DestroyImmediate(level.platformsParent.GetChild(0).gameObject);
                level.platforms = new List<PlatformType>(levelPlatforms);
                levelPlatformCount = levelPlatforms.Count;
                level.platformObjects.Clear();
                for (int i = 0; i < levelPlatformCount; i++)
                {
                    Vector3 _psp = (i <= 0 ? Vector3.zero : level.platformObjects[i - 1].endPoint.position);
                    Platform _platform = SpawnPlatformPrefab(platforms.Find(x => x.platformType == levelPlatforms[i]).gameObject, _psp).GetComponent<Platform>();
                    level.platformObjects.Add(_platform);
                }

                SetDropAreasCollectableCounts();
            }

            if (!CompareLists(levelDropAreaCollectableCountsToPass, level.dropAreaCollectableCountsToPass))
                SetDropAreasCollectableCounts();
        }
    }

    private GameObject SpawnPlatformPrefab(GameObject prefab, Vector3 spawnPos)
    {

        GameObject _platformObj = PrefabUtility.InstantiatePrefab(prefab, level.platformsParent) as GameObject;
        _platformObj.transform.position = spawnPos;
        _platformObj.transform.rotation = Quaternion.identity;
        return _platformObj;
    }

    private bool CompareLists<T>(List<T> aListA, List<T> aListB)
    {
        if (aListA == null || aListB == null || aListA.Count != aListB.Count)
            return false;
        if (aListA.Count == 0)
            return true;
        Dictionary<T, int> lookUp = new Dictionary<T, int>();

        for (int i = 0; i < aListA.Count; i++)
        {
            int count = 0;
            if (!lookUp.TryGetValue(aListA[i], out count))
            {
                lookUp.Add(aListA[i], 1);
                continue;
            }
            lookUp[aListA[i]] = count + 1;
        }
        for (int i = 0; i < aListB.Count; i++)
        {
            int count = 0;
            if (!lookUp.TryGetValue(aListB[i], out count))
            {
                return false;
            }
            count--;
            if (count <= 0)
                lookUp.Remove(aListB[i]);
            else
                lookUp[aListB[i]] = count;
        }
        return lookUp.Count == 0;
    }
}
#endif