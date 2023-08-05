using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class UIPartyLootAuctionHUD : MonoBehaviour
{
    public GameObject panel;
    public UIPartyLootAuctionHUDItemSlot slotPrefab;
    public Transform itemContent;
    public Transform resultContent;
    public UIPartyLootAuctionResultHUDItemSlot resultSlotPrefab;
    //[Range(0,1)] public float visiblityAlphaRange = 0.5f;
    public AnimationCurve alphaCurve;

    private List<string> uniqueIds;
    private void Start()
    {
        uniqueIds = new List<string>();
    }
    void Update()
    {
        Player player = Player.localPlayer;

        // only show and update while there are party members
        if (player != null)
        {
            if (player.party.InParty())
            {
                ScriptableItem data;
                for(int i=0; i< player.party.party.auctionItems.Length; i++)
                {
                    if(!uniqueIds.Contains(player.party.party.auctionItems[i].uniqueId))
                    {
                        if (ScriptableItem.All.TryGetValue(player.party.party.auctionItems[i].hashCode, out data))
                        {
                            //CREATE Auction Item UI
                            UIPartyLootAuctionHUDItemSlot auctionSlot = GameObject.Instantiate(slotPrefab, itemContent, false);
                            auctionSlot.auctionItem = player.party.party.auctionItems[i];
                            auctionSlot.itemImage.image.sprite = data.image;
                        }
                        uniqueIds.Add(player.party.party.auctionItems[i].uniqueId);
                    }
                }
                if(uniqueIds.Count > 0)
                {
                    for (int i = uniqueIds.Count - 1; i >= 0; i--)
                    {
                        if (!(player.party.party.auctionItems.Where(x => x.uniqueId == uniqueIds[i]).Any()))
                        {
                            uniqueIds.RemoveAt(i);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(player.party.party.endedAuctionWinner) && ScriptableItem.All.TryGetValue(player.party.party.endedAuctionHashCode, out data))
                {
                    //CREATE Auction result Item UI
                    UIPartyLootAuctionResultHUDItemSlot auctionResultSlot = GameObject.Instantiate(resultSlotPrefab, resultContent, false);
                    auctionResultSlot.winner.text = player.party.party.endedAuctionWinner;
                    auctionResultSlot.itemImage.sprite = data.image;

                    //Remove the auctionItem.
                    player.party.party.endedAuctionWinner = "";
                    player.party.party.endedAuctionHashCode = 0;
                }

            }
        }
    }
}