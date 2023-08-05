using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootManager : MonoBehaviour
{
    public static LootManager instance;

    public Label labelPrefab;
    [HideInInspector] public Camera mainCamera;
    [HideInInspector] public Camera labelCamera;

    const int maxColliderCacheSize = 60;
    readonly Collider[] hitColliders = new Collider[maxColliderCacheSize];

    bool highlightLoot;
    LootHighlighter _lootHighlighter;
    readonly HashSet<ItemDrop> loots = new HashSet<ItemDrop>();
    readonly HashSet<ItemDrop> _items = new HashSet<ItemDrop>();
    LabelPool lootLabelPool;
    Label label;

    IEnumerator coroutine;

    Player player;

    public Player GetPlayer() => player;
    public LootHighlighter GetLootHighlighter() => _lootHighlighter;

    public IReadOnlyCollection<ItemDrop> Items => _items;

    public bool HighlightLoot
    {
        get => highlightLoot;
        set
        {
            if (value != highlightLoot)
            {
                highlightLoot = value;

                HideLabel(); // hide a single label

                SetPlayer();

                if (highlightLoot)
                {
                    coroutine = ShowLabels();
                    StartCoroutine(coroutine);
                }
                else
                {
                    loots.Clear();
                    _lootHighlighter.Show(loots);
                }
            }
        }
    }

    void Awake()
    {
        instance = this;
        lootLabelPool = new LabelPool(transform);
        _lootHighlighter = new LootHighlighter(lootLabelPool);

        label = Create();

        mainCamera = Camera.main;

        Camera clone = Instantiate(ItemDropSettings.Settings.labelCamera);

        // renders only the item label layer
        clone.cullingMask = ItemDropSettings.Settings.GetLabelMask();
        labelCamera = clone;
    }

    public Label Create()
    {
        return Instantiate(labelPrefab, transform);
    }

    void SetPlayer()
    {
        if (!player)
        {
            player = Player.localPlayer;
        }
    }

    public void ItemPickup(ItemDrop item)
    {
        SetPlayer();

        if (player != null)
        {
            if (player.state == "IDLE" || player.state == "MOVING")
            {
                //player.ControlRaycast(true);
                player.SetTargetItem(item);
            }
        }
    }

    void FindVisibleLoots()
    {
        if (player != null)
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
            
            int numColliders = Physics.OverlapSphereNonAlloc(player.transform.position, ItemDropSettings.Settings.labelVisibilityRange, hitColliders, ItemDropSettings.Settings.GetPointMask());
            for (int i = 0; i < numColliders; i++)
            {
                if (hitColliders[i].transform.parent.TryGetComponent(out ItemDrop loot))
                {
                    if (GeometryUtility.TestPlanesAABB(planes, loot.ItemBounds))
                    {
                        loots.Add(loot);
                    }
                }
            }           
        }
    }

    public void Add(ItemDrop item)
    {
        _items.Add(item);

        if (highlightLoot)
        {
            if (highlightLoot)
            {
#if ITEM_DROP_C
                if (!player.IsMoving())
                {
                    loots.Clear();
                    FindVisibleLoots();
                }
#endif
#if ITEM_DROP_R
                if (!player.movement.IsMoving())
                {
                    loots.Clear();
                    FindVisibleLoots();
                }
#endif
            }
        }
    }

    public void Remove(ItemDrop item)
    {
        _items.Remove(item);

        if (highlightLoot)
        {
#if ITEM_DROP_C
            if (!player.IsMoving())
            {
                loots.Clear();
                FindVisibleLoots();
            }
#endif
#if ITEM_DROP_R
            if (!player.movement.IsMoving())
            {
                loots.Clear();
                FindVisibleLoots();
            }
#endif
        }
        HideLabel();
    }

    public void ShowLabel(ItemDrop item)
    {
        Vector3 labelPosition = item.transform.position + item.TitleOffset;
        Vector2 viewportPoint = mainCamera.WorldToViewportPoint(labelPosition);
        Vector2 worldPoint = labelCamera.ViewportToWorldPoint(viewportPoint);

        label.Show(worldPoint, item.Title);
    }
      
    public void HideLabel()
    {
        if (label != null)
        {
            label.Hide();
        }
    }

    IEnumerator ShowLabels()
    {
#if ITEM_DROP_C
        if (!player.IsMoving())
        {
            loots.Clear();
            FindVisibleLoots();
        }
#endif
#if ITEM_DROP_R
        if (!player.movement.IsMoving())
        {
            loots.Clear();
            FindVisibleLoots();
        }
#endif
        while (highlightLoot)
        {
#if ITEM_DROP_C
            if (player.IsMoving())
            {
                loots.Clear();
                FindVisibleLoots();
            }
#endif
#if ITEM_DROP_R
            if (player.movement.IsMoving())
            {
                loots.Clear();
                FindVisibleLoots();
            }
#endif
            _lootHighlighter.Show(loots);
            yield return null;
        }
    }
}
