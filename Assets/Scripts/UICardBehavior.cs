using System;
using UnityEngine;
using UnityEngine.EventSystems;


public class UICardBehavior : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler , IPointerClickHandler
{
    public event Action onMouseEnter;
    public event Action onMouseExit;
    public event Action onMouseClick;

    public void OnPointerEnter(PointerEventData eventData)
    {
        onMouseEnter.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onMouseExit.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onMouseClick.Invoke();
    }

}
