using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonController : MonoBehaviour, IPointerDownHandler,  IPointerUpHandler
{
    public bool OnClick = false;


    public void OnPointerDown(PointerEventData eventData)
    {
        OnClick = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnClick = false;
    }
}
