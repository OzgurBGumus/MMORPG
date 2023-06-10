using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;
using Mirror;
using System;

/*public partial class EquipmentItem
{
    [Header("GFF Bonus Aggro")]
    [Range(0, 1)] public float bonusAggro; // range [0,1]

    // tooltip
    void ToolTip_Aggro(StringBuilder tip)
    {
        tip.Replace("{AGGROBONUS}", Mathf.RoundToInt(bonusAggro * 100).ToString());
    }
}*/

/*public partial class BonusSkill
{
    [Header("GFF Bonus Aggro")]
    [FormerlySerializedAs("bonusAggro")] public LinearFloat aggroBonus; // range [0,1]

    // tooltip
    void ToolTip_Aggro(StringBuilder tip, int skillLevel)
    {
        tip.Replace("{BONUSAGGRO}", aggroBonus.Get(skillLevel).ToString());
    }
}*/

/*public partial struct Buff
{
    public float bonusAggro { get { return data.aggroBonus.Get(level); } }
}*/

public partial class Entity
{
    void DealDamageAt_Aggro(Entity entity, int amount, int damageDealt, DamageType damageType)
    {
        if (entity is Monster monster)
        {
            if (damageType == DamageType.Normal) monster.UpdateAggroList(this, 1);
            else if (damageType == DamageType.Crit) monster.UpdateAggroList(this, 3);
        }
    }
}

public partial class Monster
{
    [Serializable]public class AggroList
    {
        public Entity entity;
        public int value;
    }

    [Header("GFF Aggro Extended Addon")]
    public GameObject aggroSprite;
    public AudioClip aggroSound;
    public float aggroTime = 3f;
    public int aggroValueMax = 50;
    public List<AggroList> aggroList = new List<AggroList>();
    [SyncVar, HideInInspector] public double aggroTimeEnd;

    [Server]public void UpdateAggroList(Entity entity, int aggroValue)
    {
        if (aggroValue > 0)
        {
            //if aggro list is null 
            if (aggroList.Count == 0)
            {
                aggroList.Add(new AggroList() { entity = entity, value = aggroValueMax });
                target = entity;
                aggroTimeEnd = NetworkTime.time + aggroTime;
            }
            else
            {
                // if entity found in list
                int entityIndex = aggroList.FindIndex(x => x.entity == entity);
                if (entityIndex != -1)
                {
                    if (target == null)
                    {
                        aggroList.Add(new AggroList() { entity = entity, value = aggroValueMax });
                        target = entity;
                        aggroTimeEnd = NetworkTime.time + aggroTime;
                    }
                    else
                    {
                        if (aggroList[entityIndex].value + aggroValue <= aggroValueMax)
                            aggroList[entityIndex].value += aggroValue;
                        else
                        {
                            for (int i = 0; i < aggroList.Count; i++)
                                aggroList[i].value = 0;

                            aggroList[entityIndex].value = aggroValueMax;
                        }
                    }
                }
                else
                {
                    if (aggroValue == aggroValueMax)
                    {
                        for (int i = 0; i < aggroList.Count; i++)
                            aggroList[i].value = 0;
                    }

                    aggroList.Add(new AggroList() { entity = entity, value = aggroValue });
                }
            }
        }
        else
        {
            int entityIndex = aggroList.FindIndex(x => x.entity == entity);
            aggroList.RemoveAt(entityIndex);

            aggroList.Sort((a, b) => b.value.CompareTo(a.value));
            for (int i = 1; i < aggroList.Count; i++)
                aggroList[i].value = 0;

            target = aggroList[0].entity;
            aggroList[0].value = aggroValueMax;
            aggroTimeEnd = NetworkTime.time + aggroTime;
        }
    }
}
