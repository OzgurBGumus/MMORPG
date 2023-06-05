// Simple character selection list. The charcter prefabs are known, so we could
// easily show 3D models, stats, etc. too.
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public partial class UICharacterSelection : MonoBehaviour
{
    public UICharacterCreation uiCharacterCreation;
    public UIConfirmation uiConfirmation;
    public NetworkManagerMMO manager; // singleton is null until update
    public GameObject panel;
    public Button startButton;
    public Button deleteButton;
    public Button createButton;
    public Button quitButton;

    private void Start()
    {
        manager = GameObject.Find("NetworkManager").GetComponent<NetworkManagerMMO>();
        auth = GameObject.Find("NetworkManager").GetComponent<NetworkAuthenticatorMMO>();
    }
    void Update()
    {
        // show while in lobby and while not creating a character
        if (CheckIsVisible())
        {
            panel.SetActive(true);
            // characters available message received already?
            if (manager.charactersAvailableMsg.characters != null)
            {
                Camera.main.transform.position = manager.selectionCameraLocation.position;
                CharactersAvailableMsg.CharacterPreview[] characters = manager.charactersAvailableMsg.characters;

                // start button: calls AddPLayer which calls OnServerAddPlayer
                // -> button sends a request to the server
                // -> if we press button again while request hasn't finished
                //    then we will get the error:
                //    'ClientScene::AddPlayer: playerControllerId of 0 already in use.'
                //    which will happen sometimes at low-fps or high-latency
                // -> internally ClientScene.AddPlayer adds to localPlayers
                //    immediately, so let's check that first
                startButton.gameObject.SetActive(manager.selection != -1);
                startButton.onClick.SetListener(() => {
                    // set client "ready". we will receive world messages from
                    // monsters etc. then.
                    NetworkClient.Ready();

                    // send CharacterSelect message (need to be ready first!)
                    NetworkClient.Send(new CharacterSelectMsg{ index=manager.selection });

                    // clear character selection previews
                    manager.ClearPreviews();

                    // make sure we can't select twice and call AddPlayer twice
                    panel.SetActive(false);
                });

                // delete button
                deleteButton.gameObject.SetActive(manager.selection != -1);
                deleteButton.onClick.SetListener(() => {
                    uiConfirmation.Show(
                        "Do you really want to delete <b>" + characters[manager.selection].name + "</b>?",
                        () => { NetworkClient.Send(new CharacterDeleteMsg{ index=manager.selection }); }
                    );
                });

                // create button
                createButton.interactable = characters.Length < manager.characterLimit;
                createButton.onClick.SetListener(() => {
                    panel.SetActive(false);
                    uiCharacterCreation.Show();
                });

                // quit button
                quitButton.onClick.SetListener(() => { NetworkManagerMMO.Quit(); });
                Utils.InvokeMany(typeof(UICharacterSelection), this, "Update_", characters);
            }
        }
        else panel.SetActive(false);
    }
}
