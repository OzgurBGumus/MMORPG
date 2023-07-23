using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[HelpURL("https://docs.google.com/document/d/16n3jnV6_2UKyDijFJN3vOCcTZUlKTfkij6AtMQFYXYg/edit")]
public class ItemDropSettings : ScriptableObject
{
    static ItemDropSettings cache;
    public static ItemDropSettings Settings => cache ?? (cache = Resources.Load<ItemDropSettings>("ItemDropSettings"));

#if UNITY_EDITOR
    [HideInInspector]
    public bool isInstalled;
#endif

    [Header("Item Color")]
    public Color uniqueItem = new Color32(218, 165, 32, 255);       // #DAA520 GoldenRod
    public Color rareItem = new Color32(30, 144, 255, 255);         // #1E90FF DodgerBlue
    public Color normalItem = Color.white;                          // #FFFFFF White
    public Color gold = new Color32(105, 105, 105, 255);            // #808080 Gray

    [Header("Label Color")]
    public Color label = new Color32(0, 0, 0, 185);                 // #000000 Black
    public Color highlightedLabel = new Color32(25, 25, 112, 255);  // #191970 MidnightBlue
   
    [Header("Tag & Layer Masks")]
    [TagSelector] public string itemTag = "Untagged";
    public LayerMask itemMask;
    public LayerMask labelMask;
    public LayerMask pointMask;

    [Header("Key Bindings")]
    [Tooltip("Holding down the selected key highlights any items on the ground that you can pick up.")]
    public KeyCode itemHighlightKey = KeyCode.LeftAlt;
    public bool highlightAlways;

    [Tooltip("When pressed picks up the nearest item.")]
    public KeyCode itemPickupKey = KeyCode.F;

    [Tooltip("Holding down the selected key with clicking on an item in the inventory window will put a link to it in your chat window.")]
    public KeyCode itemLinkKey = KeyCode.LeftControl;

    [Header("Timer")]
    [Min(600f)] public float decayTime = 3600f;     // 1 hour
    [Min(600f)] public float saveInterval = 1800f;  // 30 minutes

    [Header("Prefabs")]
    public Camera labelCamera;
    public GameObject lootManager;
    public GameObject itemPrefab;
    public GameObject itemPart;
    public GameObject itemLabel;

    [TextArea(1, 10)] public string folderPrefabs;

    [Header("Others")]
    [Min(2f)] public float itemDropSpeed = 2f;
    [Min(1f)] public float itemDropRadius = 3f;
    [Min(5f)] public float labelVisibilityRange = 15f;
    [Min(1f)] public float itemPickupRadius = 4f;

    [Tooltip("The number of pixels per unit, as in your sprites.")]
    [Min(1f)] public float pixelsPerUnit = 34;
    public bool fitLabelsIntoScreen = true;
    public bool showMessages = true;

    [Header("Extras")]
    [Tooltip("All items dropped by monsters are assigned to the individual player.")]
    public bool individualLoot;

    [Tooltip("Characters drop a percentage of their gold when they die.")]
    public GoldLoss goldLoss;
    [Serializable]
    public struct GoldLoss
    {
        public bool isActive;
        [Range(1, 100)] public int percentageLoss;
    }

    [Tooltip("Characters drop a random item of their inventory when they die.")]
    public ItemLoss itemLoss;

    [Serializable]
    public struct ItemLoss
    {
        public bool isActive;
        [Range(1, 5)] public int amount;
    }

