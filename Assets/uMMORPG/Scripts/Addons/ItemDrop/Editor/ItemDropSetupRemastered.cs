#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public sealed class ItemDropSetupRemastered : ItemDropSetupBase
{
    [MenuItem("ItemDrop/Setup/Install: Remastered")]
    static void Init()
    {
        // do not open existing window again
        if (HasOpenInstances<ItemDropSetupBase>())
            return;

        if (ItemDropSetupPage.RemasteredAll.Length == 17)
        {
            // make a new window
            ItemDropSetupRemastered window = (ItemDropSetupRemastered)GetWindowWithRect(typeof(ItemDropSetupRemastered), new Rect(0, 0, 470, 210));
            window.Show();

            window.GetPage(ItemDropSetupPage.RemasteredAll[0], false);

            if (ItemDropSettings.Settings != null)
            {
                if (ItemDropSettings.Settings.isInstalled)
                {
                    window.description = "Item Drop addon is already installed.";
                    window.showUninstallOption = true;
                    window.showButton = false;
                }
            }
        }
    }

    public override string DefineSymbol => "ITEM_DROP_R";
    public override float GetProgress() => (float)(index - 1) / 14;

    public override void GetPage(ItemDropSetupPage page, bool changeProgress = true)
    {
        base.GetPage(page, changeProgress);

        if (index == 14)
        {
            buttonName = "Finish";
        }

        if (index == 15) // finish
        {
            index = 0;

            if (Uninstallation)
            {
                ItemDropSettings asset = ItemDropSettings.Settings;
                if (asset != null)
                {
                    asset.isInstalled = false;
                    asset.SetTagsAndLayers();
                    EditorUtility.SetDirty(asset);

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }

                progressText = "Uninstall Completed";
                showPath = false;
                Uninstallation = false;
            }
            else
            {
                AddItemDropSettings(false); // add item drop settings and write all unsaved asset changes to disk

                progressText = "Completed";
                showUninstallOption = true;
                showButton = false;
                buttonName = "Next";
            }
        }
    }

    public override void SwitchPage()
    {
        if (Uninstallation)
        {
            switch (index)
            {
                case 1:
                    RevertInstallation(ItemDropSetupPage.RemasteredAll[13]);    // Page 13
                    break;
                case 2:
                    Next(ItemDropSetupPage.RemasteredAll[12], false);           // Page 12
                    break;
                case 3:
                    Next(ItemDropSetupPage.RemasteredAll[11]);                  // Page 11
                    break;
                case 4:
                    Next(ItemDropSetupPage.RemasteredAll[10]);                  // Page 10
                    break;
                case 5:
                    Next(ItemDropSetupPage.RemasteredAll[9]);                   // Page 9
                    break;
                case 6:
                    Next(ItemDropSetupPage.RemasteredAll[8]);                   // Page 8
                    break;
                case 7:
                    Next(ItemDropSetupPage.RemasteredAll[7]);                   // Page 7
                    break;
                case 8:
                    Next(ItemDropSetupPage.RemasteredAll[6]);                   // Page 6
                    break;
                case 9:
                    Next(ItemDropSetupPage.RemasteredAll[5]);                   // Page 5
                    break;
                case 10:
                    Next(ItemDropSetupPage.RemasteredAll[4]);                   // Page 4
                    break;
                case 11:
                    Next(ItemDropSetupPage.RemasteredAll[3]);                   // Page 3
                    break;
                case 12:
                    Next(ItemDropSetupPage.RemasteredAll[2]);                   // Page 2
                    break;
                case 13:
                    Next(ItemDropSetupPage.RemasteredAll[1]);                   // Page 1
                    break;
                case 14:
                    Next(ItemDropSetupPage.RemasteredAll[16]);                  // Page 16 (completed)
                    break;
                default:
                    FirstPage(ItemDropSetupPage.RemasteredAll[14], true);       // Page 14
                    break;
            }
        }
        else
        {
            switch (index)
            {
                case 1:
                    Next(ItemDropSetupPage.RemasteredAll[2]);                   // Page 2
                    break;
                case 2:
                    Next(ItemDropSetupPage.RemasteredAll[3]);                   // Page 3                
                    break;
                case 3:
                    Next(ItemDropSetupPage.RemasteredAll[4]);                   // Page 4
                    break;
                case 4:
                    Next(ItemDropSetupPage.RemasteredAll[5]);                   // Page 5
                    break;
                case 5:
                    Next(ItemDropSetupPage.RemasteredAll[6]);                   // Page 6
                    break;
                case 6:
                    Next(ItemDropSetupPage.RemasteredAll[7]);                   // Page 7
                    break;
                case 7:
                    Next(ItemDropSetupPage.RemasteredAll[8]);                   // Page 8
                    break;
                case 8:
                    Next(ItemDropSetupPage.RemasteredAll[9]);                   // Page 9
                    break;
                case 9:
                    Next(ItemDropSetupPage.RemasteredAll[10]);                  // Page 10
                    break;
                case 10:
                    Next(ItemDropSetupPage.RemasteredAll[11]);                  // Page 11
                    break;
                case 11:
                    Next(ItemDropSetupPage.RemasteredAll[12]);                  // Page 12
                    break;
                case 12:
                    Next(ItemDropSetupPage.RemasteredAll[13]);                  // Page 13
                    break;
                case 13:
                    Next(ItemDropSetupPage.RemasteredAll[14], false);           // Page 14
                    break;
                case 14:
                    FinishInstallation(ItemDropSetupPage.RemasteredAll[15]);    // Page 15 (completed)
                    break;
                default:
                    FirstPage(ItemDropSetupPage.RemasteredAll[1]);              // Page 1                    
                    break;
            }
        }
    }

    // unused
    bool MoveFolder(string oldPath, string newPath)
    {
        if (string.IsNullOrEmpty(AssetDatabase.ValidateMoveAsset(oldPath, newPath)))
        {
            AssetDatabase.MoveAsset(oldPath, newPath);
            AssetDatabase.Refresh();
            return true;
        }
        //Debug.Log($"The asset is already at '{newPath}'.");
        return false;
    }
}
#endif