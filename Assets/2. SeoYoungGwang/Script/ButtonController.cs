using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonController : MonoBehaviour, IPointerDownHandler,  IPointerUpHandler
{
    public bool onClick = false;
    public bool OffClick = false;


    public void OnPointerDown(PointerEventData eventData)
    {
        onClick = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OffClick = true;
    }
}