    public int GetItemMask() => itemMask.value;
    public int GetLabelMask() => labelMask.value;
    public int GetPointMask() => pointMask.value;

#if UNITY_EDITOR
    // true/false for the first installation, 
    // because the effects of changes in the ScriptingDefineSymbols will not be included in the codeblock right after the change
    public void DefaultSettings(bool classic = true)
    {
        uniqueItem = new Color32(218, 165, 32, 255);        // #DAA520 GoldenRod
        rareItem = new Color32(30, 144, 255, 255);          // #1E90FF DodgerBlue
        normalItem = Color.white;                           // #FFFFFF White
        gold = new Color32(105, 105, 105, 255);             // #808080 Gray

        label = new Color32(0, 0, 0, 185);                  // #000000 Black
        highlightedLabel = new Color32(25, 25, 112, 255);   // #191970 MidnightBlue

        SetTagsAndLayers();

        itemHighlightKey = KeyCode.LeftAlt;
        itemPickupKey = KeyCode.F;
        itemLinkKey = KeyCode.LeftControl;

        decayTime = 3600f;      // 1 hour
        saveInterval = 1800f;   // 30 minutes

        itemDropSpeed = 2f;
        itemDropRadius = 3f;
        labelVisibilityRange = 15f;
        pixelsPerUnit = 34f;

        fitLabelsIntoScreen = true;
        showMessages = true;

        individualLoot = false;

        goldLoss.isActive = false;
        goldLoss.percentageLoss = 50;

        itemLoss.isActive = false;
        itemLoss.amount = 1;

        folderPrefabs = "Assets/uMMORPG/Scripts/Addons/ItemDrop/Prefabs/Items";

#if ITEM_DROP_R
        classic = false;
#endif
        DefaultItems(classic);
    }

    string FindTag(string tag)
    {
        string[] tags = UnityEditorInternal.InternalEditorUtility.tags;
        for (int i = 0; i < tags.Length; i++)
        {
            if (tags[i].Equals(tag))
            {
                return tag;
            }
        }
        return "Untagged";
    }

