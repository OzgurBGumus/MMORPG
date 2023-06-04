using Mirror;
using UnityEngine;


public partial class UsableItem
{
    [Header("GFF Races and classes")]
    public RaceList race;
    public int clases;
}

public partial class Player
{
    [Header("GFF Races and classes")]
    public ScriptableRacesData raceData;
    [SyncVar] public RaceList race;
    [SyncVar] public string gender;
    [SyncVar] public string specialisation_1 = "";
    [SyncVar] public string specialisation_2 = "";


    /*public void UpdateOverlays_RaceColor()
    {
        if (localPlayer != null)
        {
            // note: murderer has higher priority (a player can be a murderer and an offender at the same time)
            if (IsMurderer()) nameOverlay.color = nameOverlayMurdererColor;
            else if (IsOffender()) nameOverlay.color = nameOverlayOffenderColor;
            // member of the same party
            else if (localPlayer.party.InParty() && localPlayer.party.party.Contains(name)) nameOverlay.color = nameOverlayPartyColor;
            // otherwise default
            else
            {
                //if used "Race, Class, Gender, Specialization" addon
                if (UICharacterInfoExtended.singleton.useCharacterCreateAddon) nameOverlay.color = RaceColor();
                else
                    nameOverlay.color = nameOverlayDefaultColor;
            }
        }
    }*/
}

/*public partial class UICharacterInfoExtended
{
    void Update_RaceClassGender(Player player)
    {
        if (useRaceClassGenderAddon)
        {
            raceTextValue.text = player.race.ToString();
            classTextValue.text = player.className;
            genderTextValue.text = player.gender;
            if (player.specialisation_1 == "") specializationTextValue.text = "";
        }
    }
}*/