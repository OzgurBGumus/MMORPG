using System;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.Events;

public enum DamageType { Physical, Magical, Block, Crit }

// inventory, attributes etc. can influence max health
public interface ICombatBonus
{
    int GetCritBonus();
    int GetHitBonus();
    int GetDodgeBonus();


    int GetPhysicalDefenseBonus();
    int GetPhysicalDefenseReductionBonus();
    int GetMagicalDefenseBonus();
    int GetMagicalDefenseReductionBonus();



    int GetPhysicalAttackBonus();
    int GetMagicalAttackBonus();

    int GetHealthBonus();
    int GetHealthRecoveryBonus();

    int GetManaBonus();
    int GetManaRecoveryBonus();

    int GetAttackSpeedBonus();
    int GetCastSpeedBonus();
    int GetMoveSpeedBonus();
    int GetLuckBonus();
    int GetCritDamageBonus();


}

[Serializable] public class UnityEventIntDamageType : UnityEvent<int, DamageType> {}

[DisallowMultipleComponent]
public class Combat : NetworkBehaviour
{
    [Header("Components")]
    public Level level;
    public Entity entity;
#pragma warning disable CS0109 // member does not hide accessible member
    public new Collider collider;
#pragma warning restore CS0109 // member does not hide accessible member

    [Header("Stats")]
    [SyncVar] public bool invincible = false; // GMs, Npcs, ...
    public int basePhysicalDamage;
    public int baseMagicalDamage;
    public int basePhysicalDefense;
    public int basePhysicalDefenseReduction;
    public int baseMagicalDefense;
    public int baseMagicalDefenseReduction;
    public int baseCrit;
    public int baseCritDamage;
    public int baseDodge;
    public int baseHit;

    [Header("Damage Popup")]
    public GameObject damagePopupPrefab;

    // events
    [Header("Events")]
    public UnityEventEntity onDamageDealtTo;
    public UnityEventEntity onKilledEnemy;
    public UnityEventEntityInt onServerReceivedDamage;
    public UnityEventIntDamageType onClientReceivedDamage;

    // cache components that give a bonus (attributes, inventory, etc.)
    ICombatBonus[] _bonusComponents;
    ICombatBonus[] bonusComponents =>
        _bonusComponents ?? (_bonusComponents = GetComponents<ICombatBonus>());

    PlayerAttribute[] _playerAttributes;
    PlayerAttribute[] playerAttributes =>
        _playerAttributes ?? (_playerAttributes = GetComponents<PlayerAttribute>());

    // calculate PATK
    public int physicalAttack
    {
        get
        {
            // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
            int bonus = 0;
            foreach (ICombatBonus bonusComponent in bonusComponents)
                bonus += bonusComponent.GetPhysicalAttackBonus();
            foreach(PlayerAttribute attr in playerAttributes)
                if(attr is IPhysicalAttackBonus)
                    bonus += ((IPhysicalAttackBonus)attr).GetPhysicalAttackBonus();
            return basePhysicalDamage + bonus;
        }
    }

    //Calculate MATK
    public int magicalAttack
    {
        get
        {
            // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
            int bonus = 0;
            foreach (ICombatBonus bonusComponent in bonusComponents)
                bonus += bonusComponent.GetMagicalAttackBonus();
            foreach (PlayerAttribute attr in playerAttributes)
                if (attr is IMagicalAttackBonus)
                    bonus += ((IMagicalAttackBonus)attr).GetMagicalAttackBonus();
            return baseMagicalDamage + bonus;
        }
    }

    // calculate physical defense
    public int physicalDefense
    {
        get
        {
            // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
            int bonus = 0;
            foreach (ICombatBonus bonusComponent in bonusComponents)
                bonus += bonusComponent.GetPhysicalDefenseBonus();
            foreach (PlayerAttribute attr in playerAttributes)
                if (attr is IPhysicalDefenseBonus)
                    bonus += ((IPhysicalDefenseBonus)attr).GetPhysicalDefenseBonus();
            return basePhysicalDefense + bonus;
        }
    }

    // calculate physical defense reduction
    public int physicalDefenseReduction
    {
        get
        {
            // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
            int bonus = 0;
            foreach (ICombatBonus bonusComponent in bonusComponents)
                bonus += bonusComponent.GetPhysicalDefenseReductionBonus();
            return basePhysicalDefenseReduction + bonus;
        }
    }

    // calculate magical defense
    public int magicalDefense
    {
        get
        {
            // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
            int bonus = 0;
            foreach (ICombatBonus bonusComponent in bonusComponents)
                bonus += bonusComponent.GetMagicalDefenseBonus();
            foreach (PlayerAttribute attr in playerAttributes)
                if (attr is IMagicalDefenseBonus)
                    bonus += ((IMagicalDefenseBonus)attr).GetMagicalDefenseBonus();
            return baseMagicalDefense + bonus;
        }
    }

    // calculate magical defense reduction
    public int magicalDefenseReduction
    {
        get
        {
            // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
            int bonus = 0;
            foreach (ICombatBonus bonusComponent in bonusComponents)
                bonus += bonusComponent.GetMagicalDefenseReductionBonus();
            return baseMagicalDefenseReduction + bonus;
        }
    }

    // calculate dodge
    public int dodge
    {
        get
        {
            // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
            int bonus = 0;
            foreach (ICombatBonus bonusComponent in bonusComponents)
                bonus += bonusComponent.GetDodgeBonus();
            foreach (PlayerAttribute attr in playerAttributes)
                if (attr is IDodgeBonus)
                    bonus += ((IDodgeBonus)attr).GetDodgeBonus();
            return baseDodge + bonus;
        }
    }

