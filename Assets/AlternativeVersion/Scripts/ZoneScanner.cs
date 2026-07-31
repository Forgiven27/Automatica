using System.Collections.Generic;
using System;
using UnityEngine;
namespace DullVersion {
    public class ZoneScanner : MonoBehaviour
    {
        [SerializeField] Item[] _itemsWhiteList;
        [SerializeField] Item[] _itemsBlackList;
        [SerializeField] ItemQuality[] _qualityWhiteList;
        [SerializeField] ItemQuality[] _qualityBlackList;



        public event Action<ObjectInfo> OnInputInfo;
        public event Action<ObjectInfo> OnOutputInfo;

        private List<ObjectInfo> _innerObjects = new();

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out ObjectInfo objInfo))
            {
                if (!_innerObjects.Contains(objInfo))
                {
                    OnInputInfo.Invoke(objInfo);
                    _innerObjects.Add(objInfo);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out ObjectInfo objInfo))
            {
                if (_innerObjects.Contains(objInfo))
                {
                    OnOutputInfo.Invoke(objInfo);
                    _innerObjects.Remove(objInfo);
                }
                    
            }
        }
    }
}