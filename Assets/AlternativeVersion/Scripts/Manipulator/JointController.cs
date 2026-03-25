using UnityEngine;
using DG.Tweening;

namespace DullVersion
{
    public class JointController : MonoBehaviour
    {
        [SerializeField] Axis rotateAxis;
        [SerializeField] float _maxAngle;
        [SerializeField] float _minAngle;
        [SerializeField] float _defaultAngle;

        Vector3 axis;
        public void SetDefaultAngle(float angle)
        {
            if (_defaultAngle > _maxAngle || _defaultAngle < _minAngle)
            {
                throw new System.Exception("—тандартный угол выходит за пределы");
            }
            else
            {
                _defaultAngle = angle;
            }
        }

        public void SetMinMaxAngle(float min, float max){
            _minAngle = min;
            _maxAngle = max;
        }

        private void Start()
        {
            switch (rotateAxis)
            {
                case Axis.X:
                    axis = Vector3.right; 
                    break;
                case Axis.Y: 
                    axis = Vector3.up; 
                    break;
                case Axis.Z: 
                    axis = Vector3.forward; 
                    break;
                default:
                    axis = Vector3.up;
                    break;
            }

            //transform.rotation = Quaternion.AngleAxis(_defaultAngle, axis); 
        }


        public void RotateTo(float angle)
        {
            transform.DOLocalRotate(angle*axis, 1);
            //transform.rotation = Quaternion.AngleAxis(angle, axis);
        }

        
    }
}