using UnityEngine;

public class CollectableCounter : MonoBehaviour
{
    private DropArea dropArea;

    private void Awake()
    {
        dropArea = GetComponentInParent<DropArea>();
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.GetComponent<Collectable>() && !GameManager.Instance.isLevelFail)
            dropArea.AddCollectable(coll.gameObject);
    }
}
