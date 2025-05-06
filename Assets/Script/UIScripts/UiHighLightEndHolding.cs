using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.EventSystems;

public class UiHighLightEndHolding : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    private Color UITextColor;
    public GameObject textObj, ImageObj;
    private bool clicked = false;

    void Start()
    {
        UITextColor = new Color(0.6462264f, 0.9857091f, 1, 1);
        textObj = gameObject.transform.Find("Text").gameObject;
        ImageObj = gameObject.transform.Find("Image").gameObject;
    }
    void Update()
    {

    }
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        
    }
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if (pointerEventData.button == PointerEventData.InputButton.Left)
        {
            print("true");
            textObj.GetComponent<TextMeshProUGUI>().color = new Color(0.7843137f, 0.7843137f, 0.7843137f, 1);
            ImageObj.GetComponent<Image>().color = new Color(0.7843137f, 0.7843137f, 0.7843137f, 1);
        }
    }
    public void OnPointerUp(PointerEventData pointerEventData)
    {
        if (pointerEventData.button == PointerEventData.InputButton.Left)
        {
            print("true");
            textObj.GetComponent<TextMeshProUGUI>().color = UITextColor;
            ImageObj.GetComponent<Image>().color = Color.white;
        }
    }
}
