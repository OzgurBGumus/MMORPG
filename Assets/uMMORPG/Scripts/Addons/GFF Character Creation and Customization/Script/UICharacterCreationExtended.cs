using UnityEngine;
using UnityEngine.UI;
using Mirror;

public partial class UICharacterCreationExtended : MonoBehaviour
{
    [Header("Components")]
    public NetworkManagerMMO manager; // singleton is null until update
    public CharcterCreationComponents positions;

    [Header("Settings")]
    public ScriptableRacesData raceData;
    public bool usePanelStats = true;
    public bool useGameMasterAddon = false; //for show/hide GM toggle 

    [Header("Main Panels")]
    public GameObject panel;
    public GameObject panelCreation;
    public GameObject panelCustomization;

    [Header("Create Character right")]
    public Dropdown raceDropdown;
    public Dropdown classDropdown;
    public Dropdown genderDropdown;
    public InputField nameInput;
    public Button cancelIButton;
    public Button createButton;
    public Button customizationButton;
    public Toggle gameMasterToggle;

    [Header("Create Character left")]
    public GameObject panelStats;
    public Slider HP;
    public Slider FP;
    public Slider SP;
    public Slider Damage;
    public Slider Defense;
    public Text textWeapon;
    public Text textArmor;

    [Header("Customization")]
    public Transform content;
    public GameObject prefab;
    public Button buttonEquip;
    public Button buttonRandomize;
    public Button buttonCustomizationSave;

    [Header("Customization Scale")]
    public GameObject panelScale;
    public Slider scale;

    private GameObject playerPreview;
    public static bool customizationInGame = false;
    private bool isEquip;

    public bool IsVisible() { return panel.activeSelf; }

    private void Start()
    {
        manager = GameObject.Find("NetworkManager").GetComponent<NetworkManagerMMO>();
        LoadRaces();
        LoadClasses();
        LoadGender();
        genderDropdown.onValueChanged.AddListener(delegate { InstantiatePrefab(); });
        classDropdown.onValueChanged.AddListener(delegate { ChangeClasses(); });
        raceDropdown.onValueChanged.AddListener(delegate { ChangeRace(); });
    }

    public void Show()
    {
        panel.SetActive(true);

        // setup camera
        Camera.main.transform.position = positions.cameraCustomizationPosition.position;
        Camera.main.transform.rotation = positions.cameraCustomizationPosition.rotation;

        InstantiatePrefab();
    }

    void Update()
    {
        // only update while visible (after character selection made it visible)
        if (panel.activeSelf)
        {
            // still in lobby?
            if (manager.state == NetworkState.Lobby || manager.state == NetworkState.World)
            {
                // only show GameMaster option for host connection
                // -> this helps to test and create GameMasters more easily
                // -> use the database field for dedicated servers!
                gameMasterToggle.gameObject.SetActive(NetworkServer.localClientActive && useGameMasterAddon);

                //button cancel
                cancelIButton.gameObject.SetActive(!panelCustomization.activeSelf);
                cancelIButton.onClick.SetListener(() => {PressButtonCancel();});

                //button create
                createButton.interactable = manager.IsAllowedCharacterName(nameInput.text);
                createButton.onClick.SetListener(() => {
                    SendMessageToServerCreateCharacter();
                    panel.SetActive(false);

                    Destroy(playerPreview);
                    nameInput.text = "";
                });

                //button customization
                customizationButton.gameObject.SetActive(playerPreview != null && playerPreview.GetComponent<PlayerCustomization>().GetItemTypesForCharacterCreate().Length > 0);
                customizationButton.onClick.SetListener(() => {PressButtonCustomization();});


                //button equip
                if (isEquip) buttonEquip.GetComponentInChildren<Text>().text = "Non Equip";
                else buttonEquip.GetComponentInChildren<Text>().text = "Equip";
                buttonEquip.onClick.SetListener(() => {
                    isEquip = !isEquip;
                    EquipCharacter();
                });

                buttonRandomize.gameObject.SetActive(panelCustomization.activeSelf);
                buttonCustomizationSave.gameObject.SetActive(panelCustomization.activeSelf);

                if (panelCustomization.activeSelf)
                {
                    PlayerCustomization customization = playerPreview.GetComponent<PlayerCustomization>();

                    Customization[] types = customization.GetItemTypesForCharacterCreate();
                    UIUtils.BalancePrefabs(prefab.gameObject, types.Length + 1, content);

                    for (int i = 0; i < types.Length; i++)
                    {
                        UICustomizationSlot slot = content.GetChild(i+1).GetComponent<UICustomizationSlot>();

                        slot.text.text = types[i].type.ToString();
                        if (types[i].objects != null && types[i].objects.Length > 0)
                           slot.slider.maxValue = types[i].objects.Length - 1;
                        else slot.slider.maxValue = types[i].material.meshes.Length - 1;

                        int icopy = i;
                        slot.slider.onValueChanged.SetListener(delegate { customization.SetCustomizationLocalByType(types[icopy].type, (int)slot.slider.value); });
                        slot.buttonLeft.onClick.SetListener(() => {
                            if (slot.slider.value > 0) slot.slider.value --;
                        });
                        slot.buttonRight.onClick.SetListener(() => {
                            if (slot.slider.value < slot.slider.maxValue) slot.slider.value ++;
                        });
                    }

                    buttonRandomize.onClick.SetListener(() => {

                        for (int i = 0; i < customization.customization.Length; i++)
                        {
                            if (customization.customization[i].objects == null || customization.customization[i].objects.Length == 0)
                            {
                                customization.values[i] = UnityEngine.Random.Range(0, customization.customization[i].material.meshes.Length);

                                UICustomizationSlot slot = content.GetChild(i + 1).GetComponent<UICustomizationSlot>();
                                slot.slider.onValueChanged.RemoveAllListeners();
                                slot.slider.value = customization.values[i];
                            }
                        }

                        //scale
                        if (customization.rescaling)
                        {
                            customization.scale = UnityEngine.Random.Range(customization.scaleMin, customization.scaleMax);
                            scale.value = customization.scale;
                        }

                        customization.SetCustomization();
                    });
                    buttonCustomizationSave.onClick.SetListener(() => { PressButtonCustomizationSave(); });

                    if (customization.rescaling)
                    {
                        panelScale.SetActive(true);
                        scale.minValue = customization.scaleMin;
                        scale.maxValue = customization.scaleMax;
                        scale.value = playerPreview.transform.localScale.x;

                        scale.onValueChanged.AddListener(delegate {
                            customization.scale = scale.value;
                            playerPreview.transform.localScale = new Vector3(scale.value, scale.value, scale.value);
                        });
                    }
                    else panelScale.SetActive(false);
                }
            }
            else panel.SetActive(false);
        }
        else panel.SetActive(false);
    }

