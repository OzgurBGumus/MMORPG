using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIQuestInfos : MonoBehaviour
{
    public static UIQuestInfos singleton;
    public GameObject panel;
    public Transform content;
    public UIQuestInfoSlot slotPrefab;

    public string expandPrefix = "[+] ";
    public string hidePrefix = "[-] ";
    public Targets targets;

    public UIQuestInfos()
    {
        // assign singleton only once (to work with DontDestroyOnLoad when
        // using Zones / switching scenes)
        if (singleton == null) singleton = this;
    }

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

    public void OnEntryClicked(PointerEventData eventData, UIQuestInfoSlot slot)
    {
        var text = slot.descriptionText;
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(text, Input.mousePosition, null);
            if (linkIndex > -1)
            {
                string linkID = text.textInfo.linkInfo[linkIndex].GetLinkID();
                Vector3 bestDestination = new Vector3 (0,0,0);
                Player player = Player.localPlayer;
                PlayerNavMeshMovement playerNavMeshMovement = player.GetComponent<PlayerNavMeshMovement>();
                if (targets.TargetPositions.ContainsKey(linkID.ToLower()))
                {
                    bestDestination = playerNavMeshMovement.NearestValidDestination(targets.TargetPositions[linkID.ToLower()]);

                    // casting or stunned? then set pending destination
                    if (player.state == "STUNNED")
                    {
                        player.pendingDestination = bestDestination;
                        player.pendingDestinationValid = true;
                    }
                    // otherwise navigate there
                    else
                    {
                        playerNavMeshMovement.Navigate(bestDestination, 0);
                        if (player.state == "CASTING")
                        {
                            player.SetNextMove(bestDestination);
                            player.UpdateState();
                        }
                    }
                }

                
            }
        }
    }

}
