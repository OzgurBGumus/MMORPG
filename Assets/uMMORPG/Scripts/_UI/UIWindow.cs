// Adds window like behaviour to UI panels, so that they can be moved and closed
// by the user.
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum CloseOption
{
    DoNothing,
    DeactivateWindow,
    DestroyWindow
}

public class UIWindow : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // close option
    public CloseOption onClose = CloseOption.DeactivateWindow;

    // currently dragged UIWindow in case it's needed somewhere else
    public static UIWindow currentlyDragged;
    public bool isDraggable = true;
    // cache
    Transform window;
    Transform panelsParent;
    public Button smallerButton;
    //smaller view
    bool isSmall = false;
    void Awake()
    {
        // cache the parent window
        window = transform.parent;
        panelsParent = window.parent;
    }

    public void HandleDrag(PointerEventData d)
    {
        // send message in case the parent needs to know about it
        window.SendMessage("OnWindowDrag", d, SendMessageOptions.DontRequireReceiver);

        // move the parent
        window.Translate(d.delta);
    }

    public void OnBeginDrag(PointerEventData d)
    {
        window.transform.SetAsLastSibling();
        if (isDraggable)
        {
            currentlyDragged = this;
            HandleDrag(d);
        }
        
    }

    public void OnDrag(PointerEventData d)
    {
        if (isDraggable)
        {
            HandleDrag(d);
        }
    }

    public void OnEndDrag(PointerEventData d)
    {
        if (isDraggable)
        {
            HandleDrag(d);
            currentlyDragged = null;
        }
    }

    // OnClose is called by the close button via Inspector Callbacks
    public void OnClose()
    {
        // send message in case it's needed
        // note: it's important to not name it the same as THIS function to avoid
        //       a deadlock
        panelsParent.SendMessage("OnWindowClose", SendMessageOptions.DontRequireReceiver);

        // hide window
        if (onClose == CloseOption.DeactivateWindow)
            window.gameObject.SetActive(false);

        // destroy if needed
        if (onClose == CloseOption.DestroyWindow)
            Destroy(window.gameObject);
    }

    public void TriggerWindowSmaller()
    {
        window.transform.SetAsLastSibling();
        if (isSmall)
        {
            float additionalHeigth = 0;
            UIWindow uiWindowElement;
            foreach (Transform childTransform in window)
            {
                if (!childTransform.TryGetComponent<UIWindow>(out uiWindowElement))
                {
                    // Add the child GameObject to the list
                    childTransform.gameObject.SetActive(true);
                    additionalHeigth += childTransform.GetComponent<RectTransform>().rect.height;
                }
            }

            
            RectTransform rectTransform = GetComponent<RectTransform>();
            Vector2 vectr = new Vector2(rectTransform.position.x, rectTransform.position.y- (additionalHeigth / 2));
            rectTransform.Translate(vectr);
            //(additionalHeigth / 2);

            smallerButton.GetComponentInChildren<Text>().text = "-";
            isSmall = false;
            
        }
        else
        {
            UIWindow uiWindowElement;
            foreach (Transform childTransform in window)
            {
                if (!childTransform.TryGetComponent<UIWindow>(out uiWindowElement))
                {
                    // Add the child GameObject to the list
                    childTransform.gameObject.SetActive(false);
                }
            }
            smallerButton.GetComponentInChildren<Text>().text = "□";
            isSmall = true;
        }
        //// send message in case it's needed
        //// note: it's important to not name it the same as THIS function to avoid
        ////       a deadlock
        //if (window.gameObject.activeSelf)
        //{
        //    panelsParent.SendMessage("OnWindowClose", SendMessageOptions.DontRequireReceiver);
        //}
        //Transform sibling = window.GetChild(1);
        //sibling.gameObject.SetActive(!sibling.gameObject.activeSelf);

    }
}