    public void InstantiatePrefab()
    {
        Destroy(playerPreview);
        GameObject go = null;

        // find the prefab for required class
        if (!customizationInGame) go = manager.playerClasses.Find(p => p.name == classDropdown.captionText.text).gameObject;
        else if (Player.localPlayer != null) go = manager.playerClasses.Find(p => p.name == Player.localPlayer.className).gameObject;

        // instantiate the prefab
        if (go != null)
        {
            playerPreview = Instantiate(go, positions.characterPosition.position, positions.characterPosition.rotation);

            playerPreview.GetComponent<Player>().name = "";
            PlayerCustomization customization = playerPreview.GetComponent<PlayerCustomization>();

            if (!customizationInGame)
            {
                //show panel stats
                panelStats.SetActive(usePanelStats);
                if (panelStats.activeSelf)
                {
                    Classes character = raceData.races[raceDropdown.value].classes[classDropdown.value];

                    HP.value = character.hp;
                    FP.value = character.fp;
                    SP.value = character.sp;

                    Damage.value = character.damage;
                    Defense.value = character.defense;

                    textWeapon.text = character.weapon;
                    textArmor.text = character.armor;
                }
            }
            else
            {

            }

            for (int i = 0; i < customization.customization.Length ; i++)
            {
                if (customization.customization[i].showWhenCharactercCreate) customization.values.Add(0);
                else customization.values.Add(-1);
            }

            isEquip = true;
            EquipCharacter();
        }
        else
        {
            Debug.LogError("Class prefab not found");
        }
    }

    void LoadRaces()
    {
        for (int i = 0; i < raceData.races.Length; i++)
        {
            Dropdown.OptionData index = new Dropdown.OptionData();
            index.text = raceData.races[i].name;
            raceDropdown.options.Add(index);
        }
    }
    void LoadClasses()
    {
        classDropdown.ClearOptions();
        for (int i = 0; i < raceData.races[raceDropdown.value].classes.Count; i++)
        {
            Dropdown.OptionData index = new Dropdown.OptionData();
            index.text = raceData.races[raceDropdown.value].classes[i].name;
            classDropdown.options.Add(index);
        }
        classDropdown.value = 0;
        if (classDropdown.options.Count > 0) classDropdown.captionText.text = classDropdown.options[0].text;
    }
    void LoadGender()
    {
        genderDropdown.ClearOptions();
        if (raceData.races[raceDropdown.value].classes[classDropdown.value].men)
        {
            Dropdown.OptionData index = new Dropdown.OptionData();
            index.text = "Men";
            genderDropdown.options.Add(index);
        }
        if (raceData.races[raceDropdown.value].classes[classDropdown.value].girl)
        {
            Dropdown.OptionData index = new Dropdown.OptionData();
            index.text = "Women";
            genderDropdown.options.Add(index);
        }
        genderDropdown.value = 0;
        if (genderDropdown.options.Count > 0) genderDropdown.captionText.text = genderDropdown.options[0].text;
    }

    void ChangeRace()
    {
        LoadClasses();
        InstantiatePrefab();
    }
    void ChangeClasses()
    {
        LoadGender();
        InstantiatePrefab();
    }

