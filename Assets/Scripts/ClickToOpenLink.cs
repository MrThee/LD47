using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ClickToOpenLink : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public string url = "https://twitter.com/PhillipWitz";
    public TMPro.TextMeshProUGUI textField;

    public void OnPointerClick(PointerEventData data){
        Application.OpenURL(url);
    }

    public void OnPointerEnter(PointerEventData data){
        textField.color = Color.cyan;
    }

    public void OnPointerExit(PointerEventData data){
        textField.color = Color.white;
    }
}
