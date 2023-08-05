// there are different ways to implement a party system:
//
// - Player.cs can have a '[SyncVar] party' and broadcast it to all party members
//   when something changes in the party. there is no one source of truth, which
//   makes this a bit weird. it works, but only until we need a global party
//   list, e.g. for dungeon instances.
//
// - Player.cs can have a Party class reference that all members share. Mirror
//   can only serialize structs, which makes syncing more difficult then. There
//   is also the question of null vs. not null and we would have to not only
//   kick/leave parties, but also never forget to set .party to null. This
//   results in a lot of complicated code.
//
// - PartySystem could have a list of Party classes. But then the client would
//   need to have a local syncedParty class, which makes .party access on server
//   and client different (and hence very difficult).
//
// - PartySystem could control the parties. When anything is changed, it
//   automatically sets each member's '[SyncVar] party' which Mirror syncs
//   automatically. Server and client could access Player.party to read anything
//   and use PartySystem to modify parties.
//
//   => This seems to be the best solution for a party system with Mirror.
//   => PartySystem is almost independent from Unity. It's just a party system
//      with names and partyIds.
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using UnityEngine;

public static class PartySystem
{
    static Dictionary<int, Party> parties = new Dictionary<int, Party>();

    // start partyIds at 1. 0 means no party, because default party struct's
    // partyId is 0.
    static int nextPartyId = 1;

    // copy party to someone
    static void BroadcastTo(string member, Party party)
    {
        if (Player.onlinePlayers.TryGetValue(member, out Player player))
            player.party.party = party;
    }

    // copy party to all members & save in dictionary
    static void BroadcastChanges(Party party)
    {
        foreach (string member in party.members)
            BroadcastTo(member, party);

        parties[party.partyId] = party;
    }

    // check if a partyId exists
    public static bool PartyExists(int partyId)
    {
        return parties.ContainsKey(partyId);
    }

    // creating a party requires at least two members. it's not a party if
    // someone is alone in it.
    public static void FormParty(string creator, string firstMember)
    {
        // create party
        int partyId = nextPartyId++;
        Party party = new Party(partyId, creator, firstMember);

        // broadcast and save in dict
        BroadcastChanges(party);
        Debug.Log(creator + " formed a new party with " + firstMember);
    }

    public static void AddToParty(int partyId, string member)
    {
        // party exists and not full?
        Party party;
        if (parties.TryGetValue(partyId, out party) && !party.IsFull())
        {
            // add to members
            Array.Resize(ref party.members, party.members.Length + 1);
            party.members[party.members.Length - 1] = member;

            // broadcast and save in dict
            BroadcastChanges(party);

            Player.onlinePlayers.TryGetValue(member, out Player player);
            for (int i = 0; i < party.auctionItems.Length; i++)
            {
                var rejectedBids = party.auctionItems[i].rejectedBids;
                // add to rejectedBids
                Array.Resize(ref rejectedBids, rejectedBids.Length + 1);
                party.auctionItems[i].rejectedBids = rejectedBids;
            }
            Debug.Log(member + " was added to party " + partyId);
        }
    }

    public static void KickFromParty(int partyId, string requester, string member)
    {
        // party exists?
        Party party;
        if (parties.TryGetValue(partyId, out party))
        {
            // requester is party master, member is in party, not same?
            if (party.master == requester && party.Contains(member) && requester != member)
            {
                // reuse the leave function
                LeaveParty(partyId, member);
            }
        }
    }

    public static void LeaveParty(int partyId, string member)
    {
        // party exists?
        Party party;
        if (parties.TryGetValue(partyId, out party))
        {
            // requester is not master but is in party?
            if (party.master != member && party.Contains(member))
            {
                // remove from list
                party.members = party.members.Where(name => name != member).ToArray();

                // still > 1 people?
                if (party.members.Length > 1)
                {
                    // broadcast and save in dict
                    BroadcastChanges(party);
                    BroadcastTo(member, Party.Empty); // clear for kicked person
                    for(int i=0; i<party.auctionItems.Length; i++)
                    {
                        string auctionCheck = party.auctionItems[i].bids.Where(p => p == member).FirstOrDefault();
                        if (auctionCheck!= null)
                        {
                            party.auctionItems[i].bids = party.auctionItems[i].bids.Where(p => p != auctionCheck).ToArray();
                        }
                        else
                        {
                            auctionCheck = party.auctionItems[i].rejectedBids.Where(p => p == member).FirstOrDefault();
                            if (auctionCheck != null)
                            {
                                party.auctionItems[i].rejectedBids = party.auctionItems[i].rejectedBids.Where(p => p != auctionCheck).ToArray();
                            }
                        }
                    }
                }
                // otherwise remove party. no point in having only 1 member.
                else
                {
                    // broadcast and remove from dict
                    BroadcastTo(party.members[0], Party.Empty); // clear for master
                    BroadcastTo(member, Party.Empty); // clear for kicked person
                    parties.Remove(partyId);
                }

                Debug.Log(member + " left the party");
            }
        }
    }

