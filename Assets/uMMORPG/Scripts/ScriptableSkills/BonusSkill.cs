// Base type for bonus skill templates.
// => can be used for passive skills, buffs, etc.
using System.Text;
using UnityEngine;

public abstract class BonusSkill : ScriptableSkill
{
    public LinearInt healthMaxBonus;
    public LinearInt healthPercentBonus; //not read for yet
    public LinearInt healthPercentPerSecondBonus;

    public LinearInt manaMaxBonus;
    public LinearInt manaPercentBonus;  //not read for yet
    public LinearInt manaPercentPerSecondBonus;

    public LinearInt PhysicalAttackBonus;
    public LinearInt MagicalAttackBonus;
    public LinearInt physicalDefenseBonus;
    public LinearInt magicalDefenseBonus;
    public LinearInt physicalDefenseReductionBonus;
    public LinearInt magicalDefenseReductionBonus;

    public LinearInt critBonus;
    public LinearInt critDamageBonus;
    public LinearInt dodgeBonus;
    public LinearInt hitBonus;
    public LinearInt attackSpeedBonus;
    public LinearInt castSpeedBonus;
    public LinearInt moveSpeedBonus;
    public LinearInt luckBonus;
    

    // tooltip
    public override string ToolTip(int skillLevel, bool showRequirements = false)
    {
        StringBuilder tip = new StringBuilder(base.ToolTip(skillLevel, showRequirements));
        tip.Replace("{HEALTHMAXBONUS}", healthMaxBonus.Get(skillLevel).ToString());
        tip.Replace("{HEALTHPERCENTBONUS}", healthPercentBonus.Get(skillLevel).ToString());
        tip.Replace("{HEALTHPERCENTPERSECONDBONUS}", healthPercentPerSecondBonus.Get(skillLevel).ToString());

        tip.Replace("{MANAMAXBONUS}", manaMaxBonus.Get(skillLevel).ToString());
        tip.Replace("{MANAPERCENTBONUS}", manaPercentBonus.Get(skillLevel).ToString());
        tip.Replace("{MANAPERCENTPERSECONDBONUS}", manaPercentPerSecondBonus.Get(skillLevel).ToString());

        tip.Replace("{PATKBONUS}", PhysicalAttackBonus.Get(skillLevel).ToString());
        tip.Replace("{MATKBONUS}", MagicalAttackBonus.Get(skillLevel).ToString());
        tip.Replace("{PDEFBONUS}", physicalDefenseBonus.Get(skillLevel).ToString());
        tip.Replace("{MDEFBONUS}", magicalDefenseBonus.Get(skillLevel).ToString());
        tip.Replace("{PDEFREDUCTIONBONUS}", physicalDefenseReductionBonus.Get(skillLevel).ToString());
        tip.Replace("{MDEFREDUCTIONBONUS}", magicalDefenseReductionBonus.Get(skillLevel).ToString());
        tip.Replace("{CRITBONUS}", critBonus.Get(skillLevel).ToString());
        tip.Replace("{CRITDAMAGEBONUS}", critDamageBonus.Get(skillLevel).ToString());
        tip.Replace("{DODGEBONUS}", dodgeBonus.Get(skillLevel).ToString());
        tip.Replace("{HITBONUS}", hitBonus.Get(skillLevel).ToString());
        tip.Replace("{ATTACKSPEEDBONUS}", attackSpeedBonus.Get(skillLevel).ToString());
        tip.Replace("{CASTSPEEDBONUS}", castSpeedBonus.Get(skillLevel).ToString());
        tip.Replace("{MOVESPEEDBONUS}", moveSpeedBonus.Get(skillLevel).ToString());
        tip.Replace("{LUCKBONUS}", luckBonus.Get(skillLevel).ToString());
        return tip.ToString();
    }
}
