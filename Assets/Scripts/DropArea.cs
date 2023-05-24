using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class DropArea : MonoBehaviour
{
    public int collectableCountToPass;
    private int collectableCount;
    public GameObject[] dropGrounds;
    public ParticleSystem[] confettiFxs;
    public GameObject flatGround, barriers;
    public TextMeshProUGUI countText;
    [HideInInspector] public bool platformOpened;
    private List<GameObject> collectablesInsideArea = new List<GameObject>();

    private void Start()
    {
        countText.text = collectableCount + "/" + collectableCountToPass;
    }

    private IEnumerator OpenPlatform()
    {
        platformOpened = true;
        CarrierTool.Instance.isDropAreaPassed = true;
        yield return new WaitForSeconds(1.5f);
        foreach (var item in collectablesInsideArea)
            Destroy(item);
        flatGround.SetActive(true);
        foreach (var item in dropGrounds)
            item.SetActive(false);
        foreach (var item in confettiFxs)
            item.Play();
        barriers.GetComponent<Animation>().Play();
        flatGround.transform.DOMoveY(-0.52f, 1f).OnComplete(() => CarrierTool.Instance.OnDropAreaPassed()).SetEase(Ease.OutElastic);
        UIManager.Instance.IncreaseLevelProgress();
    }

    public void AddCollectable(GameObject collectable)
    {
        collectableCount++;
        countText.text = collectableCount + "/" + collectableCountToPass;
        if (platformOpened) return;
        if (collectableCount >= collectableCountToPass)
            StartCoroutine(OpenPlatform());
        collectablesInsideArea.Add(collectable);
    }
}