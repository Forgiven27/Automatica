using UnityEngine;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;

namespace DullVersion
{
    public class HandIntersects : MonoBehaviour
    {
        List<ObjectInfo> items = new();
        private void OnTriggerEnter(Collider other)
        {
            
            if (other.TryGetComponent<ObjectInfo>(out ObjectInfo item))
            {
                if (!items.Contains(item))
                {
                    items.Add(item);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {

            if (other.TryGetComponent<ObjectInfo>(out ObjectInfo item))
            {
                if (items.Contains(item))
                {
                    items.Remove(item);
                }
            }
        }

        public bool TryGetItem(out ObjectInfo itemInfo)
        {

            if (items.Count >= 1) 
            {
                itemInfo = items[0]; 
                return true;
            }
            else
            {
                itemInfo = null;
                return false;
            }
        }
    }
}