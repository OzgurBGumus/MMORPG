using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GFFAddons
{
    [DisallowMultipleComponent]
    public class PlayerUpgrade : NetworkBehaviour
    {
        [Header("Components")]
        public Player player;
        public EnchantmentData data;
        [HideInInspector] public List<int> upgradeIndices = new List<int>() { -1, -1, -1, -1, -1, -1 };
        [HideInInspector] public float UpgradeTimeRemaining() => NetworkTime.time >= upgradeTimeEnd ? 0 : (float)(upgradeTimeEnd - NetworkTime.time);
        [HideInInspector, SyncVar] public double upgradeTimeEnd; // server time. double for long term precision.
        [HideInInspector] public int rarityIndex = 0;

        void OnDragAndDrop_InventorySlot_UpgradeSlot(int[] slotIndices)
        {
            ItemSlot slot = player.inventory.slots[slotIndices[0]];

            //if this armor or weapon
            if (slot.amount > 0 && slotIndices[1] == 0 && slot.item.data is EquipmentItem item && slot.item.currentUpgradeLevel < slot.item.data.upgradeBonusesPercent.Length-1)
            {
                // swap them
                upgradeIndices[slotIndices[1]] = slotIndices[0];
            }
            //if this is rune
            else if (slot.amount > 0 && slot.item.data is UpgradeMaterialItem)
            {
                // swap them
                upgradeIndices[slotIndices[1]] = slotIndices[0];
            }
        }
        void OnDragAndDrop_InventoryExtendedSlot_UpgradeSlot(int[] slotIndices)
        {
            ItemSlot slot = player.inventory.slots[slotIndices[0]];

            //if this armor or weapon
            if (slot.amount > 0 && slotIndices[1] == 0 && slot.item.data is EquipmentItem item && slot.item.currentUpgradeLevel < slot.item.data.upgradeBonusesPercent.Length - 1)
            {
                // swap them
                upgradeIndices[slotIndices[1]] = slotIndices[0];
            }
            //if this is rune
            else if (slot.amount > 0 && slot.item.data is UpgradeMaterialItem)
            {
                // swap them
                upgradeIndices[slotIndices[1]] = slotIndices[0];
            }
        }


        public void ClearUpgradeIndices()
        {
            for (int i = 0; i < upgradeIndices.Count; i++)
            {
                upgradeIndices[i] = -1;
            }

        }
        public void ClearUpgradeIndex(int index)
        {
            upgradeIndices[index] = -1;
        }

        [Command]
        public void CmdStartEnchantment(List<int> playerUpgradeIndices)
        {
            upgradeIndices = playerUpgradeIndices;
            //are slots with enchanted item and rune not empty?
            if (playerUpgradeIndices[0] != -1 && player.inventory.slots[playerUpgradeIndices[0]].amount > 0 && player.inventory.slots[playerUpgradeIndices[0]].item.data is EquipmentItem it)
            {
                List<int> ValidUpgradeBoxes = new List<int>();
                for(int i = 1;i < playerUpgradeIndices.Count; i++)
                {
                    if(playerUpgradeIndices[i] != -1 && player.inventory.slots[upgradeIndices[i]].amount > 0 && player.inventory.slots[upgradeIndices[i]].item.data.category == ItemCategory.upgradeMaterial)
                    {
                        ValidUpgradeBoxes.Add(i);
                        break;
                    }
                }
                if (ValidUpgradeBoxes.Count > 0)
                {
                    Item slot_0 = player.inventory.slots[upgradeIndices[0]].item;
                    UpgradeMaterialItem[] materials = new UpgradeMaterialItem[ValidUpgradeBoxes.Count];
                    for(int i = 0;i< ValidUpgradeBoxes.Count; i++)
                    {
                        materials[i] = (UpgradeMaterialItem)player.inventory.slots[upgradeIndices[ValidUpgradeBoxes[i]]].item.data;
                    }

                    //Can upgrade ?is it in max level?
                    if (slot_0.currentUpgradeLevel <slot_0.data.upgradeBonusesPercent.Length-1)
                    {
                        //check the combination
                        if (((EquipmentItem)slot_0.data).CanUpgradeByMaterial(materials))
                        {
                            Upgrade(materials);
                        }
                        else TargetItemUpgradeError("Materials are not correct.");
                    }
                    else TargetItemUpgradeError("Maximum Enchantment");
                }
                else TargetItemUpgradeError("Upgrade Materials Could not found.");
            }
            else TargetItemUpgradeError("The Item is not Valid.");

            ClearUpgradeIndices();
        }
        public float CalculateUpgradeBonuses()
        {
            float materialBonuses = 0;
            for (int i = 0; i < upgradeIndices.Count; i++)
            {
                if (upgradeIndices[i] != -1 && player.inventory.slots[upgradeIndices[i]].item.data is UpgradeMaterialItem material)
                    materialBonuses += material.chanceOfUpgrade;
            }
            return materialBonuses;
        }
        [Server]
        private void Upgrade(UpgradeMaterialItem[] materialList)
        {
            ItemSlot slot_0 = player.inventory.slots[upgradeIndices[0]];

            //gemstone coefficient of influence
            float materialBonuses = CalculateUpgradeBonuses();

            // addon system hooks (Item rarity)
            rarityIndex = 0;
            Utils.InvokeMany(typeof(UIUpgrade), this, "rarityValue_", this, player.inventory.slots[upgradeIndices[0]].item);

            //eng - chance of successful enchantment
            //ru - шанс зачарования (берем из айтема в зависимости от апгрэйда)
            int position = slot_0.item.currentUpgradeLevel;
            float upgradeChance = slot_0.item.data.upgradeChances[position] * materialBonuses;

            //if final chance of enchantment more than random value
            //if you set an Item's UpgradeChange to 10000, that means if you put regular items to all slots (5 slots), it will success with 10000 value.
            //that means 1 slot adds 0.2 materialBonuses
            if (upgradeChance > UnityEngine.Random.Range(0, 10000))
            {
                slot_0.item.currentUpgradeLevel++;

                player.inventory.slots[upgradeIndices[0]] = slot_0;

                if (data.showSuccessfullyEnchantmentOnChat && slot_0.item.currentUpgradeLevel >= data.showSuccessfullyUpdateValue)
                {
                    string message = "Successfully Upgraded " + slot_0.item.name + " +" + slot_0.item.currentUpgradeLevel;

                    //for original chat
                    player.chat.OnSubmit(message);

                    //if used chat extended addon
                    //player.chat.OnSubmit(message, chatChannel.info);
                }

                //show info message for all online players
                if (slot_0.item.currentUpgradeLevel >= data.showSuccessfullyUpdateValue)
                {
                    foreach (Player player in Player.onlinePlayers.Values)
                    {
                        player.upgrade.RpcSuccessfullyEnchantedOpenPanel(player.name, slot_0.item.name, slot_0.item.currentUpgradeLevel);
                    }
                }
            }
            else
            {
                slot_0.amount = 0;
                TargetItemUpgradeError("Item Destroyed");

                player.inventory.slots[upgradeIndices[0]] = slot_0;

                //show informational messages
                if (data.showFailedEnchantmentOnChat)
                {
                    string message = "Failed upgrade to" + slot_0.item.name + " +" + (slot_0.item.currentUpgradeLevel + 1);

                    //for original chat
                    player.chat.OnSubmit(message);

                    //if used chat extended addon
                    //player.chat.OnSubmit(message, chatChannel.info);
                }
            }

            //decrease amount gems and rune on 1
            for (int i = 1; i < upgradeIndices.Count; i++)
            {
                if (upgradeIndices[i] != -1)
                {
                    ItemSlot slot_i = player.inventory.slots[upgradeIndices[i]];
                    slot_i.amount -= 1;
                    player.inventory.slots[upgradeIndices[i]] = slot_i;
                }
            }
        }

        [TargetRpc] // only send to one client
        public void TargetItemUpgradeError(string message)
        {
            UIUpgrade.singleton.ItemUpgradeError(message);
        }

        [ClientRpc]
        private void RpcSuccessfullyEnchantedOpenPanel(string playername, string itemname, int amount)
        {
            UIUpgrade.singleton.ItemInfoUpgradeSuccess(playername, itemname, amount);
        }





        /////WINGS
        [Server]
        private void WingUpgrade()
        {
            ItemSlot slot_0 = player.inventory.slots[upgradeIndices[0]];

            //gemstone coefficient of influence
            float materialBonuses = CalculateUpgradeBonuses();

            // addon system hooks (Item rarity)
            rarityIndex = 0;
            Utils.InvokeMany(typeof(UIUpgrade), this, "rarityValue_", this, player.inventory.slots[upgradeIndices[0]].item);

            //eng - chance of successful enchantment
            //ru - шанс зачарования (берем из айтема в зависимости от апгрэйда)
            int position = slot_0.item.currentUpgradeLevel;
            float upgradeChance = slot_0.item.data.upgradeChances[position] * materialBonuses;

            //if final chance of enchantment more than random value
            //if you set an Item's UpgradeChange to 10000, that means if you put regular items to all slots (5 slots), it will success with 10000 value.
            //that means 1 slot adds 0.2 materialBonuses
            if (upgradeChance > UnityEngine.Random.Range(0, 10000))
            {
                slot_0.item.currentUpgradeLevel++;

                player.inventory.slots[upgradeIndices[0]] = slot_0;

                if (data.showSuccessfullyEnchantmentOnChat && slot_0.item.currentUpgradeLevel >= data.showSuccessfullyUpdateValue)
                {
                    string message = "Successfully Upgraded " + slot_0.item.name + " +" + slot_0.item.currentUpgradeLevel;

                    //for original chat
                    player.chat.OnSubmit(message);

                    //if used chat extended addon
                    //player.chat.OnSubmit(message, chatChannel.info);
                }

                //show info message for all online players
                if (slot_0.item.currentUpgradeLevel >= data.showSuccessfullyUpdateValue)
                {
                    foreach (Player player in Player.onlinePlayers.Values)
                    {
                        player.upgrade.RpcSuccessfullyEnchantedOpenPanel(player.name, slot_0.item.name, slot_0.item.currentUpgradeLevel);
                    }
                }
            }
            else
            {
                slot_0.amount = 0;
                TargetItemUpgradeError("Item Destroyed");

                player.inventory.slots[upgradeIndices[0]] = slot_0;

                //show informational messages
                if (data.showFailedEnchantmentOnChat)
                {
                    string message = "Failed upgrade to" + slot_0.item.name + " +" + (slot_0.item.currentUpgradeLevel + 1);

                    //for original chat
                    player.chat.OnSubmit(message);

                    //if used chat extended addon
                    //player.chat.OnSubmit(message, chatChannel.info);
                }
            }

            //decrease amount gems and rune on 1
            for (int i = 1; i < upgradeIndices.Count; i++)
            {
                if (upgradeIndices[i] != -1)
                {
                    ItemSlot slot_i = player.inventory.slots[upgradeIndices[i]];
                    slot_i.amount -= 1;
                    player.inventory.slots[upgradeIndices[i]] = slot_i;
                }
            }
        }
    }
}