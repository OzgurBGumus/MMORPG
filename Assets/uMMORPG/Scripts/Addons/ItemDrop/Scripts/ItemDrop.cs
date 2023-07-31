using UnityEngine;
using Mirror;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(NetworkName))]
public class ItemDrop : NetworkBehaviour
{
    [HideInInspector] public ScriptableItem data;
    [HideInInspector, SyncVar] public int stack;
    [SyncVar] public string owner;
    [HideInInspector, SyncVar] public Vector3 endingPoint;
    [HideInInspector] public string uniqueId;
    [HideInInspector] public bool isMarked;

    [Header("Components")]
    public BoxCollider itemCollider;
    public Shader highlightedColor;

    Material[] materials;
    Renderer[] renderers;

    Vector3 startingPoint;
    Vector3 controlPoint;
    float count;

    string _title = null;
    public string Title
    {
        set { _title = value; }
        get { return _title ?? name; }
    }

    public Bounds ItemBounds => itemCollider.bounds;
    public Vector3 TitleOffset => Vector3.up;
    public bool IsVisible => renderers != null ? renderers[0].isVisible : false;
    public bool IsMoving => transform.position != endingPoint;

    IEnumerator coroutineDrop;
    IEnumerator coroutineDestroy;
    IEnumerator courtineRemoveOwner;
    WaitForSeconds updateInterval;
    WaitForSeconds removeOwnerInterval;

    void Awake()
    {
        coroutineDrop = Drop();
        coroutineDestroy = DestroyAfter();
        courtineRemoveOwner = RemoveOwnerAfter();

        updateInterval = new WaitForSeconds(ItemDropSettings.Settings.decayTime);
        removeOwnerInterval = new WaitForSeconds(ItemDropSettings.Settings.ownerRemoveTime);
    }

    void Start()
    {
#if ITEM_DROP_C
        if (ScriptableItem.dict.TryGetValue(name.GetStableHashCode(), out data))
        {
            if (data.prefab != null)
            {
                startingPoint = transform.position;

                foreach (MeshFilter mesh in data.prefab.GetComponentsInChildren<MeshFilter>())
                {
                    GameObject child = Instantiate(ItemDropSettings.Settings.itemPart);
                    child.transform.SetParent(transform, false);
                    //child.name = name.Replace("(Clone)", "").Trim();

                    child.transform.localScale = mesh.transform.localScale;
                    child.transform.localPosition = new Vector3(data.offsetX, data.offsetY, data.offsetZ);
                    child.transform.localRotation = Quaternion.Euler(data.rotX, data.rotY, data.rotZ);

                    child.AddComponent<MeshFilter>().mesh = mesh.sharedMesh;
                    child.AddComponent<MeshRenderer>().materials = mesh.GetComponent<MeshRenderer>().sharedMaterials;
                }

                foreach (SkinnedMeshRenderer mesh in data.prefab.GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    GameObject child = Instantiate(ItemDropSettings.Settings.itemPart);
                    child.transform.SetParent(transform, false);
                    //child.name = name.Replace("(Clone)", "").Trim();

                    child.transform.localScale = mesh.transform.localScale;
                    child.transform.localPosition = new Vector3(data.offsetX, data.offsetY, data.offsetZ);
                    child.transform.localRotation = Quaternion.Euler(data.rotX, data.rotY, data.rotZ);

                    child.AddComponent<MeshFilter>().mesh = mesh.sharedMesh;
                    child.AddComponent<MeshRenderer>().materials = mesh.sharedMaterials;
                }

                CalculateBoxCollider();

                // destroy after a certain amount of time
                if (isServer)
                {
                    if (uniqueId == "")
                    {
                        StartCoroutine(coroutineDestroy);
                    }

                    foreach (Renderer rend in renderers)
                    {
                        rend.enabled = false;
                    }
                }

                LootManager.instance.Add(this);

                Title = data.GetTitle(stack);

                StartCoroutine(coroutineDrop);
            }
        }
#endif
#if ITEM_DROP_R
        if (ScriptableItem.All.TryGetValue(name.GetStableHashCode(), out data))
        {
            if (data.prefab != null)
            {
                startingPoint = transform.position;

                foreach (MeshFilter mesh in data.prefab.GetComponentsInChildren<MeshFilter>())
                {
                    GameObject child = Instantiate(ItemDropSettings.Settings.itemPart);
                    child.transform.SetParent(transform, false);
                    //child.name = name.Replace("(Clone)", "").Trim();

                    child.transform.localScale = mesh.transform.localScale;
                    child.transform.localPosition = new Vector3(data.offsetX, data.offsetY, data.offsetZ);
                    child.transform.localRotation = Quaternion.Euler(data.rotX, data.rotY, data.rotZ);

                    child.AddComponent<MeshFilter>().mesh = mesh.sharedMesh;
                    child.AddComponent<MeshRenderer>().materials = mesh.GetComponent<MeshRenderer>().sharedMaterials;
                }

                foreach (SkinnedMeshRenderer mesh in data.prefab.GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    GameObject child = Instantiate(ItemDropSettings.Settings.itemPart);
                    child.transform.SetParent(transform, false);
                    //child.name = name.Replace("(Clone)", "").Trim();

                    child.transform.localScale = mesh.transform.localScale;
                    child.transform.localPosition = new Vector3(data.offsetX, data.offsetY, data.offsetZ);
                    child.transform.localRotation = Quaternion.Euler(data.rotX, data.rotY, data.rotZ);

                    child.AddComponent<MeshFilter>().mesh = mesh.sharedMesh;
                    child.AddComponent<MeshRenderer>().materials = mesh.sharedMaterials;
                }

                CalculateBoxCollider();

                // destroy after a certain amount of time
                if (isServer)
                {
                    if (uniqueId == "")
                    {
                        StartCoroutine(coroutineDestroy);
                        StartCoroutine(courtineRemoveOwner);
                    }

                    foreach (Renderer rend in renderers)
                    {
                        rend.enabled = false;
                    }
                }

                LootManager.instance.Add(this);

                Title = data.GetTitle(stack, owner);

                StartCoroutine(coroutineDrop);
            }
        }
#endif
    }

