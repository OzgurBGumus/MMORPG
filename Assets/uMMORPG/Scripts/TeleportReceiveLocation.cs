using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
public class TeleportReceiveLocation : MonoBehaviour
{
    [Scene]
    public string currentScene = "";
    private NetworkManagerMMO manager;
    private bool teleported = false;
    // Start is called before the first frame update
    void Start()
    {

    }
    private void Update()
    {
        //if (!teleported)
        //{
        //    Player player = Player.localPlayer;
        //    manager = GameObject.Find("NetworkManager").GetComponent<NetworkManagerMMO>();
        //    TeleportPoint tpp = manager.GetTeleportPoint(currentScene);
        //    // teleport player to instance entry
        //    Vector3 tpLocation = new Vector3(tpp.x, tpp.y, tpp.z);
        //    player.movement.Warp(tpLocation);
        //    teleported = true;
        //}
    }


}
