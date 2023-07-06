using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestInfos : MonoBehaviour
{
    public GameObject panel;
    public Transform content;
    public UIQuestInfoSlot slotPrefab;

    public string expandPrefix = "[+] ";
    public string hidePrefix = "[-] ";

    // Update is called once per frame
    void Update()
    {
        Player player = Player.localPlayer;
        if (player != null)
        {
            // only show active quests, no completed ones
            List<Quest> allQuests = player.quests.quests.Where(q => !q.completed).ToList();
            // instantiate/destroy enough slots
            UIUtils.BalancePrefabs(slotPrefab.gameObject, allQuests.Count, content);

            // refresh all
            for (int i = 0; i < allQuests.Count; ++i)
            {
                UIQuestInfoSlot slot = content.GetChild(i).GetComponent<UIQuestInfoSlot>();
                Quest quest = allQuests[i];

                // name button
                GameObject descriptionPanel = slot.descriptionText.gameObject;
                string prefix = descriptionPanel.activeSelf ? hidePrefix : expandPrefix;
                slot.nameButton.GetComponentInChildren<Text>().text = prefix + quest.name;
                slot.nameButton.onClick.SetListener(() => {
                    descriptionPanel.SetActive(!descriptionPanel.activeSelf);
                });

                // description
                slot.descriptionText.text = quest.ToolTip(player, true);
            }
        }
    }
}
