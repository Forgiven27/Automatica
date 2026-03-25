using UnityEngine;
namespace DullVersion
{
    public class ItemSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject itemPrefab;
        [SerializeField] private float cooldown;
        [SerializeField] private bool onOff;

        [SerializeField] private Material _lowQualityMaterial;
        [SerializeField] private Material _mediumQualityMaterial;
        [SerializeField] private Material _highQualityMaterial;

        float timer = 0f;
        GameObject currentGO;


        void Update()
        {
            if (!onOff) return;
            if (timer > 0f) timer -= Time.deltaTime;
            else
            {
                currentGO = Instantiate(itemPrefab, transform.position, Quaternion.identity,transform);
                if (currentGO.TryGetComponent<ObjectInfo>(out ObjectInfo objInfo))
                {
                    int quality = Random.Range(0, 3);
                    switch (quality)
                    {
                        case 0:
                            objInfo.quality = ItemQuality.LowQuality;
                            currentGO.GetComponent<MeshRenderer>().material = _lowQualityMaterial;
                            break;
                        case 1:
                            objInfo.quality = ItemQuality.MediumQuality;
                            currentGO.GetComponent<MeshRenderer>().material = _mediumQualityMaterial;
                            break;
                        case 2:
                            objInfo.quality = ItemQuality.HighQuality;
                            currentGO.GetComponent<MeshRenderer>().material = _highQualityMaterial;
                            break;
                    }
                }
                timer = cooldown;
            }
        }
    }
}