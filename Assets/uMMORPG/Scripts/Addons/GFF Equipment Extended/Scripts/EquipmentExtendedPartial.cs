using Mirror;
using UnityEngine;

public partial class EquipmentItem
{
    public override void Use(Player player, int inventoryIndex)
    {
        // always call base function too
        base.Use(player, inventoryIndex);

        // find a slot that accepts this category, then equip it
        int equipmentIndex = FindEquipableSlotFor(player, inventoryIndex);

        if (equipmentIndex != -1)
        {
            //weapon (left or right hands)
            if (equipmentIndex == 0 || equipmentIndex == 4)
            {
                //if item is weapon (not shield)
                if (equipmentIndex == 0)
                {
                    //if shield slot is empty
                    if (player.equipment.slots[4].amount == 0) SwapSlots(player, equipmentIndex, inventoryIndex);
                    else
                    {
                        //if weapon only for one hand
                        if (((WeaponItem)player.inventory.slots[inventoryIndex].item.data).category == ItemCategory.WeaponSword) SwapSlots(player, equipmentIndex, inventoryIndex);
                        else
                        {
                            if (player.inventory.CanAdd(player.equipment.slots[4].item, 1))
                            {
                                player.inventory.Add(player.equipment.slots[4].item, 1);

                                ItemSlot temp = player.equipment.slots[4];
                                temp.amount = 0;
                                player.equipment.slots[4] = temp;

                                SwapSlots(player, equipmentIndex, inventoryIndex);
                            }
                        }
                    }
                }
                else
                {
                    //if the weapon slot is empty and the shield is worn ||
                    //weapon and shield slots are empty ||
                    //the shield slot is empty and the weapon is a sword ||
                    //the shield slot is full and the weapon is a sword
                    if ((player.equipment.slots[0].amount < 1 && player.equipment.slots[4].amount > 0) ||
                        (player.equipment.slots[4].amount < 1 && player.equipment.slots[0].amount < 1) ||
                        (player.equipment.slots[4].amount < 1 && player.equipment.slots[0].amount > 0 && ((WeaponItem)player.equipment.slots[0].item.data).category == ItemCategory.WeaponSword) ||
                        (player.equipment.slots[4].amount > 0 && player.equipment.slots[0].amount > 0 && ((WeaponItem)player.equipment.slots[0].item.data).category == ItemCategory.WeaponSword)
                        )
                    {
                        SwapSlots(player, equipmentIndex, inventoryIndex);
                    }
                    else
                    {

                        if (player.inventory.CanAdd(player.equipment.slots[0].item, 1))
                        {
                            player.inventory.Add(player.equipment.slots[0].item, 1);

                            ItemSlot tem = player.equipment.slots[0];
                            tem.amount = 0;
                            player.equipment.slots[0] = tem;
                            SwapSlots(player, equipmentIndex, inventoryIndex);
                        }
                    }
                }
            }

            //amulets
            else if (equipmentIndex == 8)
            {
                if (player.equipment.slots[equipmentIndex].amount < 1) SwapSlots(player, equipmentIndex, inventoryIndex);
                else SwapSlots(player, equipmentIndex + 1, inventoryIndex);
            }

            //rings
            else if (equipmentIndex == 10 || equipmentIndex == 11)
            {
                if (player.equipment.slots[equipmentIndex].amount < 1) SwapSlots(player, equipmentIndex, inventoryIndex);
                else SwapSlots(player, equipmentIndex + 1, inventoryIndex);
            }
            //earrings
            else if (equipmentIndex == 12 || equipmentIndex == 13)
            {
                if (player.equipment.slots[equipmentIndex].amount < 1) SwapSlots(player, equipmentIndex, inventoryIndex);
                else SwapSlots(player, equipmentIndex + 1, inventoryIndex);
            }

            else SwapSlots(player, equipmentIndex, inventoryIndex);
        }
    }

    void SwapSlots(Player player, int equipmentSlot, int inventorySlot)
    {
        ItemSlot temp = player.equipment.slots[equipmentSlot];
        player.equipment.slots[equipmentSlot] = player.inventory.slots[inventorySlot];
        player.inventory.slots[inventorySlot] = temp;
    }
}

public partial class PlayerEquipment
{
    [Command]
    public void CmdSwapEquipmentEquipment(int fromIndex, int toIndex)
    {
        // swap them
        ItemSlot temp = slots[fromIndex];
        slots[fromIndex] = slots[toIndex];
        slots[toIndex] = temp;
    }

    // drag & drop /////////////////////////////////////////////////////////////
    void OnDragAndDrop_EquipmentSlot_EquipmentSlot(int[] slotIndices)
    {
        if (((EquipmentItem)slots[slotIndices[0]].item.data).category == slotInfo[slotIndices[1]].requiredCategory)
        {
            CmdSwapEquipmentEquipment(slotIndices[0], slotIndices[1]);
            //player.PlaySoundItemEquip();
        }
    }
}


