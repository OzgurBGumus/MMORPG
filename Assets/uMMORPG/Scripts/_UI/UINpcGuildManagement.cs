using UnityEngine;
using UnityEngine.UI;

public partial class UINpcGuildManagement : MonoBehaviour
{
    public static UINpcGuildManagement singleton;
    public GameObject panel;
    public Text createPriceText;
    public InputField createNameInput;
    public Button createButton;
    public Button terminateButton;

    private bool lastFrameWasInactive = true;

    public UINpcGuildManagement()
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
            player.target != null && player.target is Npc &&
            Utils.ClosestDistance(player, player.target) <= player.interactionRange)
        {
            FirstActiveFrame();
            createNameInput.interactable = !player.guild.InGuild() &&
                                           player.gold >= GuildSystem.CreationPrice;
            createNameInput.characterLimit = GuildSystem.NameMaxLength;

            createPriceText.text = GuildSystem.CreationPrice.ToString();

            createButton.interactable = !player.guild.InGuild() && GuildSystem.IsValidGuildName(createNameInput.text);
            createButton.onClick.SetListener(() =>
            {
                player.guild.CmdCreate(createNameInput.text);
                createNameInput.text = ""; // clear the input afterwards
            });

            terminateButton.interactable = player.guild.guild.CanTerminate(player.name);
            terminateButton.onClick.SetListener(() =>
            {
                player.guild.CmdTerminate();
            });
        }
        else
        {
            if (panel.activeSelf) FirstInActiveFrame();
        }
    }
    public void Toggle()
    {
        if (panel.activeSelf)
        {
            panel.SetActive(false);
        }
        else
        {
            panel.SetActive(true);
        }

    }
    public void Open()
    {
        Player.localPlayer.inventory.ItemUsingBlocked = true;
        FindObjectOfType<Canvas>().GetComponent<UIUniqueWindow>().CloseWindows();
        UIInventory.singleton.Open();
        panel.SetActive(true);
    }
    public void Close()
    {
        Player.localPlayer.inventory.ItemUsingBlocked = false;
        panel.SetActive(false);
    }
    public void FirstActiveFrame()
    {
        if (lastFrameWasInactive)
        {
            Open();
            lastFrameWasInactive = false;
        }
    }
    private void FirstInActiveFrame()
    {
        if (!lastFrameWasInactive)
        {
            Close();
            lastFrameWasInactive = true;
        }
    }
}
