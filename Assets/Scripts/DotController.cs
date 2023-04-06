using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DotController : MonoBehaviour, IPointerClickHandler
{
    public UnityAction <DotController> OnDotClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnDotClick?.Invoke(this);
    }
}
