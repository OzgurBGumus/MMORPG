using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrivateMessageEntry : MonoBehaviour
{
    [SerializeField]
    public enum PrivateMessageEntryOwner
    {
        currentUser,
        otherUser
    }

    public PrivateMessageEntryOwner owner;
    public GameObject background;
    public TMP_Text messageText;
    // Start is called before the first frame update
    void Start()
    {
        if(owner == PrivateMessageEntryOwner.currentUser)
        {
            Vector3 newPositon = new Vector3(background.transform.position.x+20, background.transform.position.y, background.transform.position.z);
            background.transform.position = newPositon;
        }
    }

    public void Fill(bool outgoing, string message)
    {
        if (outgoing)
        {
            VerticalLayoutGroup vlg = GetComponent<VerticalLayoutGroup>();
            vlg.childAlignment = TextAnchor.MiddleRight; 
        }
        messageText.text = message;
    }
}