    //Buttons in Create panel
    void PressButtonCustomization()
    {
        panelCreation.SetActive(false);
        panelCustomization.SetActive(true);

        Camera.main.transform.position = positions.cameraFaceCustomizationPosition.position;
    }
    void PressButtonCancel()
    {
        Camera.main.transform.position = manager.selectionCameraLocation.position;
        Camera.main.transform.rotation = manager.selectionCameraLocation.rotation;

        Destroy(playerPreview);
        nameInput.text = "";
        panel.SetActive(false);
    }

    //Buttons in Customization panel
    void EquipCharacter()
    {
        Player player = playerPreview.GetComponent<Player>();

        if (isEquip)
        {
            for (int i = 0; i < ((PlayerEquipment)player.equipment).slotInfo.Length; i++)
            {
                if (((PlayerEquipment)player.equipment).slotInfo[i].defaultItem.item != null && ((PlayerEquipment)player.equipment).slotInfo[i].defaultItem.item is EquipmentItem eitem)
                {
                    // has a model? then set it
                    if (eitem.modelPrefab != null)
                    {
                        // load the model and parent to info.location
                        GameObject go = Instantiate(eitem.modelPrefab, ((PlayerEquipment)player.equipment).slotInfo[i].location, false);

                        // skinned mesh and all bones can be be replaced?
                        // then replace all. this way the equipment can follow IK
                        // too (if any).
                        // => this is the RECOMMENDED method for animated equipment.
                        //    name all equipment bones the same as player bones and
                        //    everything will work perfectly
                        // => this is the ONLY way for equipment to follow IK, e.g.
                        //    in games where arms aim up/down.
                        // NOTE: uMMORPG doesn't use IK at the moment, but it might
                        //       need this later.
                        SkinnedMeshRenderer equipmentSkin = go.GetComponentInChildren<SkinnedMeshRenderer>();
                        if (equipmentSkin != null && ((PlayerEquipment)player.equipment).CanReplaceAllBones(equipmentSkin))
                            ((PlayerEquipment)player.equipment).ReplaceAllBones(equipmentSkin);

                        // animator? then replace controller to follow player's
                        // animations
                        // => this is the ALTERNATIVE method for animated equipment.
                        //    add the Animator and use the player's avatar. works
                        //    for animated pants, etc. but not for IK.
                        // => this is NECESSARY for 'external' equipment like wings,
                        //    staffs, etc. that should be animated but don't contain
                        //    the same bones as the player.
                        Animator anim = go.GetComponent<Animator>();
                        if (anim != null)
                        {
                            // assign main animation controller to it
                            anim.runtimeAnimatorController = player.animator.runtimeAnimatorController;
                        }
                        else
                        {
                            Debug.Log("for item " + eitem.name + " anim not found");
                        }
                    }
                }
            }

            // restart all animators, so that skinned mesh equipment will be in sync with the main animation
            ((PlayerEquipment)player.equipment).RebindAnimators();
        }
        else
        {
            for (int i = 0; i < ((PlayerEquipment)player.equipment).slotInfo.Length; i++)
                if (((PlayerEquipment)player.equipment).slotInfo[i].location != null && ((PlayerEquipment)player.equipment).slotInfo[i].location.childCount > 0)
                    Destroy(((PlayerEquipment)player.equipment).slotInfo[i].location.GetChild(0).gameObject);
        }
    }
    void PressButtonCustomizationSave()
    {
        panelCreation.SetActive(true);
        panelCustomization.SetActive(false);

        if (!customizationInGame)
        {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, positions.cameraPosition.position, Time.deltaTime * 1.0f);
        }
        else
        {
            Player player = Player.localPlayer;
            if (player != null)
            {
                /*player.CmdSetCustomization(preview.transform.localScale.x,
                    preview.transform.localScale.y,
                    preview.transform.localScale.z,
                    skinColorId,
                    hairColor,
                    browColor,
                    eyeColor,
                    clothing);
                player.customization.SetCustomization();*/
            }
            panel.SetActive(false);
            customizationInGame = false;
        }
    }

    //message
    void SendMessageToServerCreateCharacter()
    {
        string _values = "";
        for (int i = 0; i < playerPreview.GetComponent<PlayerCustomization>().values.Count; i++)
        {
            _values += playerPreview.GetComponent<PlayerCustomization>().values[i] + ";";
        }

        //Character has been created successfully
        CharacterCreateMsg message = new CharacterCreateMsg
        {
            name = nameInput.text,
            classIndex = manager.playerClasses.FindIndex(x => x.name == classDropdown.captionText.text),
            gameMaster = gameMasterToggle.isOn,

            race = (RaceList)raceDropdown.value + 1,
            gender = genderDropdown.captionText.text,

            customization = _values,
            scale = playerPreview.GetComponent<PlayerCustomization>().scale
        };
        NetworkClient.Send(message);
    }
}
