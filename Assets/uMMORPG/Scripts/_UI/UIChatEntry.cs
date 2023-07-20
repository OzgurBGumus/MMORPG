using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;
using System.Text;

public class UIChatEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TMP_Text text;
    public UIShowToolTip tooltip;

    // keep all the message info in case it's needed to reply etc.
    [HideInInspector] public ChatMessage message;

    private void Update()
    {
        // only build tooltip while it's actually shown. this
        // avoids MASSIVE amounts of StringBuilder allocations.
        
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
        var text = GetComponent<TextMeshProUGUI>();
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(text, Input.mousePosition, null);
            if(linkIndex > -1)
            {
                //var linkInfo = text.textInfo.linkInfo[linkIndex];
                //var linkId = linkInfo.GetLinkID();
                string b64 = message.links[linkIndex];
                int index = b64.IndexOf(':');
                var data = DecodeBase64(b64.Substring(index + 1));

                // only build tooltip while it's actually shown. this
                // avoids MASSIVE amounts of StringBuilder allocations.
                tooltip.enabled = true;
                tooltip.text = data;
            }
        }

        // find the chat component in the parents
        GetComponentInParent<UIChat>().OnEntryClicked(this);
    }
    private string DecodeBase64(string base64String)
    {
        byte[] bytes = Convert.FromBase64String(base64String);
        return Encoding.UTF8.GetString(bytes);
    }

}
