using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum CustomizationType { byMaterials, byObjects }
public enum EquipmentItemType { Hat, SunGlasses, Masks, Skin, Hair, Brow, Eye, Jackets, Pants, Boots, Gloves, ProtectiveSuit, Backpack, Bodyarmor }

[Serializable]public class CustomizationObject
{
    public string name;
    public GameObject[] parts;
    public EquipmentItem item;
}
[Serializable]public class CustomizationMaterials
{
    public SkinnedMeshRenderer mesh;
    public int positionInMesh;
    public Material[] meshes;
}

[Serializable]public class Customization
{
    [HideInInspector]public string name;
    public EquipmentItemType type;
    public CustomizationType customizationBy;
    public CustomizationObject[] objects;
    public CustomizationMaterials material;
    public bool showWhenCharactercCreate = false;
}

[DisallowMultipleComponent]
public class PlayerCustomization : NetworkBehaviour
{
    public Customization[] customization;
    public bool rescaling;
    public float scaleMin = 0.5f;
    public float scaleMax = 1.5f;

    public SyncList<int> values = new SyncList<int>();
    [SyncVar]public float scale = 1;

    void Start()
    {
        // do nothing if not spawned (=for character selection previews)
        if (!isServer && !isClient) return;

        SetCustomization();
    }

    public int FindIndex(EquipmentItemType type)
    {
        for (int i = 0; i < customization.Length; i++)
        {
            if (customization[i].type == type) return i;
        }
        return -1;
    }

    public Customization[] GetItemTypesForCharacterCreate()
    {
        List<Customization> list = new List<Customization>();

        for (int i = 0; i < customization.Length; i++)
        {
            if (customization[i].showWhenCharactercCreate) list.Add(customization[i]);
        }

        return list.ToArray();
    }

    public void SetCustomizationLocalByType(EquipmentItemType type, int value)
    {
        int index = FindIndex(type);

        if (index != -1)
        {
            if (customization[index].customizationBy == CustomizationType.byObjects)
            {
                for (int i = 0; i < customization[index].objects.Length; i++)
                {
                    bool active = value == i;
                    for (int x = 0; x < customization[index].objects[i].parts.Length; x++)
                        customization[index].objects[i].parts[x].SetActive(active);
                }
            }
            else
            {
                Material[] mats = customization[index].material.mesh.materials;
                mats[customization[index].material.positionInMesh] = customization[index].material.meshes[value];
                customization[index].material.mesh.materials = mats;
            }

            values[index] = value;
        }
    }

    public void SetCustomization()
    {
        if (values!= null && values.Count > 0)
        {
            for (int i = 0; i < customization.Length; i++)
            {
                if (customization[i].customizationBy == CustomizationType.byObjects)
                {
                    bool active = values[i] == i;
                    for (int x = 0; x < customization[i].objects.Length; x++)
                    {
                        for (int y = 0; y < customization[i].objects[x].parts.Length; y++)
                        {
                            customization[i].objects[x].parts[y].SetActive(active);
                        }
                    }
                }
                else
                {
                    Material[] mats = customization[i].material.mesh.materials;
                    mats[customization[i].material.positionInMesh] = customization[i].material.meshes[values[i]];
                    customization[i].material.mesh.materials = mats;
                }
            }

            if (rescaling) gameObject.transform.localScale = new Vector3(scale, scale, scale);
        }
    }

    private void OnValidate()
    {
        for (int i = 0; i < customization.Length; i++)
        {
            customization[i].name = customization[i].type.ToString();
        }
    }
}
