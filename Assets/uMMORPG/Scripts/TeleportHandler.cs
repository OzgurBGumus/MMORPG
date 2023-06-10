
using Mirror;
using UnityEngine;

// This script is attached to a prefab called Zone
// NetworkManagerMMO, in OnStartServer_Additive, instantiates the prefab only on the server.
// It never exists for clients (other than host client if there is one).
// The prefab has a Sphere Collider with isTrigger = true.
// These OnTrigger events only run on the server and will only send a message to the
// client that entered the Zone to load the subscene assigned to the subscene property.
public class TeleportHandler : MonoBehaviour
{
    [Scene]
    [Tooltip("Assign the sub-scene to load for this zone")]
    public string subScene;

    public bool needParty;

    public int minLevel;

    [Tooltip("Instance template in the Scene. Don't use a prefab, Mirror can't handle prefabs that contain NetworkIdentity children.")]
    public Instance instanceTemplate;

    private NetworkManagerMMO manager;

    private bool initialUpdated = false;

    private void Start()
    {
        manager = GameObject.Find("NetworkManager").GetComponent<NetworkManagerMMO>();
    }
    [ServerCallback]
    void OnTriggerEnter(Collider co)
    {

        // a player might have a root collider and a hip collider.
        // only fire Portal OnTriggerEnter code here ONCE.
        // => only for the collider ON the player
        // => so we check GetComponent. DO NOT check GetComponentInParent.
        // Debug.Log($"Loading {subScene}");
        Player player = co.GetComponent<Player>();
        if(player != null)
        {
            // only call this for server and for local player. not for other
            // players on the client. no need in locally creating their
            // instances too.
            if (player.isServer || player.isLocalPlayer)
            {
                // required level?
                if (player.level.current >= minLevel)
                {
                    // can only enter with a party
                    if (!needParty || (needParty && player.party.InParty()))
                    {
                        // call OnPortal on server and on local client
                        OnTeleport(player);
                    }
                    // show info message on client directly. no need to do it via Rpc.
                    else if (player.isClient)
                        player.chat.AddMsgInfo("Can't enter instance without a party.");
                }
                // show info message on client directly. no need to do it via Rpc.
                else if (player.isClient)
                    player.chat.AddMsgInfo("Portal requires level " + minLevel);
            }
        }
        
    }


    void OnTeleport(Player player)
    {
        // check party again, just to be sure.
        if (!needParty || (needParty && player.party.InParty()))
        {
            // teleport player to instance entry
            //if (player.isServer) player.movement.Warp(existingInstance.entry.position);
            Debug.Log("Teleporting " + player.name);
            NetworkConnectionToClient conn = player.connectionToClient;
            manager.TeleportPlayer(conn, subScene);
        }
    }

    void OnPortalToInstance(Player player)
    {
        // check party again, just to be sure.
        if (player.party.InParty())
        {
            // is there an instance for the player's party yet?
            if (instanceTemplate.instances.TryGetValue(player.party.party.partyId, out Instance existingInstance))
            {
                // teleport player to instance entry
                if (player.isServer) player.movement.Warp(existingInstance.entry.position);
                Debug.Log("Teleporting " + player.name + " to existing instance=" + existingInstance.name + " with partyId=" + player.party.party.partyId);
            }
            // otherwise create a new one
            else
            {
                Instance instance = Instance.CreateInstance(instanceTemplate, player.party.party.partyId);
                if (instance != null)
                {
                    // teleport player to instance entry
                    if (player.isServer) player.movement.Warp(instance.entry.position);
                    Debug.Log("Teleporting " + player.name + " to new instance=" + instance.name + " with partyId=" + player.party.party.partyId);
                }
                else if (player.isServer) player.chat.TargetMsgInfo("There are already too many " + instanceTemplate.name + " instances. Please try again later.");
            }
        }
    }
}
