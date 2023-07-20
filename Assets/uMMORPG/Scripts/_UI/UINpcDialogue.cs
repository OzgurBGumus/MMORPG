// Note: this script has to be on an always-active UI parent, so that we can
// always find it from other code. (GameObject.Find doesn't find inactive ones)
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class UINpcDialogue : MonoBehaviour
{
    public static UINpcDialogue singleton;
    public GameObject panel;
    public Text welcomeText;
    public Transform offerPanel;
    public GameObject offerButtonPrefab;

    public UINpcDialogue()
    {
        // assign singleton only once (to work with DontDestroyOnLoad when
        // using Zones / switching scenes)
        if (singleton == null) singleton = this;
    }

    void Update()
    {
        Player player = Player.localPlayer;

        // use collider point(s) to also work with big entities
        if (player != null &&
            panel.activeSelf &&
            player.target != null &&
            player.target is Npc npc &&
            Utils.ClosestDistance(player, player.target) <= player.interactionRange)
        {
            // welcome text
            welcomeText.text = npc.welcome;

            // count amount of valid offers
            int validOffers = 0;
            foreach (NpcOffer offer in npc.offers)
                if (offer.HasOffer(player))
                    ++validOffers;

            List<ScriptableQuest> questsAvailable = npc.quests.QuestsVisibleFor(player);
            // instantiate enough buttons
            UIUtils.BalancePrefabs(offerButtonPrefab, validOffers+ questsAvailable.Count, offerPanel);

            // show a button for each valid offer
            int index = 0;
            foreach (NpcOffer offer in npc.offers)
            {
                if (offer.HasOffer(player))
                {
                    Button button = offerPanel.GetChild(index).GetComponent<Button>();
                    button.GetComponentInChildren<Text>().text = offer.GetOfferName();
                    button.onClick.SetListener(() => {
                        offer.OnSelect(player);
                    });
                    ++index;
                }
            }
            foreach(ScriptableQuest npcQuest in questsAvailable)
            {
                Button button = offerPanel.GetChild(index).GetComponent<Button>();
                button.GetComponentInChildren<Text>().text = npcQuest.name;
                button.onClick.SetListener(() => {
                    npcQuest.OnSelect(player, npcQuest.name);
                });
                ++index;
            }
        }
        else panel.SetActive(false);
    }

    public void Show() {
        FindObjectOfType<Canvas>().GetComponent<UIUniqueWindow>().CloseWindows();
        panel.SetActive(true); 
    }
}
