using UnityEngine;

[CreateAssetMenu(menuName = "GFF Addons/Item Enchantment/Gem Item", order = 999)]
public class EnchantmentGemItem : ScriptableItem
{
    [Header("GFF Upgrade addon")]
    [SerializeField]private int gemType;
    [SerializeField]private float chanceOfEnchantment = 0.5f;

    public int GetGemType
    {
        get
        {
            return gemType;
        }
    }

    public float GetChanceOfEnchantment
    {
        get
        {
            return chanceOfEnchantment;
        }
    }
}
