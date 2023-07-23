﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DuplicateKeyComparer<TKey> : IComparer<TKey> where TKey : IComparable
{
    public int Compare(TKey x, TKey y)
    {
        int result = x.CompareTo(y);

        if (result == 0)
            return 1;   // handle equality as being greater

        return result;
    }
}

public class LootLabelLayout
{
    readonly SortedList<float, Rect> placedRects = new SortedList<float, Rect>(new DuplicateKeyComparer<float>());
    float verticalPadding;

    public LootLabelLayout(float verticalPaddingInPixels)
    {
        verticalPadding = verticalPaddingInPixels / ItemDropSettings.Settings.pixelsPerUnit;
    }

    public void Arrange(IReadOnlyCollection<KeyValuePair<ItemDrop, Label>> labels)
    {
        IOrderedEnumerable<KeyValuePair<ItemDrop, Label>> sortedLabels = labels.OrderBy(x => x.Key.transform.position.y);
        placedRects.Clear();
        foreach (KeyValuePair<ItemDrop, Label> entry in sortedLabels)
        {
            ItemDrop loot = entry.Key;
            Label label = entry.Value;

            Vector3 position = loot.transform.position + loot.TitleOffset;
            Vector2 viewportPoint = LootManager.instance.mainCamera.WorldToViewportPoint(position);
            Vector2 worldPoint = LootManager.instance.labelCamera.ViewportToWorldPoint(viewportPoint);
            label.Show(worldPoint, loot.Title);
            Rect rect = label.GetScreenRect();
            Rect placedRect = PutRect(rect);
            Vector2 offset = placedRect.position - rect.position;
            label.Show(worldPoint + offset, loot.Title);
        }
    }

    Rect PutRect(Rect rect)
    {
        foreach (Rect placedRect in placedRects.Values)
        {
            if (rect.Overlaps(placedRect))
            {
                rect.y = placedRect.yMax + verticalPadding;
            }
        }
        placedRects.Add(rect.y, rect);
        return rect;
    }
}
