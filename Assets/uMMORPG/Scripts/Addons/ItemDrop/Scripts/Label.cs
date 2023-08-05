using System;
using TMPro;
using UnityEngine;

public class Label : MonoBehaviour
{
    public TMP_Text text;
    public RectTransform rectTransform;

    public Transform background;
   
    public MeshRenderer meshRenderer;
    MaterialPropertyBlock matBlock;

    Vector2 screenSize;

    void Awake()
    {
        matBlock = new MaterialPropertyBlock();

        screenSize = new Vector2(Screen.width, Screen.height);
    }

    public void Show(Vector2 position, string text)
    {
        if (!gameObject.activeSelf)
        {
            if (this.text.text != text)
            {
                this.text.SetText(text);
                SetSize();
            }

            gameObject.SetActive(true);
        }

        if (rectTransform.anchoredPosition != position)
            rectTransform.anchoredPosition = position;

        if (ItemDropSettings.Settings.fitLabelsIntoScreen)
            FitIntoScreen();
    }
    public void SetTitle(string text)
    {
        if (this.text.text != text)
        {
            this.text.SetText(text);
            SetSize();
        }
    }


     
    void FitIntoScreen()
    {        
        Rect rect = GetScreenRect();
        Vector2 minPosition = LootManager.instance.labelCamera.WorldToViewportPoint(rect.min);
        Vector2 maxPosition = LootManager.instance.labelCamera.WorldToViewportPoint(rect.max);
        float leftOverflow = Math.Min(minPosition.x, 0);
        float rightOverflow = Math.Max(maxPosition.x - 1, 0);
        float topOverflow = Math.Max(maxPosition.y - 1, 0);
        float horizontalOffset = -(leftOverflow + rightOverflow) * screenSize.x / ItemDropSettings.Settings.pixelsPerUnit;
        float verticalOffset = -topOverflow * screenSize.y / ItemDropSettings.Settings.pixelsPerUnit;
        rectTransform.anchoredPosition += new Vector2(horizontalOffset, verticalOffset);        
    }
    
    public Rect GetScreenRect()
    {
        Rect rect = new Rect(rectTransform.anchoredPosition, rectTransform.rect.size);
        rect.x -= rect.width / 2;

        return rect;
    }

    public void SetSize()
    {
        Vector2 bounds = text.GetPreferredValues(text.text);
        float width = bounds.x + 0.5f;

        Vector2 size = new Vector2(width, background.localScale.y);
        text.rectTransform.sizeDelta = size;
        background.localScale = size;
    }

    public void SetColor(Color color)
    {
        matBlock.SetColor("_Color", color);
        meshRenderer.SetPropertyBlock(matBlock);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        //LootManager.instance.GetPlayer()?.ControlRaycast(false);
    }
}
