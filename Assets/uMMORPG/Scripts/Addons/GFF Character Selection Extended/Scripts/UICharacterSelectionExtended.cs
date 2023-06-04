using UnityEngine;
using UnityEngine.UI;

public partial class UICharacterSelection
{

    [Header("Components")]
    public NetworkAuthenticatorMMO auth;
    public UICharacterCreationExtended characterCreationExtended;

    [Header("Settings")]
    public bool useStats;
    public bool useGameControl;
    public bool useGameControlPremiumService;

    [Header("Stats")]
    public GameObject panelStats;
    public Text textNameValue;
    public Text textRace;
    public Text textRaceValue;
    public Text textClassValue;
    public Text textSpecialization;
    public Text textSpecializationValue;
    public Text textGender;
    public Text textGenderValue;
    public Text textLevelValue;
    public Text textGoldValue;
    public Text textLocationValue;

    [Header("GFF GameControl Panel Addon")]
    public GameObject panelPremium;
}