    IEnumerator Drop()
    {
        while (IsMoving)
        {
            float step = ItemDropSettings.Settings.itemDropSpeed * Time.deltaTime;

            if (!isMarked)
            {
                // define a point that controls where the curve is made
                controlPoint = startingPoint + (endingPoint - startingPoint) / 2 + Vector3.up * 2.0f;
                isMarked = true;
            }

            if (count < 1.0f)
            {
                count += step;

                // move item along a curve to make a nice looking drop effect
                transform.position = AddonItemDrop.GetPoint(startingPoint, controlPoint, endingPoint, count);
                yield return null;
            }
        }
    }

    IEnumerator DestroyAfter()
    {
        yield return updateInterval;
        NetworkServer.Destroy(gameObject);
    }
    IEnumerator RemoveOwnerAfter()
    {
        yield return removeOwnerInterval;
        owner = "";
        Title = data.GetTitle(stack, owner);
    }

    [Client]
    void OnMouseEnter()
    {
        if (Utils.IsCursorOverUserInterface())
            return;

        if (!LootManager.instance.HighlightLoot)
        {
            LootManager.instance.ShowLabel(this);

            if (!highlightedColor)
                return;

            foreach (Renderer child in renderers)
            {
                foreach (Material material in child.materials)
                {
                    //material.color = Color.green;
                    material.shader = highlightedColor;
                }
            }
        }
    }

    [Client]
    void OnMouseExit()
    {
        LootManager.instance.HideLabel();

        if (!highlightedColor)
            return;

        foreach (Renderer child in renderers)
        {
            child.materials = materials;
        }        
    }
#if ITEM_DROP_R    
    [Client]
    void OnMouseDown()
    {
        LootManager.instance.ItemPickup(this);
    }    
#endif
    void OnDestroy()
    {
        StackControl();
        LootManager.instance.Remove(this);
    }

    void StackControl()
    {
        Transform currentItem = transform.GetChild(0);
        do
        {
            // cast a ray straight upwards
            if (Physics.Raycast(currentItem.position, Vector3.up, out var hit, Mathf.Infinity, ItemDropSettings.Settings.GetPointMask()))
            {
                if (hit.collider != null)
                {
                    //Debug.Log(hit.transform.parent.name);

                    // cast a ray straight downwards
                    if (Physics.Raycast(hit.transform.position, Vector3.down, out var hitDown, Mathf.Infinity, ItemDropSettings.Settings.GetPointMask()))
                    {
                        // use item control point
                        hit.transform.parent.position = hitDown.point;
                    }
                    else
                    {
                        // there is no control point underneath
                        hit.transform.parent.position = currentItem.parent.position;
                    }
                    currentItem = hit.transform;
                    isMarked = true;
                }
            }
            else currentItem = null;
        }
        while (currentItem != null);
    }

    void CalculateBoxCollider()
    {
        Quaternion currentRotation = transform.rotation;
        transform.rotation = Quaternion.identity;

        Bounds bounds = new Bounds(transform.position, Vector3.zero);
        renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
            materials = renderer.sharedMaterials;            
        }

        Vector3 localCenter = bounds.center - transform.position;
        bounds.center = localCenter;

        transform.rotation = currentRotation;

        // box collider
        itemCollider.center = bounds.center;
        itemCollider.size = bounds.size;

        // control point
        Vector3 controlPoint = transform.GetChild(0).position;
        controlPoint.y = ItemBounds.max.y;
        transform.GetChild(0).position = controlPoint;
    }
}

public struct Loot
{
    public int hashCode;
    public string uniqueId;
    public int stack;

    public Loot(int _hashCode, string _uniqueId, int _stack)
    {
        hashCode = _hashCode;
        uniqueId = _uniqueId;
        stack = _stack;
    }
}
