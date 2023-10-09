// Base type for damage skill templates.
// => there may be target damage, targetless damage, aoe damage, etc.
using System.Text;
using UnityEngine;

public abstract class DamageSkill : ScriptableSkill
{
    [Header("Damage")]
    public LinearInt physicalBonusDamage = new LinearInt{baseValue=1};
    public LinearInt physicalDamagePercent = new LinearInt { baseValue = 1 };
    public LinearInt magicalBonusDamage = new LinearInt { baseValue = 1 };
    public LinearInt magicalDamagePercent = new LinearInt { baseValue = 1 };

    public LinearFloat stunChance; // range [0,1]
    public LinearFloat stunTime; // in seconds
    public DamageType damageType;

    public int GetDamage(int combatPhysicalAttack, int combatMagicalAttack, int skillLevel)
    {
        if(damageType == DamageType.Physical)
        {
            return (int)(physicalBonusDamage.Get(skillLevel) + combatPhysicalAttack / 100.0*physicalDamagePercent.Get(skillLevel));
        }
        else
        {
            return (int)(magicalBonusDamage.Get(skillLevel) + combatMagicalAttack / 100.0 * magicalDamagePercent.Get(skillLevel));
        }
    }
    // tooltip
    public override string ToolTip(int skillLevel, bool showRequirements = false)
    {
        StringBuilder tip = new StringBuilder(base.ToolTip(skillLevel, showRequirements));
        StringBuilder damageText = new StringBuilder();
        if(physicalDamagePercent.Get(skillLevel) > 0)
        {
            damageText.Append($"<color=red>{physicalDamagePercent.Get(skillLevel)}%</color>");
        }
        if(magicalDamagePercent.Get(skillLevel) > 0)
        {
            damageText.Append((damageText.Length != 0 ? " + " : "") + $"<color=blue>{magicalDamagePercent.Get(skillLevel)}%</color>");
        }
        if (physicalBonusDamage.Get(skillLevel) > 0)
        {
            damageText.Append((damageText.Length != 0 ? " + " : "") + $"<color=red>{physicalBonusDamage.Get(skillLevel)}%</color>");
        }
        if (magicalBonusDamage.Get(skillLevel) > 0)
        {
            damageText.Append((damageText.Length != 0 ? " + " : "") + $"<color=blue>{magicalBonusDamage.Get(skillLevel)}%</color>");
        }
        tip.Replace("{DAMAGE}", damageText.ToString());



        tip.Replace("{STUNCHANCE}", Mathf.RoundToInt(stunChance.Get(skillLevel) * 100).ToString());
        tip.Replace("{STUNTIME}", stunTime.Get(skillLevel).ToString("F1"));
        return tip.ToString();
    }
}
