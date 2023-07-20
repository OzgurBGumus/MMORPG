using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIQuestInfoSlot : MonoBehaviour, IPointerClickHandler
{
    public Button nameButton;
    public TMP_Text descriptionText;

    public void OnPointerClick(PointerEventData eventData)
    {
        // find the chat component in the parents
        GetComponentInParent<UIQuestInfos>().OnEntryClicked(eventData, this);
    }
}
