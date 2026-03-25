using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace DullVersion
{
    public class ManipulatorController : MonoBehaviour
    {
        [SerializeField] private string _name;

        [SerializeField] private JointController[] _joints;
        [SerializeField] private HandIntersects _handIntersects;
        [SerializeField] private Transform _handGrabPlacement;
        float _time = 0f;
        bool _isScannig = false;
        ItemFilter _filter;
        bool _isGrabbed;
        ObjectInfo _grabbedObject;

        public void RotateJoint(int index, float angle)
        {
            if (index >= _joints.Length || index < 0) return;
            _joints[index].RotateTo(angle);
        }

        public void AddWaitTime(float value)
        {
            _time += value;
        }

        public void Grab()
        {
            if (_isGrabbed) return;
            if (_handIntersects.TryGetItem(out ObjectInfo objectInfo))
            {
                _grabbedObject = objectInfo;
                _grabbedObject.TryGetComponent(out Rigidbody rb);
                if (_filter.isEmptyFilter)
                {
                    _grabbedObject.transform.SetParent(_handGrabPlacement);
                    _isGrabbed = true;
                    if (rb != null)
                    {
                        rb.isKinematic = true;
                        rb.useGravity = false;
                    }
                }
                else
                {
                    if (_filter.CheckObject(objectInfo))
                    {
                        _grabbedObject.transform.SetParent(_handGrabPlacement);
                        _isGrabbed = true;
                        if (rb != null)
                        {
                            rb.isKinematic = true;
                        }
                    }
                }
            }
        }

        public void Release()
        {
            if (!_isGrabbed) return;
            if (_grabbedObject.TryGetComponent(out Rigidbody rb))
            {
                rb.isKinematic = false;
            }
            _grabbedObject.transform.SetParent(null);
            _grabbedObject = null;
            _isGrabbed = false;
        }

        public void EnableScaner()
        {
            _isScannig = true;
        }

        public void DisableScaner()
        {
            _isScannig = false;
        }

        public void SetFilter(ItemFilter filter)
        {
            _filter = filter;
        }

        
        
        private void Update()
        {
            if(_time > 0f)
            {
                _time -= Time.deltaTime;
                return;
            }
            if (_isScannig)
            {
                Grab();
                if (_isGrabbed) _isScannig = false;
            }
        }

    }

    public struct ItemFilter
    {
        public bool isEmptyFilter 
        { 
            get 
            {
                if ((itemsWhiteList != null && itemsWhiteList.Count > 0) ||
                    (itemsBlackList != null && itemsBlackList.Count > 0) ||
                    (qualityWhiteList != null && qualityWhiteList.Count > 0) ||
                    (qualityBlackList != null && qualityBlackList.Count > 0))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        public List<Item> itemsWhiteList;
        public List<Item> itemsBlackList;
        public List<ItemQuality> qualityWhiteList;
        public List<ItemQuality> qualityBlackList;


        public bool CheckObject(ObjectInfo objectInfo)
        {
            if (isEmptyFilter) return true;
            if ((itemsBlackList != null  && itemsBlackList.Count > 0 && itemsBlackList.Contains(objectInfo.item)) ||
                (qualityBlackList != null && qualityBlackList.Count > 0 && qualityBlackList.Contains(objectInfo.quality)))
            {
                return false;
            }
            if ((itemsWhiteList != null && itemsWhiteList.Count > 0 && !itemsWhiteList.Contains(objectInfo.item)) ||
                (qualityWhiteList != null && qualityWhiteList.Count > 0 && !qualityWhiteList.Contains(objectInfo.quality)))
            {
                return false;
            }
            return true;
        }
    }
}