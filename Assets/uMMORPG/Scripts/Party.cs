// Parties have to be structs in order to work with SyncLists.

using System.Collections.Generic;

public struct Party
{
    // Guild.Empty for ease of use
    public static Party Empty = new Party();

    // properties
    public int partyId;
    public string[] members; // first one == master
    public bool shareExperience;
    public bool shareGold;

    public int shareLoot;
    public int nextLootOwner;
    public AuctionItem[] auctionItems;
    public string endedAuctionWinner;
    public int endedAuctionHashCode;
    // helper properties
    public string master => members != null && members.Length > 0 ? members[0] : "";

    // statics
    public static int Capacity = 8;
    public static float BonusExperiencePerMember = 0.1f;

    // if we create a party then always with two initial members
    public Party(int partyId, string master, string firstMember)
    {
        // create members array
        this.partyId = partyId;
        members = new string[]{master, firstMember};
        shareExperience = false;
        shareGold = false;
        shareLoot = 0;
        nextLootOwner = 0;
        auctionItems = new AuctionItem[] { };
        endedAuctionWinner = "";
        endedAuctionHashCode = 0;
    }

    public bool Contains(string memberName)
    {
        if (members != null)
            foreach (string member in members)
                if (member == memberName)
                    return true;
        return false;
    }

    public bool IsFull()
    {
        return members != null && members.Length == Capacity;
    }
}

public struct AuctionItem
{

    public string winnerPlayer { get; set; }
    public string[] acceptableMembers { get; set; }
    public string[] bids { get; set; }
    public string[] rejectedBids { get; set; }
    public int hashCode;
    public string uniqueId;
    public int stack;
    public AuctionItem(int hashCode, string uniqueId, int stack)
    {
        this.hashCode = hashCode;
        this.uniqueId = uniqueId;
        this.stack = stack;
        acceptableMembers = new string[0];
        bids = new string[0];
        rejectedBids = new string[0];
        winnerPlayer = null;
    }
} 