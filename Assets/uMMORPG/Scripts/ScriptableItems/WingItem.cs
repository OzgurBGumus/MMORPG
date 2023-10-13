using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "uMMORPG Item/Wing", order = 9991)]
public class WingItem : EquipmentItem
{
    [Header("Wing")]
    public ScriptableItem nextWing;
    public ScriptableItem wingUpgradeMaterial;
    public int wingUpgradeMaterialAmount;
    public int wingUpgradeChance;
}