    public static void DismissParty(int partyId, string requester)
    {
        // party exists?
        Party party;
        if (parties.TryGetValue(partyId, out party))
        {
            // is master?
            if (party.master == requester)
            {
                // clear party for everyone
                foreach (string member in party.members)
                    BroadcastTo(member, Party.Empty);

                // remove from dict
                parties.Remove(partyId);
                Debug.Log(requester + " dismissed the party");
            }
        }
    }

    public static void SetPartyExperienceShare(int partyId, string requester, bool value)
    {
        // party exists and master?
        Party party;
        if (parties.TryGetValue(partyId, out party) && party.master == requester)
        {
            // set new value
            party.shareExperience = value;

            // broadcast and save in dict
            BroadcastChanges(party);
        }
    }

    public static void SetPartyGoldShare(int partyId, string requester, bool value)
    {
        // party exists and master?
        Party party;
        if (parties.TryGetValue(partyId, out party) && party.master == requester)
        {
            // set new value
            party.shareGold = value;

            // broadcast and save in dict
            BroadcastChanges(party);
        }
    }
    public static void SetPartyLootShare(int partyId, string requester, int value)
    {
        // party exists and master?
        Party party;
        if (parties.TryGetValue(partyId, out party) && party.master == requester)
        {
            // set new value
            party.shareLoot = value;

            // broadcast and save in dict
            BroadcastChanges(party);
        }
    }
    
    public static bool AddItemIntoMember(int partyId, string playerName, GameObject item, ScriptableItem data, string uniqueId, int stack)
    {
        Party party;
        if (parties.TryGetValue(partyId, out party))
        {
            Player player = Player.GetOnlinePlayer(playerName);
            // enough space in the inventory, pick up the item
            if (player != null && player.inventory.Add(new Item(data), stack))
            {
                if (ItemDropSettings.Settings.showMessages)
                {
                    string message = stack == 1 ? $"{player.name} got [{data.name}]" : $"{player.name} got [{data.name}] (<color=#ADFF2F>{stack}</color>)";
                    foreach(string name in party.members)
                    {
                        if (Player.onlinePlayers.TryGetValue(name, out Player member))
                        {
                            member.chat.TargetMsgInfo(message);
                        }
                    }
                }
                    

                DestroyLoot(item, uniqueId);
                return true;
            }
        }
        return false;
    }

