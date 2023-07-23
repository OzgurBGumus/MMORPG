#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public abstract class ItemDropSetupBase : EditorWindow
{
    #region ScriptableObjects
    //[MenuItem("ItemDrop/Setup/New Page", false, 1)] // disabled by default for addon users
    public static void CreateNewPage()
    {
        ItemDropSetupPage asset = CreateInstance<ItemDropSetupPage>();

        //string uniqueFileName = AssetDatabase.GenerateUniqueAssetPath("Assets/uMMORPG/Scripts/Addons/ItemDrop/Resources/ItemDropSetup/Page 0.asset");
        string uniqueFileName = AssetDatabase.GenerateUniqueAssetPath("Assets/uMMORPG/Scripts/Addons/ItemDrop/ItemDropSetup/Page 0.asset");

        AssetDatabase.CreateAsset(asset, uniqueFileName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;

        EditorGUIUtility.PingObject(asset);
    }

    public static void CreateItemDropSettings()
    {
        ItemDropSettings asset = CreateInstance<ItemDropSettings>();
        asset.isInstalled = true;
        asset.DefaultSettings();

        string path = "Assets/uMMORPG/Scripts/Addons/ItemDrop/Resources/ItemDropSettings.asset";

        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;

        EditorGUIUtility.PingObject(asset);
    }    
    #endregion

    readonly string version = "v2.0.1";

    public ItemDropSetupPage scriptablePage;

    public int index = 0;

    float progressBar = 0;
    public string progressText = "";

    string pageTitle;
    public string description;

    bool groupEnabled;
    public bool showPath;
    string path = "";
    string customPath = "";

    string message = "";

    public bool showUninstallOption;
    public bool _uninstallation;

    public bool showButton = true;
    public string buttonName = "Next";

    public virtual void GetPage(ItemDropSetupPage page, bool changeProgress = true)
    {
        scriptablePage = page;
        pageTitle = page.title;
        description = page.description;
        customPath = page.path;

        if (changeProgress)
        {            
            index++;
            progressBar = GetProgress();

            //Debug.Log(index);
        }               
    }

    public bool Uninstallation
    {
        get => _uninstallation;
        set
        {
            if (value != _uninstallation)
            {
                _uninstallation = value;

                showButton = _uninstallation ? true : false;
            }
        }
    }

    public abstract string DefineSymbol { get; }
    string GetDefineSymbol() => $";{DefineSymbol}";

    string GetPath() => Path.Combine(customPath, scriptablePage.fileName);
    public abstract float GetProgress();

    public string StrikeThrough(string s)
    {
        string strikethrough = "";
        int n = 0;

        foreach (char c in s)
        {
            strikethrough += c;
            if (n > 1 && c != '\n')
            {
                strikethrough += '\u0336';
            }
            if (c == '\n')
            {
                n = 0;
            }
            else
            {
                n++;
            }
        }
        return strikethrough;
    }

    #region GUI
    void OnGUI()
    {
        EditorGUI.ProgressBar(new Rect(3, 3, position.width - 6, 20), progressBar, progressText); // progress bar

        EditorGUILayout.Space(30);

        EditorGUILayout.LabelField(pageTitle, EditorStyles.boldLabel); // title

        EditorGUILayout.Space();
               
        if (!showPath)
        {
            GUIStyle style = new GUIStyle(GUI.skin.label)
            {
                fontSize = 10,
                wordWrap = true,
                richText = true
            };

            EditorGUILayout.LabelField(description, style); // description

            if (showUninstallOption)
            {
                EditorGUILayout.Space(30);

                EditorGUILayout.BeginVertical();
                Uninstallation = EditorGUILayout.ToggleLeft("Remove all installed features", Uninstallation);

                if (_uninstallation)
                {
                    EditorGUILayout.LabelField("Press Next to continue uninstalling.", style);
                }
                EditorGUILayout.EndVertical();
            }
        }
        else
        {
            groupEnabled = EditorGUILayout.BeginToggleGroup("Path", groupEnabled);

            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            customPath = EditorGUILayout.TextField(customPath); // file path
            EditorGUILayout.LabelField(scriptablePage.fileName); // file name
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Default", GUILayout.Width(80)))
            {
                customPath = scriptablePage.path;
                EditorGUI.FocusTextInControl(customPath);
            }

            EditorGUILayout.EndToggleGroup();
        }

        GUILayout.FlexibleSpace();

        EditorGUILayout.LabelField(message, EditorStyles.centeredGreyMiniLabel); // message

        if (EditorApplication.isCompiling)
        {
            EditorGUILayout.LabelField("compiling...", EditorStyles.centeredGreyMiniLabel); // compiling
        }

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(version, EditorStyles.miniBoldLabel); // version

        GUILayout.FlexibleSpace();
        
        if (showButton)
        {
            if (GUILayout.Button(buttonName, GUILayout.Width(80)))
            {
                // preventing rapid clicks on a button
                if (!EditorApplication.isCompiling)
                {
                    EditorGUI.FocusTextInControl(customPath);
                    SwitchPage();
                }
            }
        }

        if (GUILayout.Button("Cancel", GUILayout.Width(80)))
        {
            Close();
        }

        EditorGUILayout.EndHorizontal();
    }
    #endregion

    public abstract void SwitchPage();

    #region Replace
    public string NormalizeLineEndings(string lines, string targetLineEnding = null)
    {
        if (string.IsNullOrEmpty(lines))
        {
            return lines;
        }

        if (targetLineEnding == null)
        {
            targetLineEnding = System.Environment.NewLine;
        }

        const string unixLineEnding = "\n";
        const string windowsLineEnding = "\r\n";
        const string macLineEnding = "\r";

        if (targetLineEnding != unixLineEnding && 
            targetLineEnding != windowsLineEnding &&
            targetLineEnding != macLineEnding)
        {
            Debug.LogWarning("Unknown target line ending character(s).");
        }

        lines = lines.Replace(windowsLineEnding, unixLineEnding).Replace(macLineEnding, unixLineEnding);

        if (targetLineEnding != unixLineEnding)
        {
            lines = lines.Replace(unixLineEnding, targetLineEnding);
        }

        return lines;
    }

    public bool ReplaceInFile(string filePath, ItemDropSetupPage.Text[] text)
    {
        MonoScript lScript = (MonoScript)AssetDatabase.LoadAssetAtPath(filePath, typeof(MonoScript));
        if (lScript != null)
        {
            string content = string.Empty;

            content = NormalizeLineEndings(lScript.text);

            for (int i = 0; i < text.Length; i++)
            {
                ItemDropSetupPage.Text index = text[i];
                bool installed = index.installed;
                
                if (Uninstallation)
                {
                    if (installed) // only if changes are already made
                    {
                        // undo changes
                        string searchText = NormalizeLineEndings(index.replace);
                        string replaceText = NormalizeLineEndings(index.search);

                        content = content.Replace(searchText, replaceText);
                        text[i].installed = false;
                    }
                }
                else
                {
                    if (!installed) // only if there are no code changes yet to avoid duplication
                    {
                        string searchText = NormalizeLineEndings(index.search);
                        string replaceText = NormalizeLineEndings(index.replace);

                        content = content.Replace(searchText, replaceText);
                        text[i].installed = true;
                    }
                }
            }
            EditorUtility.SetDirty(scriptablePage);

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.Write(NormalizeLineEndings(content));
                writer.Close();
            }
            return true;
        }
        return false;
    }
    #endregion

    #region Define Symbols
    public void AddDefineSymbol()
    {
        BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        string defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

        if (defineSymbols.Contains(DefineSymbol))
        {
            Debug.Log($"Selected build target ({EditorUserBuildSettings.activeBuildTarget.ToString()}) already contains {DefineSymbol} Scripting Define Symbol.");
            return;
        }
        PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, $"{defineSymbols}{GetDefineSymbol()}");
    }

    public void RemoveDefineSymbol()
    {
        BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        string defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

        if (defineSymbols.Contains(GetDefineSymbol()))
        {
            string defaultSymbols = defineSymbols.Replace(GetDefineSymbol(), "");
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defaultSymbols);
        }
    }
    #endregion

    #region Tooltip
    public bool AddTooltip()
    {
        GameObject tooltipPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
        if (tooltipPrefab != null)
        {
            foreach (string guid in AssetDatabase.FindAssets("t:Object", new[] { customPath }))
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                UIChatEntry asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UIChatEntry)) as UIChatEntry;
                if (asset != null)
                {
                    if (!asset.TryGetComponent(out UIShowToolTip component))
                    {
                        UIShowToolTip tooltip = asset.gameObject.AddComponent<UIShowToolTip>();

                        tooltip.enabled = false;
                        tooltip.tooltipPrefab = tooltipPrefab;
                        //Debug.Log(asset);
                    }
                }
            }
            return true;
        }
        return false;
    }

    public bool RemoveTooltip()
    {
        GameObject tooltipPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
        if (tooltipPrefab != null)
        {
            foreach (string guid in AssetDatabase.FindAssets("t:Object", new[] { customPath }))
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                UIChatEntry asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UIChatEntry)) as UIChatEntry;
                if (asset != null)
                {
                    GameObject root = PrefabUtility.LoadPrefabContents(assetPath);
                    if (root.TryGetComponent(out UIShowToolTip component))
                    {
                        DestroyImmediate(component, true);
                        PrefabUtility.SaveAsPrefabAsset(root, assetPath);
                        PrefabUtility.UnloadPrefabContents(root);
                    }
                }
            }
            return true;
        }
        return false;
    }
    #endregion

    #region Tag
    public bool AddTag(string tagName)
    {
        Object asset = AssetDatabase.LoadMainAssetAtPath("ProjectSettings/TagManager.asset");
        if (asset != null)
        {
            SerializedObject tagManager = new SerializedObject(asset);
            SerializedProperty tags = tagManager.FindProperty("tags");

            int numTags = tags.arraySize;

            for (int i = 0; i < numTags; i++)
            {
                SerializedProperty existingTag = tags.GetArrayElementAtIndex(i);
                if (existingTag.stringValue.Equals(tagName))
                {
                    Debug.Log($"Tag: <color=blue>{tagName}</color> already exists.");
                    return true;
                }
            }

            // create a new tag
            tags.InsertArrayElementAtIndex(numTags);
            tags.GetArrayElementAtIndex(numTags).stringValue = tagName;
            tagManager.ApplyModifiedProperties();
            //tagManager.Update();
            Debug.Log($"Tag: <color=blue>{tagName}</color> has been added.");
            return true;
        }
        return false;
    }

    public bool RemoveTag(string tagName)
    {
        Object asset = AssetDatabase.LoadMainAssetAtPath("ProjectSettings/TagManager.asset");
        if (asset != null)
        {
            SerializedObject tagManager = new SerializedObject(asset);
            SerializedProperty tags = tagManager.FindProperty("tags");

            int numTags = tags.arraySize;

            for (int i = 0; i < numTags; i++)
            {
                SerializedProperty existingTag = tags.GetArrayElementAtIndex(i);
                if (existingTag.stringValue.Equals(tagName))
                {
                    tags.DeleteArrayElementAtIndex(i);
                    tagManager.ApplyModifiedProperties();
                    Debug.Log($"Tag: <color=blue>{tagName}</color> has been removed.");
                    return true;
                }
            }
            Debug.Log($"Tag: <color=blue>{tagName}</color> is already removed.");
            return true;
        }
        return false;
    }
    #endregion

    #region Layer
    public bool AddLayer(string layerName)
    {
        Object asset = AssetDatabase.LoadMainAssetAtPath("ProjectSettings/TagManager.asset");
        if (asset != null)
        {
            SerializedObject tagManager = new SerializedObject(asset);
            SerializedProperty layers = tagManager.FindProperty("layers");

            int numLayers = layers.arraySize;

            for (int i = 8, j = numLayers; i < j; i++) // the first 8 of these Layers are specified by Unity
            {
                SerializedProperty existingLayer = layers.GetArrayElementAtIndex(i);
                if (existingLayer.stringValue.Equals(layerName))
                {
                    Debug.Log($"Layer: <color=blue>{layerName}</color> already exists.");
                    return true;
                }
                else if (existingLayer.stringValue == "")
                {
                    // create a new layer
                    existingLayer.stringValue = layerName;
                    tagManager.ApplyModifiedProperties();
                    //tagManager.Update();
                    Debug.Log($"Layer: <color=blue>{layerName}</color> has been added.");
                    return true;
                }
                if (i == j) Debug.Log("All allowed layers have been filled.");
            }
        }
        return false;
    }

    public bool RemoveLayer(string layerName)
    {
        Object asset = AssetDatabase.LoadMainAssetAtPath("ProjectSettings/TagManager.asset");
        if (asset != null)
        {
            SerializedObject tagManager = new SerializedObject(asset);
            SerializedProperty layers = tagManager.FindProperty("layers");

            int numLayers = layers.arraySize;

            for (int i = 8, j = numLayers; i < j; i++) // the first 8 of these Layers are specified by Unity
            {
                SerializedProperty existingLayer = layers.GetArrayElementAtIndex(i);
                if (existingLayer.stringValue.Equals(layerName))
                {
                    existingLayer.stringValue = "";
                    tagManager.ApplyModifiedProperties();
                    Debug.Log($"Layer: <color=blue>{layerName}</color> has been removed.");
                    return true;
                }
            }
            Debug.Log($"Layer: <color=blue>{layerName}</color> is already removed.");
            return true;
        }
        return false;
    }
    #endregion

    public void AddItemDropSettings(bool classic = true)
    {
        ItemDropSettings asset = ItemDropSettings.Settings;
        if (asset != null)
        {
            asset.isInstalled = true;
            asset.DefaultSettings(classic);
            EditorUtility.SetDirty(asset);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
            EditorGUIUtility.PingObject(asset);
        }
        else
        {
            CreateItemDropSettings();
        }
    }

    public void FirstPage(ItemDropSetupPage page, bool backward = false)
    {
        progressText = "";
        showUninstallOption = false;

        GetPage(page);

        if (backward)
        {
            message = "";
            description = StrikeThrough(description);
            index = 1;
        }
        else
        {
            showPath = true;
        }
    }

    public void Next(ItemDropSetupPage page, bool replace = true)
    {
        path = GetPath();

        if (replace)
        {
            if (ReplaceInFile(path, scriptablePage.text))
            {
                if (_uninstallation)
                {
                    //Debug.Log($"{pageTitle} removed! {path}");
                    message = $"{pageTitle} removed! {path}";
                }
                else
                {
                    //Debug.Log($"{pageTitle} completed! {path}");
                    message = $"{pageTitle} completed! {path}";
                }

                GetPage(page);
            }
            else
            {
                //Debug.LogWarning("Cannot find the file specified.");
                message = "Cannot find the file specified.";
            }
        }
        else
        {
            if (_uninstallation)
            {
                if (RemoveTooltip())
                {
                    //Debug.Log($"{pageTitle} removed! {path}");
                    message = $"{pageTitle} removed! {path}";

                    GetPage(page);
                }
                else
                {
                    //Debug.LogWarning("Cannot find the file specified.");
                    message = "Cannot find the file specified.";
                }
            }
            else
            {
                if (AddTooltip())
                {
                    showPath = false;

                    //Debug.Log($"{pageTitle} completed! {path}");
                    message = $"{pageTitle} completed! {path}";

                    GetPage(page);
                }
                else
                {
                    //Debug.LogWarning("Cannot find the file specified.");
                    message = "Cannot find the file specified.";
                }
            }
        }      
    }

    public void FinishInstallation(ItemDropSetupPage page)
    {
        string tag = "Item";
        string layer = "ItemLabel";
        string layerItemPoint = "ItemPoint";
        GameObject itemPrefab = ItemDropSettings.Settings.itemPrefab;
        if (itemPrefab != null)
        {
            if (AddTag(tag))
            {
                // assign tag
                if (itemPrefab.tag != tag)
                {
                    itemPrefab.tag = tag;
                    Debug.Log($"Item.prefab Tag: <color=blue>{tag}</color> has been assigned.");
                }
            }

            if (AddLayer(tag))
            {
                // assign layer
                int index = LayerMask.NameToLayer(tag);
                if (itemPrefab.layer != index)
                {
                    itemPrefab.layer = index;
                    Debug.Log($"Item.prefab Layer: <color=blue>{tag}</color> has been assigned.");
                }
            }

            if (AddLayer(layerItemPoint))
            {
                GameObject itemPoint = itemPrefab.transform.GetChild(0).gameObject;
                if (itemPoint != null)
                {
                    // assign layer
                    int index = LayerMask.NameToLayer(layerItemPoint);
                    if (itemPoint.layer != index)
                    {
                        itemPoint.layer = index;
                        Debug.Log($"Item(ItemPoint).prefab Layer: <color=blue>{layerItemPoint}</color> has been assigned.");
                    }
                }
            }
        }

        GameObject itemPart = ItemDropSettings.Settings.itemPart;
        if (itemPart != null)
        {
            // assign layer
            int index = LayerMask.NameToLayer(tag);
            if (itemPart.layer != index)
            {
                itemPart.layer = index;
                Debug.Log($"ItemPart.prefab Layer: <color=blue>{tag}</color> has been assigned.");
            }          
        }

        GameObject itemLabel = ItemDropSettings.Settings.itemLabel;
        if (itemLabel != null)
        {
            if (AddLayer(layer))
            {
                // assign layer
                int index = LayerMask.NameToLayer(layer);
                if (itemLabel.layer != index)
                {
                    itemLabel.layer = index;
                    foreach (Transform child in itemLabel.transform)
                    {
                        child.gameObject.layer = index;
                    }
                    Debug.Log($"ItemLabel.prefab Layer: <color=blue>{layer}</color> has been assigned.");
                }
            }
        }

        LootManager lootManager = FindObjectOfType<LootManager>();
        if (!lootManager)
        {
            GameObject prefab = ItemDropSettings.Settings.lootManager;
            if (prefab != null)
            {
                PrefabUtility.InstantiatePrefab(prefab);
            }
        }

        UILoot loot = FindObjectOfType<UILoot>();
        if (loot != null)
        {
            loot.enabled = false;
        }

        NetworkManagerMMO manager = FindObjectOfType<NetworkManagerMMO>();
        if (manager != null)
        {
            if (!manager.spawnPrefabs.Contains(itemPrefab))
            {
                manager.spawnPrefabs.Add(itemPrefab);
            }            
        }
       
        // turn on
        Camera mainCamera = Camera.main;
        mainCamera.cullingMask |= 1 << LayerMask.NameToLayer(tag);

        // turn off
        mainCamera.cullingMask &= ~(1 << LayerMask.NameToLayer(layer));

        // turn off
        GameObject minimapCamera = GameObject.FindWithTag("MinimapCamera");
        if (minimapCamera != null)
        {
            if (minimapCamera.TryGetComponent(out Camera camera))
            {
                camera.cullingMask &= ~(1 << LayerMask.NameToLayer(tag));
                camera.cullingMask &= ~(1 << LayerMask.NameToLayer(layer));
                camera.cullingMask &= ~(1 << LayerMask.NameToLayer(layerItemPoint));
            }
        }

        AddDefineSymbol();

        EditorSceneManager.MarkSceneDirty(mainCamera.gameObject.scene);
        EditorSceneManager.SaveScene(mainCamera.gameObject.scene);        

        //Debug.Log($"{pageTitle} completed! The installation was successful.");
        message = $"{pageTitle} completed! The installation was successful.";
        GetPage(page);
    }  
    
    public void RevertInstallation(ItemDropSetupPage page)
    {
        string tag = "Item";
        string layer = "ItemLabel";
        string layerItemPoint = "ItemPoint";
        GameObject itemPrefab = ItemDropSettings.Settings.itemPrefab;
        if (itemPrefab != null)
        {
            if (RemoveTag(tag))
            {
                itemPrefab.tag = "Untagged";
            }

            if (RemoveLayer(tag))
            {
                itemPrefab.layer = 0;
            }

            GameObject itemPoint = itemPrefab.transform.GetChild(0).gameObject;
            if (itemPoint != null)
            {
                if (RemoveLayer(layerItemPoint))
                {
                    itemPoint.layer = 0;
                }
            }
        }
        
        GameObject itemPart = ItemDropSettings.Settings.itemPart;
        if (itemPart != null)
        {
            itemPart.layer = 0;
        }
        
        GameObject itemLabel = ItemDropSettings.Settings.itemLabel;
        if (itemLabel != null)
        {
            if (RemoveLayer(layer))
            {
                itemLabel.layer = 0;
                foreach (Transform child in itemLabel.transform)
                {
                    child.gameObject.layer = 0;
                }
            }
        }

        LootManager lootManager = FindObjectOfType<LootManager>();
        if (lootManager != null)
        {
            DestroyImmediate(lootManager.gameObject, true);
        }

        UILoot loot = FindObjectOfType<UILoot>();
        if (loot != null)
        {
            loot.enabled = true;
        }

        NetworkManagerMMO manager = FindObjectOfType<NetworkManagerMMO>();
        if (manager != null)
        {
            if (manager.spawnPrefabs.Contains(itemPrefab))
            {
                manager.spawnPrefabs.Remove(itemPrefab);
            }

            EditorSceneManager.MarkSceneDirty(manager.gameObject.scene);
            EditorSceneManager.SaveScene(manager.gameObject.scene);
        }

        RemoveDefineSymbol();

        showPath = true;

        //Debug.Log($"{pageTitle} removed! Press Next to continue uninstalling.");
        message = $"{pageTitle} removed! Press Next to continue uninstalling.";
        GetPage(page);
    }
}
#endif