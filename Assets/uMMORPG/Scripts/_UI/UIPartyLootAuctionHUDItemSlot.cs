using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPartyLootAuctionHUDItemSlot : MonoBehaviour
{
    public Button bidAccept;
    public Button bidReject;
    public UniversalSlot itemImage;
    public Image timerBarEmpty;
    public Image timerBarFilled;
    public float duration = 6f;
    private float timer = 0f;
    private bool timeEnd = false;
    public AuctionItem auctionItem;
    

    // Start is called before the first frame update
    void Start()
    {
        bidAccept.onClick.AddListener(() =>
        {
            Player player = Player.localPlayer;
            player.party.CmdAcceptBidToLoot(auctionItem.uniqueId);
            Destroy(this.gameObject);

        });
        bidReject.onClick.AddListener(() =>
        {
            RejectBid();
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (!timeEnd)
        {
            timer += Time.deltaTime;
            float fillPercentage = timer / duration;
            timerBarFilled.fillAmount = Mathf.Clamp01(fillPercentage);
            if (timer >= duration)
            {
                //Make Auction Item Invisible with effect.
                RejectBid();
                timeEnd = true;

            }
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void RejectBid()
    {
        Player player = Player.localPlayer;
        player.party.CmdRejectBidToLoot(auctionItem.uniqueId);
        Destroy(this.gameObject);
    }
}
