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

        List<Operation> operations = new List<Operation>();
        int _opIndex = 0;

        public void AddOperation(Operation operation)
        {
            operations.Add(operation);
        }

        public void ClearOperations()
        {
            operations.Clear();
        }

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
            if (_time > 0f)
            {
                _time -= Time.deltaTime;
                return;
            }
            if (_isScannig)
            {
                Grab();
                if (_isGrabbed) _isScannig = false;
                else return;
            }
            operations[_opIndex].Execute(this);
            _opIndex++;
        }

    }
}