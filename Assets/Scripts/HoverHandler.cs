using UnityEngine;
using UnityEngine.Events;

public class HoverHandler : MonoBehaviour
{

    [SerializeField] private UnityEvent onMouseEnter;
    [SerializeField] private UnityEvent onMouseExit;

    private void OnMouseEnter()
    {
        onMouseEnter?.Invoke();
    }

    private void OnMouseExit()
    {
        onMouseExit?.Invoke();
    }

}
