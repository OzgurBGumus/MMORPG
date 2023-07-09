using UnityEngine;

namespace GFFAddons
{
    public class NpcEnchantment : NpcOffer
    {

        public override bool HasOffer(Player player) => OpenCheck(player);

        public override string GetOfferName() => "Item Enchantment";

        public override void OnSelect(Player player)
        {
            UIUpgrade.singleton.Show();
            UIInventory.singleton.panel.SetActive(true); // better feedback

            UIMerchant.singleton.panel.SetActive(false);
            UINpcDialogue.singleton.panel.SetActive(false);
        }

        private bool OpenCheck(Player player)
        {
            return (player.upgrade.data.operatingMode == UpgradeAddonMode.npc || player.upgrade.data.operatingMode == UpgradeAddonMode.npcAndRemote);
        }
    }
}


