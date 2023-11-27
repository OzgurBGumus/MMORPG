using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class UIPrivateChat : MonoBehaviour
{
    public static UIPrivateChat singleton;
    public Transform content;

    bool eatActivation;

    private string previousText = "";
    private bool convertingText = false;
    public bool inputFromBackend;
    public Dictionary<int, string> links;
    [HideInInspector] public List<UIPrivateMessage> messageWindows;
    public Vector2 basePosition;

    private void Awake()
    {
        messageWindows = new List<UIPrivateMessage>();
    }

    void Update()
    {
        //Player player = Player.localPlayer;
    }


    public UIPrivateChat()
    {
        // assign singleton only once (to work with DontDestroyOnLoad when
        // using Zones / switching scenes)
        if (singleton == null) singleton = this;
    }


    public void AddMessage(ChatMessage message, bool outgoing)
    {
        UIPrivateMessage chat;
        if (messageWindows.Find(x => x.name == message.sender) == null)
        {
            GameObject go = Instantiate(message.textPrefab, content.transform, false);
            chat = go.GetComponent<UIPrivateMessage>();
            chat.name = message.sender;
            chat.senderNameText.text = message.sender;
            Vector2 newPosition = new Vector2(basePosition.x, basePosition.y);
            while (messageWindows.Exists(window => (Vector2)window.transform.position == newPosition))
            {
                newPosition.y += 25;
            }
            chat.transform.position = newPosition;
            ///User received a new chat 
            if (!Player.localPlayer.name.Equals(message.sender))
            {
                //make window smaller for other user
                chat.window.TriggerWindowSmaller();
            }

            //add this new chat into the list.
            messageWindows.Add(chat);
        }
        else
        {
            chat = content.Find(message.sender).GetComponent<UIPrivateMessage>();
        }
        chat.AddMessage(message, outgoing);

        //AutoScroll();
    }

    public void UIPrivateMessageDestroyed(UIPrivateMessage chat)
    {
        messageWindows.Remove(chat);
    }
}

