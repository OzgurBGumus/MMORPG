using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPrivateMessage : MonoBehaviour
{
    public UIWindow window;
    public Button hideButton;
    public GameObject panel;
    public GameObject content;
    public GameObject PrivateMessageEntryPrefab;
    public Text senderNameText;
    public TMP_InputField messageInput;
    public Button sendButton;

    public bool unreadMessage;
    
    public Color color1;
    public Color color2;
    [HideInInspector] public bool lastWasInactive;

    public KeyCode[] activationKeys = { KeyCode.Return, KeyCode.KeypadEnter };
    bool eatActivation;
    private void Start()
    {
        StartCoroutine(ChangeColorCoroutine());
    }

    void OnDestroy()
    {
        transform.parent.GetComponent<UIPrivateChat>().UIPrivateMessageDestroyed(this);
    }
    // Update is called once per frame
    void Update()
    {
        Player player = Player.localPlayer;
        if(player != null)
        {
            if (panel.activeSelf)
            {
                if (lastWasInactive) unreadMessage = false;
                lastWasInactive = false;


                // character limit
                messageInput.characterLimit = player.chat.maxLength;

                // activation (ignored once after deselecting, so it doesn't immediately
                // activate again)
                if (Utils.AnyKeyDown(activationKeys) && !eatActivation)
                    messageInput.Select();
                eatActivation = false;

                // end edit listener
                messageInput.onEndEdit.SetListener((value) => {
                    // submit key pressed? then submit and set new input text
                    if (Utils.AnyKeyDown(activationKeys))
                    {
                        string newinput = player.chat.OnSubmit("/w "+ name +" " + value);
                        messageInput.text = newinput;
                        messageInput.MoveTextEnd(false);
                        eatActivation = true;
                    }

                    // unfocus the whole chat in any case. otherwise we would scroll or
                    // activate the chat window when doing wsad movement afterwards
                    UIUtils.DeselectCarefully();
                });

                //messageInput.onValidateInput += ValidateInput;
                //messageInput.onValueChanged.AddListener(HandleValueChanged);
                // send button
                sendButton.onClick.SetListener(() => {
                    // submit and set new input text
                    string newinput = player.chat.OnSubmit("/w " + name + " " + messageInput.text);
                    messageInput.text = newinput;
                    messageInput.MoveTextEnd(false);

                    // unfocus the whole chat in any case. otherwise we would scroll or
                    // activate the chat window when doing wsad movement afterwards
                    UIUtils.DeselectCarefully();
                });



            }
            else
            {
                lastWasInactive = true;
            }
        }
        
    }

    public void AddMessage(ChatMessage message, bool outgoing)
    {
        GameObject go = Instantiate(PrivateMessageEntryPrefab, content.transform, false);
        PrivateMessageEntry privateMessageEntry = go.GetComponent<PrivateMessageEntry>();
        privateMessageEntry.Fill(outgoing, message.message);
        unreadMessage = true;
    }

    private IEnumerator ChangeColorCoroutine()
    {
        while (true) // This coroutine runs indefinitely
        {
            if (unreadMessage)
            {
                if (window.GetComponent<Image>().color == color1)
                {
                    window.GetComponent<Image>().color = color2;
                }
                else
                {
                    window.GetComponent<Image>().color = color1;
                }
            }
            else
            {
                window.GetComponent<Image>().color = color1;
            }

            yield return new WaitForSeconds(1.0f); // Wait for 1 second
        }
    }

}


