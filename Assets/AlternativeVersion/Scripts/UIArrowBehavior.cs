using DG.Tweening;
using UnityEngine;

namespace DullVersion
{
    public class UIArrowBehavior : MonoBehaviour
    {
        Sequence sequence;
        void Start()
        {
            sequence = DOTween.Sequence()
                .Append(transform.DOMove(transform.position + Vector3.up, 1).SetEase(Ease.InOutCubic))
                .Join(transform.DORotate(new Vector3(0, 180, 0), 1, RotateMode.WorldAxisAdd).SetEase(Ease.Linear))
                .Append(transform.DOMove(transform.position, 1).SetEase(Ease.InOutCubic))
                .Join(transform.DORotate(new Vector3(0, 180, 0), 1, RotateMode.WorldAxisAdd).SetEase(Ease.Linear))
                .SetAutoKill(false);
        }

        private void OnEnable()
        {
            if (sequence != null)
            {
                sequence.Play();
            }
        }

        void Update()
        {
            if (sequence.IsComplete() && !sequence.IsPlaying())
            {
                sequence.Restart();
            }
        }

        private void OnDisable()
        {
            if (sequence != null)
            {
                sequence.Pause();
            }
        }

    }
}