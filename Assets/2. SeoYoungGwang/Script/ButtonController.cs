using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonController : MonoBehaviour, IPointerDownHandler,  IPointerUpHandler
{
    public bool onClick = false;
    public bool offClick = false;
    public bool isClick = false;


    public void OnPointerDown(PointerEventData eventData)
    {
        offClick = false;
        onClick = true;
        isClick = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onClick = false;
        offClick = true;
        isClick = false;
    }
}
