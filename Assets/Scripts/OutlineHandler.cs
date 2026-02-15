using UnityEngine;
using System.Collections.Generic;

public class OutlineHandler : MonoBehaviour
{
    [SerializeField]private List<MeshRenderer> renderers = new List<MeshRenderer>();
    [SerializeField]private RenderingLayerMask layerMask;
    public void ActiveOutline()
    {
        for (int i = 0; i < renderers.Count; i++)
        {
            renderers[i].renderingLayerMask |= layerMask.value;
        }
    }
    public void DeactiveOutline()
    {
        for (int i = 0; i < renderers.Count; i++)
        {
            renderers[i].renderingLayerMask = ~renderers[i].renderingLayerMask | ~layerMask.value;
        }
    }

}