    public static string AddItemIntoRandomMember(int partyId, string[] acceptableMembers, GameObject item, string uniqueId, int hashCode, int stack)
    {
        Party party;
        bool userGotTheItem = false;
        string ItemOwner = null;
        if (ScriptableItem.All.TryGetValue(hashCode, out var data))
        {
            if (parties.TryGetValue(partyId, out party))
            {
                int rand = 0;
                List<int> fullInventoryPlayers = new List<int>();
                System.Random random = new System.Random();
                while (!userGotTheItem)
                {
                    rand = random.Next(0, acceptableMembers.Length);
                    if (fullInventoryPlayers.Count != acceptableMembers.Length)
                    {
                        while (fullInventoryPlayers.Contains(rand))
                        {
                            rand = random.Next(0, acceptableMembers.Length);
                        }
                        userGotTheItem = AddItemIntoMember(partyId, acceptableMembers[rand], item, data, uniqueId, stack);
                        if (userGotTheItem)
                        {
                            ItemOwner = acceptableMembers[rand];
                        }
                        fullInventoryPlayers.Add(rand);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
        return ItemOwner;
    }

    public static bool AddItemIntoNextMember(int partyId, string[] acceptableMembers, GameObject item, ScriptableItem data, string uniqueId, int stack)
    {
        Party party;
        bool userGotTheItem = false;
        if (parties.TryGetValue(partyId, out party))
        {
            int startIndex = party.nextLootOwner;
            int next = party.nextLootOwner;
            while(acceptableMembers.Where(p => p == party.members[next]).FirstOrDefault() == null)
            {
                if ((next == startIndex - 1) || (startIndex == 0 && next == party.members.Length-1)) break;
                next = (next+1)%party.members.Length;
            }
            int acceptableNext = 0;
            for (int i=0;i< acceptableMembers.Length; i++)
            {
                if(acceptableMembers[i] == party.members[next])
                {
                    acceptableNext = i;
                    break;
                }
            }
            int acceptableStart = acceptableNext;
            List<int> fullInventoryPlayers = new List<int>();
            do
            {
                userGotTheItem = AddItemIntoMember(partyId, acceptableMembers[acceptableNext], item, data, uniqueId, stack);
                fullInventoryPlayers.Add(acceptableNext);
                acceptableNext = (acceptableNext + 1) % acceptableMembers.Length;
            } while (acceptableStart != acceptableNext && !userGotTheItem);
            if (userGotTheItem)
            {
                party.nextLootOwner = acceptableNext;
                parties[party.partyId] = party;
            }
        }
        return userGotTheItem;
    }

    public static void CmdAddItemIntoAuction(int partyId, GameObject item, AuctionItem auctionItem)
    {
        // party exists and not full?
        Party party;
        if (parties.TryGetValue(partyId, out party))
        {
            AuctionItem[] auctionItems = party.auctionItems;
            // add to members
            Array.Resize(ref auctionItems, auctionItems.Length + 1);
            auctionItems[auctionItems.Length - 1] = auctionItem;

            party.auctionItems = auctionItems;
            Player currentPlayer;
            BroadcastChanges(party);
            parties[partyId] = party;

            DestroyLoot(item, auctionItem.uniqueId);
            
        }
    }
    public static void CmdBidToAuction(int partyId, Player player, string lootUniqueID)
    {
        // party exists?
        Party party;
        if (parties.TryGetValue(partyId, out party))
        {
            bool playerInParty = party.members.Where(m=>m==player.name).Any();
            if (playerInParty)
            {
                for(int i=0; i< party.auctionItems.Length; i++)
                {
                    if(party.auctionItems[i].uniqueId == lootUniqueID)
                    {
                        if (!party.auctionItems[i].bids.Contains(player.name))
                        {
                            string[] bids = party.auctionItems[i].bids;
                            // add to bids
                            Array.Resize(ref bids, bids.Length + 1);
                            bids[bids.Length - 1] = player.name;
                            party.auctionItems[i].bids = bids;
                            if (party.auctionItems[i].bids.Length + party.auctionItems[i].rejectedBids.Length == party.members.Length)
                            {
                                CmdEndLootAuction(partyId, lootUniqueID);
                            }
                        }
                        break;
                    }
                }
            }
        }
    }
    public static void CmdRejectBidToAuction(int partyId, Player player, string lootUniqueID)
    {
        // party exists?
        Party party;
        if (parties.TryGetValue(partyId, out party))
        {
            bool playerInParty = party.members.Where(m => m == player.name).Any();
            if (playerInParty)
            {
                for (int i = 0; i < party.auctionItems.Length; i++)
                {
                    if (party.auctionItems[i].uniqueId == lootUniqueID)
                    {
                        if (!party.auctionItems[i].rejectedBids.Contains(player.name))
                        {
                            string[] rejectedBids = party.auctionItems[i].rejectedBids;
                            // add to bids
                            Array.Resize(ref rejectedBids, rejectedBids.Length + 1);
                            rejectedBids[rejectedBids.Length - 1] = player.name;
                            party.auctionItems[i].rejectedBids = rejectedBids;

                            if (party.auctionItems[i].bids.Length + party.auctionItems[i].rejectedBids.Length == party.members.Length)
                            {
                                CmdEndLootAuction(partyId, lootUniqueID);
                            }
                        }

                        break;
                    }
                }
            }
        }
    }

    public static void CmdEndLootAuction(int partyId, string lootUniqueID)
    {
        
        // party exists?
        Party party;
        if (parties.TryGetValue(partyId, out party))
        {

            AuctionItem auction = party.auctionItems.Where(item => item.uniqueId == lootUniqueID).FirstOrDefault();
            if(auction.acceptableMembers != null)
            {
                string winner = AddItemIntoRandomMember(partyId, auction.bids, null, auction.uniqueId, auction.hashCode, auction.stack);

                party.endedAuctionHashCode = auction.hashCode;
                party.endedAuctionWinner = winner;

                // remove from list
                party.auctionItems = party.auctionItems.Where(item => item.uniqueId != lootUniqueID).ToArray();
                BroadcastChanges(party);

                party.endedAuctionHashCode = 0;
                party.endedAuctionWinner = "";
                parties[party.partyId] = party;
            }
            
        }
    }
    public static void DestroyLoot(GameObject item, string uniqueId)
    {
        if (item != null)
        {
            //Debug.Log($"server item: {item}");
            AddonItemDrop.DeleteItem(uniqueId);
            NetworkServer.Destroy(item);
        }

    }
}

public enum LootShareEnum
{
    Individual,
    Random,
    Ordered,
    Auction
}
