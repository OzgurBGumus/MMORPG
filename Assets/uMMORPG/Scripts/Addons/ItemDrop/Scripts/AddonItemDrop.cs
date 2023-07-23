using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;
using System.Text;
using UnityEngine.AI;

public static class AddonItemDrop
{
    public static int goldHashCode = "Gold".GetStableHashCode();

    static ItemDrop cache;
    public static ItemDrop ItemPrefab => cache ?? (cache = ItemDropSettings.Settings.itemPrefab.GetComponent<ItemDrop>());

    public static string Color(string itemName, Color color) => $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{itemName}</color>";
    public static string ColorStack(string itemName, int stack, Color color) => $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{itemName} x{stack}</color>";

    public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default)
    {
        return dictionary.TryGetValue(key, out TValue value) ? value : defaultValue;
    }

    public static WaitForFixedUpdate FixedUpdate { get; } = new WaitForFixedUpdate();

    #region Database
    public static bool SaveItems()
    {
        List<ItemDrop> items = (from entry in LootManager.instance.Items where entry.uniqueId != "" && entry.isMarked select entry).ToList();

        if (items.Count > 0)
        {
            Database.singleton.ItemSaveMany(items);
            //Debug.Log($"{items.Count} items have been saved.");
            return true;
        }
        return false;
    }

    public static void LoadItems()
    {
        if (ItemPrefab != null)
        {
            Database.singleton.ItemLoad(ItemPrefab);
        }
        else Debug.LogWarning("Unable to find the Item.prefab!");
    }
    
    public static void DeleteItems()
    {
        if (ItemDropSettings.Settings.individualLoot)
        {
            foreach (ItemDrop item in LootManager.instance.Items)
            {
                if (item.uniqueId != "")
                {
                    NetworkServer.Destroy(item.gameObject);
                }
            }

            foreach (Player player in Player.onlinePlayers.Values)
            {
                player.TargetDeleteItems();
            }
        }
        else
        {
            foreach (ItemDrop item in LootManager.instance.Items)
            {
                NetworkServer.Destroy(item.gameObject);
            }
        }

        Database.singleton.ItemsDelete();
    }

    public static void DeleteItem(string uniqueId)
    {
        if (uniqueId != "")
        {
            Database.singleton.ItemDelete(uniqueId);
        }
    }
    #endregion

    #region Monster
    /// <summary>
    /// Returns an item created without a unique ID.
    /// </summary>
    public static ItemDrop GenerateLoot(string itemName, bool currency, long gold, Vector3 center, Vector3 point)
    {
        ItemDrop clone = UnityEngine.Object.Instantiate(ItemPrefab, center, Quaternion.identity);
        clone.name = itemName;
        clone.stack = currency ? Convert.ToInt32(gold) : 1;
        clone.endingPoint = point;
        return clone;
    }

    /// <summary>
    /// Returns a random point inside a sphere on the reachable NavMesh around the center position.
    /// </summary>
    public static bool RandomPoint3D(Vector3 center, out Vector3 result)
    {
        bool blocked = true;
       
        while (blocked)
        {
            Vector3 randomPoint = (UnityEngine.Random.insideUnitSphere * ItemDropSettings.Settings.itemDropRadius) + center;
            blocked = NavMesh.Raycast(center, randomPoint, out var hit, 1);

            if (!blocked)
            {
                //Debug.Log("ok");
                result = hit.position;
                return true;
            }
            //Debug.Log("find another place");
        }        
        //Debug.Log("nothing");
        result = Vector3.zero;
        return false;
    }    
    #endregion

    #region Player
    /// <summary>
    /// Returns an item created with a unique ID (can be saved).
    /// </summary>
    public static ItemDrop GenerateItem(string itemName, int amount, Player player, Vector3 point)
    {
        ItemDrop clone = UnityEngine.Object.Instantiate(ItemPrefab, player.transform.position, player.transform.rotation);
        clone.name = itemName;
        clone.uniqueId = Guid.NewGuid().ToString();
        clone.stack = amount;
        clone.endingPoint = CheckTargetPoint(player, point, clone);
        return clone;
    }

    /// <summary>
    /// Looks for a place where the player can drop an item.
    /// </summary>
    public static Vector3 CheckTargetPoint(Player player, Vector3 targetPoint, ItemDrop item)
    {
        Vector3 center = player.transform.position;
        Vector3 origin = targetPoint + Vector3.up * 4;

        bool blocked = NavMesh.Raycast(center, targetPoint, out var hit, 1);
        // check if the path from the source to target is unobstructed
        if (!blocked)
        {
            if (Physics.Raycast(origin, Vector3.down, out var itemPoint, Mathf.Infinity, ItemDropSettings.Settings.GetPointMask()))
            {
                // this position is occupied with another item
                if (itemPoint.collider != null)
                {
                    float stackHeight = itemPoint.collider.bounds.center.y - center.y;
                    //Debug.Log(stackHeight);

                    // height control of items on the stack
                    if (stackHeight >= 1.25f)
                    {
                        // too high to reach, find randomly another reachable position
                        RandomPoint3D(center, out Vector3 randomPoint);
                        return randomPoint;
                    }
                    // put on top of the stack
                    return itemPoint.collider.bounds.center;
                }
            }            
            else
            {
                // check if this position is on the NavMesh
                if (NavMesh.SamplePosition(targetPoint, out var navMeshHit, Mathf.Infinity, 1))
                {
                    return navMeshHit.position;
                }
            }            
        }
        // if this position is not reachable try to find another one randomly
        RandomPoint3D(center, out Vector3 result);
        return result;
    }
    #endregion

    /// <summary>
    /// Quadratic Bezier Curve (point1 as a control point).
    /// </summary>
    public static Vector3 GetPoint(Vector3 point0, Vector3 point1, Vector3 point2, float t)
    {
        return Vector3.Lerp(Vector3.Lerp(point0, point1, t), Vector3.Lerp(point1, point2, t), t);
    }

    public static List<T> GetRandomElements<T>(IEnumerable<T> list, int elementsCount)
    {
        return list.OrderBy(x => Guid.NewGuid()).Take(elementsCount).ToList();
    }

    public static float ClosestDistance(Entity entity, ItemDrop item)
    {
        float distance = Vector3.Distance(entity.transform.position, item.transform.position);

        float radiusA = Utils.BoundsRadius(entity.collider.bounds);
        float radiusB = Utils.BoundsRadius(item.itemCollider.bounds);

        float distanceInside = distance - radiusA - radiusB;

        return Mathf.Max(distanceInside, 0);
    }

    public static Vector3 ClosestPoint(ItemDrop item, Vector3 point)
    {
        float radius = Utils.BoundsRadius(item.itemCollider.bounds);

        Vector3 direction = item.transform.position - point;

        Vector3 directionSubtracted = Vector3.ClampMagnitude(direction, direction.magnitude - radius);

        return point + directionSubtracted;
    }

    public static int GetLayerIndex(int layerMaskValue)
    {
        return (int)Mathf.Log(layerMaskValue, 2);
    }

    public static LayerMask DisableLayers(LayerMask layerMask)
    {
        layerMask &= ~(1 << GetLayerIndex(ItemDropSettings.Settings.GetItemMask()));
        layerMask &= ~(1 << GetLayerIndex(ItemDropSettings.Settings.GetLabelMask()));
        layerMask &= ~(1 << GetLayerIndex(ItemDropSettings.Settings.GetPointMask()));
        return layerMask;
    }
}

