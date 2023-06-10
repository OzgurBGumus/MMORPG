using UnityEngine;
using UnityEngine.UI;

public partial class UniversalSlot : MonoBehaviour {

    public UIShowToolTip tooltip;
    public Button button;
    public UIDragAndDropable dragAndDropable;
    public Image image;
    public Image cooldownCircle;
    public GameObject amountOverlay;
    public Text amountText;

    [Header("Rarity Addon")]
    public Image rarityImage;

    [Header("Item Enchantment Addon")]
    public Text upgradeText;

    [Header("only for Equipment")]
    public GameObject categoryOverlay;
    public Text categoryText;

    [Header("only for Skillbar")]
    public Text hotkeyText;

    [Header("only for skills")]
    public GameObject cooldownOverlay;
    public Text cooldownText;

    [Header("only for loot")]
    public Text nameText;
}
