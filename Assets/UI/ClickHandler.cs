using UnityEngine;
using UnityEngine.EventSystems;
using System;


public class ClickHandler : MonoBehaviour, IPointerClickHandler
{
    public Action<PointerEventData> OnClick;
    

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke(eventData);
    }
}
