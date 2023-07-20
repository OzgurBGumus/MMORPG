using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class UIChat : MonoBehaviour
{
    public static UIChat singleton;
    public GameObject panel;
    public TMP_InputField messageInput;
    public Button sendButton;
    public Transform content;
    public ScrollRect scrollRect;
    public KeyCode[] activationKeys = {KeyCode.Return, KeyCode.KeypadEnter};
    public int keepHistory = 100; // only keep 'n' messages

    bool eatActivation;

    private string previousText = "";
    private bool convertingText = false;
    public bool inputFromBackend;
    public Dictionary<int,string> links;

    private void Awake()
    {
        inputFromBackend = false;
        messageInput.onValueChanged.AddListener(HandleValueChanged);
        messageInput.inputValidator = new TMP_InputValidation();
    }

    void Update()
    {
        Player player = Player.localPlayer;
        if (player)
        {
            if (Input.GetKey(KeyCode.C) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
            {
                if (Application.isFocused && messageInput.isFocused)
                {
                    // Ctrl+C was pressed while the TMP_InputField is focused
                    CopyVisibleText(messageInput.text);
                }
            }

            panel.SetActive(true);

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
                    string newinput = player.chat.OnSubmit(value);
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
                string newinput = player.chat.OnSubmit(messageInput.text);
                messageInput.text = newinput;
                messageInput.MoveTextEnd(false);

                // unfocus the whole chat in any case. otherwise we would scroll or
                // activate the chat window when doing wsad movement afterwards
                UIUtils.DeselectCarefully();
            });
        }
        else panel.SetActive(false);
    }


    public UIChat()
    {
        // assign singleton only once (to work with DontDestroyOnLoad when
        // using Zones / switching scenes)
        if (singleton == null) singleton = this;
    }
    private char ValidateInput(string text, int charIndex, char addedChar)
    {
        // Check if user input is a rich text tag start character ("<")
        if (!inputFromBackend && (addedChar == '<' || addedChar == '>'))
        {
            // Prevent user input of rich text tags
            return '\0'; // Return null character to suppress input
        }


        return addedChar; // Return the unmodified character
    }

    private void CopyVisibleText(string value)
    {
        // Remove HTML tags from the visible text
        string visibleText = RemoveHtmlTags(value);

        // Copy the visible text to the clipboard
        GUIUtility.systemCopyBuffer = visibleText;
    }

    private string RemoveHtmlTags(string text)
    {
        // Remove HTML tags from the input string
        string regex = @"<[^>]*>";
        string visibleText = System.Text.RegularExpressions.Regex.Replace(text, regex, "");

        return visibleText;
    }

    private string RemoveFirstHtmlOpeningOrClosing(string text, bool setStringPosition)
    {
        int openingTagIndex = text.IndexOf("<");
        int closingTagIndex = text.IndexOf(">");
        while (text.Length > closingTagIndex+1 && text[closingTagIndex + 1] == '<')
        {
            closingTagIndex += text.Substring(closingTagIndex + 1).IndexOf(">") + 1;
        }
        if (openingTagIndex != -1 && closingTagIndex != -1 && openingTagIndex < closingTagIndex)
        {
            string substringToRemove = text.Substring(openingTagIndex, closingTagIndex - openingTagIndex + 1);
            text = text.Remove(openingTagIndex, substringToRemove.Length);
            if (setStringPosition)
            {
                messageInput.stringPosition -= substringToRemove.Length;
            }
        }
        return text;
    }
    private string RemoveFirstHtmlOpeningWithBrecked(string text)
    {
        int openingTagIndex = text.IndexOf("[<");
        int closingTagIndex = text.IndexOf(">");
        while(text[closingTagIndex+1] == '<')
        {
            closingTagIndex += text.Substring(closingTagIndex+1).IndexOf(">")+1;
        }
        if (openingTagIndex != -1 && closingTagIndex != -1 && openingTagIndex < closingTagIndex)
        {
            string substringToRemove = text.Substring(openingTagIndex, closingTagIndex - openingTagIndex + 1);
            text = text.Remove(openingTagIndex, substringToRemove.Length);
            messageInput.stringPosition -= substringToRemove.Length;
        }
        return text;
    }
    private string RemoveFirstHtmlClosingWithBrecked(string text)
    {
        int openingTagIndex = text.IndexOf("<");
        int closingTagIndex = text.IndexOf(">]");
        if (openingTagIndex != -1 && closingTagIndex != -1 && openingTagIndex < closingTagIndex)
        {
            string substringToRemove = text.Substring(openingTagIndex, closingTagIndex - openingTagIndex + 2);
            text = text.Remove(openingTagIndex, substringToRemove.Length);
        }
        return text;
    }

    private string RemoveCorruptedHtmlElements(string text)
    {
        int startIndex = text.IndexOf("[<");
        int endIndex = text.IndexOf(">]");
        string pattern = @"<\/\w+\s*>]";
        if (endIndex == -1 && startIndex == -1)
        {
            return text;
        }
        if (endIndex >= 0 && endIndex < startIndex)
        {
            text = RemoveFirstHtmlOpeningOrClosing(text, setStringPosition: true);
            text = RemoveFirstHtmlClosingWithBrecked(text);
            text = RemoveCorruptedHtmlElements(text);
        }

        else if (endIndex == -1)
        {
            //Remove Opening:
            text = RemoveFirstHtmlOpeningWithBrecked(text);

            //Remove if we have Html Closing without brecket
            if (text.IndexOf(">") != text.IndexOf(">]"))
            {
                text = RemoveFirstHtmlOpeningOrClosing(text, setStringPosition: true);
            }
            text = RemoveCorruptedHtmlElements(text);

        }
        else if (startIndex == -1)
        {
            text = RemoveFirstHtmlClosingWithBrecked(text);

            
            if (text.IndexOf("[<") != text.IndexOf("<") - 1)
            {
                text = RemoveFirstHtmlOpeningOrClosing(text, setStringPosition:false);
            }
            Console.WriteLine(text);
            text = RemoveCorruptedHtmlElements(text);
        }
        else
        {
            int secondStart =text.Substring(startIndex + 2).IndexOf("[<");
            secondStart = secondStart != -1 ? startIndex + 2 + secondStart : -1;
            // Check if founded closing is after the second Opening, this means startIndex's closing is corrupted.
            if (secondStart != -1 && secondStart < endIndex)
            {
                text = RemoveFirstHtmlOpeningWithBrecked(text);
                secondStart = text.Substring(startIndex + 2).IndexOf("[<");
                if (secondStart > text.IndexOf("<"))
                {
                    text = RemoveFirstHtmlOpeningOrClosing(text, setStringPosition: true);
                }
                text = RemoveCorruptedHtmlElements(text);
            }
            else
            {
                string secondHalf = text.Substring(endIndex + 2);
                text = text.Substring(0, endIndex + 2) + RemoveCorruptedHtmlElements(secondHalf);
            }
            
        }
        return text;
    }
    private string RemoveNonExistHtml(string input)
    {
        int caretPosition = messageInput.stringPosition;

        if (caretPosition > 1 && caretPosition < input.Length - 1)
        {
            string beforeCaret = input.Substring(0, caretPosition);
            int lastClosingIndexBeforeCaret = beforeCaret.LastIndexOf("]");
            int lastOpeningIndexBeforeCaret = beforeCaret.LastIndexOf("[");
            if(lastOpeningIndexBeforeCaret > lastClosingIndexBeforeCaret)
            {
                input = input.Remove(lastOpeningIndexBeforeCaret, 1);
            }
        }

        return input;
    }




    private void HandleValueChanged(string value)
    {
        if (Input.GetKeyDown(KeyCode.V) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && (value == ">" || value == "<"))
        {
            // Allow complete HTML tags when pasting complete HTML content
            messageInput.SetTextWithoutNotify(previousText);
        }
        else
        {
            if (inputFromBackend)
            {
                inputFromBackend = false;
            }
            previousText = RemoveCorruptedHtmlElements(value);
            previousText = RemoveNonExistHtml(previousText);
            if (previousText != value)
            {
                inputFromBackend = true;
                
                messageInput.text = previousText;
            }

        }
        if (!convertingText) links = GetLinks();
        else convertingText = false;
    }

    public void AppendText(string htmlText)
    {
        inputFromBackend = true;
        messageInput.text += htmlText;
        messageInput.stringPosition = messageInput.text.Length;
        messageInput.ActivateInputField();
    }
    string RemoveCompletedHtmlElements(string beforeString, string afterString)
    {
        // Find all HTML elements in the before string
        var beforeTags = Regex.Matches(beforeString, @"<[^>]+>");

        // Remove completed HTML elements in the after string
        string cleanedString = afterString;
        foreach (Match tag in beforeTags)
        {
            string openingTag = tag.Value;
            string closingTag = "</" + openingTag.Substring(1);

            int openingTagIndex = cleanedString.IndexOf(openingTag);
            int closingTagIndex = cleanedString.IndexOf(closingTag);

            if (openingTagIndex != -1 && closingTagIndex == -1)
            {
                cleanedString = cleanedString.Substring(0, openingTagIndex);
            }
        }

        // Remove incomplete opening tags at the end of the string
        cleanedString = Regex.Replace(cleanedString, @"<[^>]+$", string.Empty);

        return cleanedString;
    }

    private Dictionary<int, string> GetLinks()
    {
        Dictionary<int, string> tempList = new Dictionary<int, string>();

        // Copy the key-value pairs from the original dictionary to the cloned one
        if(links != null)
        {
            foreach (var kvp in links)
            {
                tempList.Add(kvp.Key, kvp.Value);
            }
        }
        

        string[] linksInInput = messageInput.text.Split("<link=");
        Dictionary<int, string> outputLinks = new Dictionary<int, string>();
        int closingIndex = 0;
        for(int i = 1; i < linksInInput.Length; i++)
        {
            string value = linksInInput[i].IndexOf('>') != -1 ? linksInInput[i].Substring(0, linksInInput[i].IndexOf('>')) : "";
            if (!string.IsNullOrEmpty(value))
            {
                //This means this link already a Guild
                if(value.Split(":").Length == 1)
                {
                    outputLinks.Add(value.ToInt(), tempList[value.ToInt()]);
                }
                else
                {
                    outputLinks.Add(i-1, value);

                    convertingText = true;
                    messageInput.text = messageInput.text.Replace("<link="+value, "<link="+ (i-1));
                    messageInput.stringPosition = messageInput.text.Length;
                }
            }
        }

        return outputLinks;
    }


    void AutoScroll()
    {
        // update first so we don't ignore recently added messages, then scroll
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0;
    }

    public void AddMessage(ChatMessage message)
    {
        // delete old messages so the UI doesn't eat too much performance.
        // => every Destroy call causes a lag because of a UI rebuild
        // => it's best to destroy a lot of messages at once so we don't
        //    experience that lag after every new chat message
        if (content.childCount >= keepHistory) {
            for (int i = 0; i < content.childCount / 2; ++i)
                Destroy(content.GetChild(i).gameObject);
        }

        // instantiate and initialize text prefab with parent
        GameObject go = Instantiate(message.textPrefab, content.transform, false);
        go.GetComponent<TMP_Text>().text = message.Construct();
        go.GetComponent<UIChatEntry>().message = message;

        AutoScroll();
    }

    // called by chat entries when clicked
    public void OnEntryClicked(UIChatEntry entry)
    {
        // any reply prefix?
        if (!string.IsNullOrWhiteSpace(entry.message.replyPrefix))
        {
            // set text to reply prefix
            messageInput.text = entry.message.replyPrefix;

            // activate
            messageInput.Select();

            // move cursor to end (doesn't work in here, needs small delay)
            Invoke(nameof(MoveTextEnd), 0.1f);
        }
    }

    void MoveTextEnd()
    {
        messageInput.MoveTextEnd(false);
    }
}
