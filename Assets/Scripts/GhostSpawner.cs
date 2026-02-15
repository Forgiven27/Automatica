using System.Linq;
using UnityEngine;

public class GhostSpawner : MonoBehaviour
{
    public GameObject InstantiateGhost(GameObject gameObject)
    {
        var ghost = Instantiate(gameObject, transform);
        int ghostLayer = LayerMask.NameToLayer("GhostBuilding");
        ghost.layer = ghostLayer;
        var renderers = ghost.GetComponentsInChildren<Transform>();
        for (int i = 0;i < renderers.Count(); i++)
        {
            renderers[i].gameObject.layer = ghostLayer;
        }
        return ghost;
    }

    public void DestroyGhost(GameObject gameObject)
    {
        Destroy(gameObject);
    }
}
