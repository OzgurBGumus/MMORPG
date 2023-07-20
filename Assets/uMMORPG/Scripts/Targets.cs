using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Targets : MonoBehaviour, IPointerClickHandler
{
    private readonly List<RaycastResult> m_RaycastResultCache = new List<RaycastResult>();
    public static Vector3 positionBandit = new Vector3(10, 2, 5);
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            TMP_Text clickedText = GetClickedText(eventData.position);
            if(clickedText != null)
            {
                string clickedWord = GetClickedWord(clickedText);
                if (clickedWord == "Bandit")
                {
                    TargetBandit();
                }
            }
        }
    }
    private TMP_Text GetClickedText(Vector2 clickPosition)
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = clickPosition;

        EventSystem.current.RaycastAll(pointerEventData, m_RaycastResultCache);

        foreach (RaycastResult result in m_RaycastResultCache)
        {
            TMP_Text clickedText = result.gameObject.GetComponent<TMP_Text>();
            if (clickedText != null)
            {
                // Ensure the clicked text is a child of SlotQuestInfo
                if (clickedText.transform.IsChildOf(transform))
                {
                    m_RaycastResultCache.Clear();
                    return clickedText;
                }
            }
        }

        m_RaycastResultCache.Clear();
        return null;
    }
    private string GetClickedWord(TMP_Text clickedText)
    {
        if (clickedText != null)
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(clickedText, Input.mousePosition, null);
            if (linkIndex != -1)
            {
                string linkID = clickedText.textInfo.linkInfo[linkIndex].GetLinkID();
                return linkID;
            }
        }
        return null;
    }

    private void SpecificWordClicked()
    {
        // Perform the desired action when the specific word is clicked
    }



    public void TargetBandit()
    {
        Debug.Log("Bandit Target Clicked.");
    }
}