public partial class ScriptableItem
{

    public bool gold;

    public GameObject prefab;

    // Additional fields to set in the ScriptableItem: Offset X, Offset Y, Offset Z for position and Rot X, Rot Y, Rot Z for rotation.
    //
    // They have been added to correct the standard models from uMMORPG. If you prepared your models the right way*, before import them to Unity engine, 
    // you should not need to change anything here but for the uMMORPG models it is necessary.
    //
    // Unity engine uses different axis than most standard 3d modeling software which can cause problems with scale and rotation, especially for new users.
    //
    // *Scale Factor from your model should be 1 after import, blue arrow as front of your prefab, scale (1, 1, 1), rotation (0, 0, 0).

    [Range(-5, 5)] public float offsetX;
    [Range(-5, 5)] public float offsetY;
    [Range(-5, 5)] public float offsetZ;
    [Range(-180, 180)] public float rotX;
    [Range(-180, 180)] public float rotY;
    [Range(-180, 180)] public float rotZ;

    public string GetTitle(int stack = 1)
    {
        StringBuilder stringBuilder = new StringBuilder();
        Color color = ItemDropSettings.Settings.gold;

        if (!gold)
        {
            color = Player.localPlayer.itemRarityConfig.GetColor(rarity);
        }

        if (stack == 1)
        {
            stringBuilder.Append(AddonItemDrop.Color(name, color));
        }
        else
        {
            stringBuilder.Append(AddonItemDrop.ColorStack(name, stack, color));
        }
        return stringBuilder.ToString();
    }
}