    // calculate crit
    public int crit
    {
        get
        {
            // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
            int bonus = 0;
            foreach (ICombatBonus bonusComponent in bonusComponents)
                bonus += bonusComponent.GetCritBonus();
            foreach (PlayerAttribute attr in playerAttributes)
                if (attr is ICritBonus)
                    bonus += ((ICritBonus)attr).GetCritBonus();
            return baseCrit + bonus;
        }
    }

    // calculate crit damage
    public int critDamage
    {
        get
        {
            // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
            int bonus = 0;
            foreach (ICombatBonus bonusComponent in bonusComponents)
                bonus += bonusComponent.GetCritDamageBonus();
            return baseCritDamage + bonus;
        }
    }

    // calculate hit damage
    public int hit
    {
        get
        {
            // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
            int bonus = 0;
            foreach (ICombatBonus bonusComponent in bonusComponents)
                bonus += bonusComponent.GetHitBonus();
            foreach (PlayerAttribute attr in playerAttributes)
                if (attr is IHitBonus)
                    bonus += ((IHitBonus)attr).GetHitBonus();
            return baseHit + bonus;
        }
    }

    // combat //////////////////////////////////////////////////////////////////
    // deal damage at another entity
    // (can be overwritten for players etc. that need custom functionality)
    [Server]
    public virtual void DealDamageAt(Entity victim, string from, int amount, DamageType damageType, float stunChance=0, float stunTime=0)
    {
        Combat victimCombat = victim.combat;
        int damageDealt = 0;

        // don't deal any damage if entity is invincible
        if (!victimCombat.invincible)
        {
            // block? (we use < not <= so that block rate 0 never blocks)
            if (victim.combat.hit < victimCombat.dodge)
            {
                damageType = DamageType.Block;
            }
            // deal damage
            else
            {
                // subtract defense (but leave at least 1 damage, otherwise
                // it may be frustrating for weaker players)
                int receivedDamage = 0;
                if (damageType == DamageType.Physical) receivedDamage = Convert.ToInt32((amount - victim.combat.physicalDefense) * (victim.combat.physicalDefenseReduction == 0 ? 1 : victim.combat.physicalDefenseReduction / 100.0));
                else if(damageType == DamageType.Magical) receivedDamage = Convert.ToInt32((amount - victim.combat.magicalDefense) * (victim.combat.magicalDefenseReduction == 0 ? 1 : victim.combat.magicalDefenseReduction / 100.0));
                damageDealt = Mathf.Max(receivedDamage, 1);

                // critical hit?
                if (damageType == DamageType.Physical && victim.combat.physicalDefense < UnityEngine.Random.Range(0, crit))
                {
                    damageDealt *= 2;
                    damageType = DamageType.Crit;
                }

                // deal the damage
                victim.health.current -= damageDealt;

                //SHOULD ONLY CALL IF VICTIM IS NOT A PLAYER
                if(!(victim is Player))
                    victim.AddReceivedDamage(new ReceivedDamage() { from = from, damage = damageDealt, time=DateTime.Now});

                // call OnServerReceivedDamage event on the target
                // -> can be used for monsters to pull aggro
                // -> can be used by equipment to decrease durability etc.
                victimCombat.onServerReceivedDamage.Invoke(entity, damageDealt);

                // stun?
                if (UnityEngine.Random.value < stunChance)
                {
                    // dont allow a short stun to overwrite a long stun
                    // => if a player is hit with a 10s stun, immediately
                    //    followed by a 1s stun, we don't want it to end in 1s!
                    double newStunEndTime = NetworkTime.time + stunTime;
                    victim.stunTimeEnd = Math.Max(newStunEndTime, entity.stunTimeEnd);
                }
            }

            // call OnDamageDealtTo / OnKilledEnemy events
            onDamageDealtTo.Invoke(victim);
            if (victim.health.current == 0)
                onKilledEnemy.Invoke(victim);
        }

        // let's make sure to pull aggro in any case so that archers
        // are still attacked if they are outside of the aggro range
        victim.OnAggro(entity);
        
        // show effects on clients
        victimCombat.RpcOnReceivedDamaged(damageDealt, damageType);
        

        // reset last combat time for both
        entity.lastCombatTime = NetworkTime.time;
        victim.lastCombatTime = NetworkTime.time;
    }

    // no need to instantiate damage popups on the server
    // -> calculating the position on the client saves server computations and
    //    takes less bandwidth (4 instead of 12 byte)
    [Client]
    void ShowDamagePopup(int amount, DamageType damageType)
    {
        // spawn the damage popup (if any) and set the text
        if (damagePopupPrefab != null)
        {
            // showing it above their head looks best, and we don't have to use
            // a custom shader to draw world space UI in front of the entity
            Bounds bounds = collider.bounds;
            Vector3 position = new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);

            GameObject popup = Instantiate(damagePopupPrefab, position, Quaternion.identity);
            if (damageType == DamageType.Physical)
            {
                popup.GetComponentInChildren<TextMeshPro>().text = amount.ToString();
            }
            else if(damageType == DamageType.Magical)
            {
                popup.GetComponentInChildren<TextMeshPro>().color = Color.red;
                popup.GetComponentInChildren<TextMeshPro>().text = amount.ToString();
            }
            else if (damageType == DamageType.Block)
                popup.GetComponentInChildren<TextMeshPro>().text = "<i>Block!</i>";
            else if (damageType == DamageType.Crit)
                popup.GetComponentInChildren<TextMeshPro>().text = amount + " Crit!";
        }
    }

    [ClientRpc]
    public void RpcOnReceivedDamaged(int amount, DamageType damageType)
    {
        // show popup above receiver's head in all observers via ClientRpc
        ShowDamagePopup(amount, damageType);

        // call OnClientReceivedDamage event
        onClientReceivedDamage.Invoke(amount, damageType);
    }
}
