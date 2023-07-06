// Note: this script has to be on an always-active UI parent, so that we can
// always react to the hotkey.
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class UISkills : MonoBehaviour
{
    public KeyCode hotKey = KeyCode.R;
    public GameObject panel;
    public UISkillSlot slotPrefab;
    public Transform[] content;
    public Text skillExperienceText;

    private Dictionary<Skill, int> rootSkills;
    private Player player;
    private int currentContent;
    private void Start()
    {

        rootSkills = new Dictionary<Skill, int>();
    }
    void Update()
    {
        player = Player.localPlayer;
        if (player)
        {
            if(rootSkills.Count == 0)
            {
                currentContent = 0;
                for (int i = 0; i < player.skills.skills.Count; i++)
                {
                    if (!player.skills.skills[i].learnDefault && player.skills.skills[i].predecessor == null)
                    {
                        rootSkills.Add(player.skills.skills[i], i);
                    }
                }
            }
            // hotkey (not while typing in chat, etc.)
            if (Input.GetKeyDown(hotKey) && !UIUtils.AnyInputActive())
                panel.SetActive(!panel.activeSelf);

            // only update the panel if it's active
            if (panel.activeSelf)
            {
                // instantiate/destroy enough slots
                // (we only care about non status skills)

                for(int i = 0; i < 3; i++)
                {
                    UIUtils.BalancePrefabs(slotPrefab.gameObject, 8, content[i]);
                }
                

                // refresh all
                foreach(var skill in rootSkills)
                {
                    int contentIndex = player.skills.skills[skill.Value].data.contentSlot;
                    if (contentIndex == currentContent)
                    {

                        RefreshRootSkill(skill.Value);
                    }
                }

                // skill experience
                skillExperienceText.text = ((PlayerSkills)player.skills).skillExperience.ToString();
            }
        }
        else panel.SetActive(false);
    }

    private void RefreshChildSkill(ScriptableSkill data, int treeSlot, int parentsIndexInTree)
    {
        Skill skill = player.skills.skills.Find(x => x.predecessor == data);
        
        //So we have a skill which is data's child.
        if (skill.hash != 0)
        {
            int indexInTree = skill.data.requiredLevel.baseValue / 10;
            while (parentsIndexInTree + 1 != indexInTree)
            {
                parentsIndexInTree++;
                UISkillSlot noSkillSlot = content[treeSlot].GetChild(parentsIndexInTree).GetComponent<UISkillSlot>();
                //in here, change the slot to Arrow or empty;

            }

            UISkillSlot slot = content[treeSlot].GetChild(indexInTree).GetComponent<UISkillSlot>();
            RefreshSkill(player.skills.skills.FindIndex(x=> x.name == skill.name), slot, skill);
            RefreshChildSkill(skill.data, treeSlot, indexInTree);
        }
    }

    private void RefreshRootSkill(int skillIndexInSkills)
    {
        Skill skill = player.skills.skills[skillIndexInSkills];
        int indexInTree = skill.data.requiredLevel.baseValue / 10;
        int treeInSlot = skill.data.treeInSlot;
        UISkillSlot slot = content[treeInSlot].GetChild(indexInTree).GetComponent<UISkillSlot>();
        
        RefreshSkill(skillIndexInSkills, slot, skill);
        RefreshChildSkill(skill.data, treeInSlot, indexInTree);
    }

    private void RefreshSkill(int skillIndexInSkills, UISkillSlot slot, Skill skill)
    {
        bool isPassive = skill.data is PassiveSkill;

        // set state
        slot.dragAndDropable.name = (skill.data.requiredLevel.baseValue / 10).ToString();
        slot.dragAndDropable.dragable = skill.level > 0 && !isPassive;

        // can we cast it? checks mana, cooldown etc.
        bool canCast = player.skills.CastCheckSelf(skill);

        // if movement does NOT support navigation then we need to
        // check distance too. otherwise distance doesn't matter
        // because we can navigate anywhere.
        if (!player.movement.CanNavigate())
            canCast &= player.skills.CastCheckDistance(skill, out Vector3 _);

        // click event
        slot.button.interactable = skill.level > 0 &&
                                   !isPassive &&
                                   canCast;

        int icopy = skillIndexInSkills;
        slot.button.onClick.SetListener(() =>
        {
            // try use the skill or walk closer if needed
            ((PlayerSkills)player.skills).TryUse(icopy);
        });

        // image
        if (skill.level > 0)
        {
            slot.image.color = Color.white;
            slot.image.sprite = skill.image;
        }

        // description
        //slot.descriptionText.text = skill.ToolTip(showRequirements: skill.level == 0);

        // learn / upgrade
        if (skill.level < skill.maxLevel && ((PlayerSkills)player.skills).CanUpgrade(skill))
        {
            slot.upgradeButton.gameObject.SetActive(true);
            slot.upgradeButton.onClick.SetListener(() => { ((PlayerSkills)player.skills).CmdUpgrade(icopy); });
        }
        //else slot.upgradeButton.gameObject.SetActive(false);

        // cooldown overlay
        float cooldown = skill.CooldownRemaining();
        slot.cooldownOverlay.SetActive(skill.level > 0 && cooldown > 0);
        slot.cooldownText.text = cooldown.ToString("F0");
        slot.cooldownCircle.fillAmount = skill.cooldown > 0 ? cooldown / skill.cooldown : 0;
    }
}
