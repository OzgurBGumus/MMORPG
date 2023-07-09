using System;
using UnityEngine;

namespace GFFAddons
{
    public enum UpgradeAddonMode { npc, remote, npcAndRemote, shortcuts }

    public enum UpgradeMaterialType
    {
        magicPowder,
        oldMagicPowder,
        DragonCrystal
    }

    [CreateAssetMenu(menuName = "GFF Addons/Item Enchantment/Enchantment Settings", order = 999)]
    public class EnchantmentData : ScriptableObject
    {
        public UpgradeAddonMode operatingMode;
        [SerializeField] private bool _needHolesForEnchantment = true;

        [Header("Settings : Item Durability Addon")]
        public bool useDurabilityAddon = false;
        public int minDurabilityValue = 10;
        public int durabilityDecreaseValue = 1;
        public float durabilityDecreasePercent = 0;
        public bool durabilityDecreasesWhenRunesDestroyed;
        public bool durabilityDecreasesWhenModificationFailed;
        public bool durabilityDecreasesWhenRunesExtract;

        [Header("Settings : Show info messages")]
        public bool showSuccessfullyEnchantmentOnChat;
        public int showSuccessfullyUpdateValue;
        public bool showFailedEnchantmentOnChat;
        public int showFailedEnchantmentAmount;

        public bool AllowedToShowPanel(Player player)
        {
            if (operatingMode == UpgradeAddonMode.remote || operatingMode == UpgradeAddonMode.shortcuts) return true;
            else if (operatingMode == UpgradeAddonMode.npc && player.target != null && player.target is Npc && Utils.ClosestDistance(player, player.target) <= player.interactionRange) return true;
            else if (operatingMode == UpgradeAddonMode.npcAndRemote) return true;
            else return false;
        }

        public bool needHolesForEnchantment => _needHolesForEnchantment;
    }
}