    LayerMask FindLayer(string layer)
    {
        string[] layers = UnityEditorInternal.InternalEditorUtility.layers;
        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i].Equals(layer))
            {
                return LayerMask.GetMask(layer);
            }
        }
        return LayerMask.GetMask("Default");
    }

    public void SetTagsAndLayers()
    {
        itemTag = FindTag("Item");
        itemMask = FindLayer("Item");
        labelMask = FindLayer("ItemLabel");
        pointMask = FindLayer("ItemPoint");
    }

    #region Default Models
    void LoadModel(ScriptableItem item, string prefabName)
    {
        if (!item.prefab)
        {
            string path = System.IO.Path.Combine(folderPrefabs, prefabName);
            item.prefab = (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
        }
    }

    void DefaultItems(bool classic = true)
    {
        if (!AssetDatabase.IsValidFolder(folderPrefabs))
        {
            Debug.Log($"The given path does not exists ({folderPrefabs}).");
            return;
        }

        ScriptableItem[] items = Resources.LoadAll<ScriptableItem>("");
        for (int i = 0; i < items.Length; ++i)
        {
            ScriptableItem itemData = items[i];

            if (itemData is EquipmentItem)
            {
                EquipmentItem equipment = itemData as EquipmentItem;
                if (equipment.modelPrefab)
                    itemData.prefab = equipment.modelPrefab;
            }

            // prepare default items from uMMORPG
            if (classic)
            {
                // classic
                if (itemData.name == "Arrow")
                {
                    if (!itemData.prefab)
                    {
                        TargetProjectileSkill skill = Resources.Load<TargetProjectileSkill>("Skills, Buffs, Status Effects/Archer/Normal Attack (Archer)");
                        if (skill != null)
                        {
                            itemData.prefab = skill.projectile.gameObject;
                        }
                    }
                    itemData.offsetX = -0.3f;
                    itemData.rotY = 90;
                }
                else if (itemData.name == "Dark Shield")
                {
                    itemData.rotX = 90;
                }
                else if (itemData.name == "Dark Sword")
                {
                    itemData.offsetX = -0.3f;
                    itemData.rotX = 90;
                    itemData.rotZ = -90;
                }
                else if (itemData.name == "Desert Cleaver")
                {
                    itemData.offsetX = -0.3f;
                    itemData.rotY = 90;
                }
                else if (itemData.name == "Health Potion")
                {
                    LoadModel(itemData, "HealthPotion.prefab");
                }
                else if (itemData.name == "Mana Potion")
                {
                    LoadModel(itemData, "ManaPotion.prefab");
                }
                else if (itemData.name == "Pet Health Potion")
                {
                    LoadModel(itemData, "PetHealthPotion.prefab");
                }
                else if (itemData.name == "Vigor Potion")
                {
                    LoadModel(itemData, "VigorPotion.prefab");
                }
                else if (itemData.name == "Monster Scroll (Skeleton Giant)")
                {
                    LoadModel(itemData, "Scroll.prefab");
                }
                else if (itemData.name == "Sun Shield")
                {
                    itemData.rotX = -90;
                }
                else if (itemData.name == "Sun Sword")
                {
                    itemData.offsetX = -0.5f;
                    itemData.rotX = 90;
                    itemData.rotZ = 180;
                }
                else if (itemData.name == "Warrior Boots")
                {
                    itemData.rotX = -90;
                }
                else if (itemData.name == "Warrior Chest")
                {
                    itemData.offsetZ = -1.6f;
                    itemData.rotZ = -180;
                }
                else if (itemData.name == "Warrior Gloves")
                {
                    itemData.offsetZ = -1.25f;
                    itemData.rotZ = -180;
                }
                else if (itemData.name == "Warrior Pants")
                {
                    itemData.offsetZ = -1.0f;
                    itemData.rotZ = -180;
                }
                else if (itemData.name == "Warrior Shoulders")
                {
                    itemData.offsetZ = -1.8f;
                    itemData.rotZ = -180;
                }
                else if (itemData.name == "White Bow")
                {
                    itemData.rotX = 180;
                    itemData.rotZ = 90;
                }
            }
            else
            {
                // remastered
                if (itemData.name == "Health Potion")
                {
                    LoadModel(itemData, "HealthPotion.prefab");
                }
                else if (itemData.name == "Mana Potion")
                {
                    LoadModel(itemData, "ManaPotion.prefab");
                }
                else if (itemData.name == "Pet Health Potion")
                {
                    LoadModel(itemData, "PetHealthPotion.prefab");
                }
                else if (itemData.name == "Vigor Potion")
                {
                    LoadModel(itemData, "VigorPotion.prefab");
                }
                else if (itemData.name == "Monster Scroll (Forest Giant)")
                {
                    LoadModel(itemData, "Scroll.prefab");
                }
                else if (itemData.name == "Archer Boots")
                {
                    itemData.rotX = -90;
                }
                else if (itemData.name == "Archer Leg Armor")
                {
                    itemData.offsetZ = -1.0f;
                    itemData.rotZ = -180;
                }
                else if (itemData.name == "Archer Shoulder Armor")
                {
                    itemData.offsetZ = -1.5f;
                    itemData.rotZ = -180;
                }
                else if (itemData.name == "Archer Top Armor")
                {
                    itemData.offsetZ = -1.5f;
                    itemData.rotZ = -180;
                }
                else if (itemData.name == "Arrow")
                {
                    if (!itemData.prefab)
                    {
                        TargetProjectileSkill skill = Resources.Load<TargetProjectileSkill>("Skills, Buffs, Status Effects/Archer/Normal Attack (Archer)");
                        if (skill != null)
                        {
                            itemData.prefab = skill.projectile.gameObject;
                        }
                    }
                    itemData.offsetX = -0.3f;
                    itemData.rotY = 90;
                }
                else if (itemData.name == "Blacksmiths Shield")
                {
                    itemData.rotX = -90;
                }
                else if (itemData.name == "Blacksmiths Sword")
                {
                    itemData.rotZ = -90;
                }
                else if (itemData.name == "Warrior Belt")
                {
                    itemData.offsetZ = -1.0f;
                    itemData.rotX = -90;
                    itemData.rotZ = 180;
                }
                //else if (itemData.name == "Warrior Boots")
                //{
                //}
                else if (itemData.name == "Warrior Gloves")
                {
                    itemData.offsetX = -0.65f;
                    itemData.offsetY = -1.4f;
                }
                else if (itemData.name == "Warrior Helmet")
                {
                    itemData.offsetY = -1.6f;
                }
                else if (itemData.name == "WarShoulders")
                {
                    itemData.offsetZ = -1.5f;
                    itemData.rotX = -90;
                    itemData.rotY = 180;
                }
                else if (itemData.name == "White Bow")
                {
                    itemData.rotX = 180;
                    itemData.rotZ = 90;
                }
            }
            EditorUtility.SetDirty(itemData);
        }
    }
    #endregion

#endif
}
