#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public sealed class ItemDropSetupClassic : ItemDropSetupBase
{
    [MenuItem("ItemDrop/Setup/Install: Classic")]
    static void Init()
    {
        // do not open existing window again
        if (HasOpenInstances<ItemDropSetupBase>())
            return;

        if (ItemDropSetupPage.ClassicAll.Length == 12)
        {
            // make a new window
            ItemDropSetupClassic window = (ItemDropSetupClassic)GetWindowWithRect(typeof(ItemDropSetupClassic), new Rect(0, 0, 470, 210));
            window.Show();
            
            window.GetPage(ItemDropSetupPage.ClassicAll[0], false);

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

    public override string DefineSymbol => "ITEM_DROP_C";
    public override float GetProgress() => (float)(index - 1) / 9;

    public override void GetPage(ItemDropSetupPage page, bool changeProgress = true)
    {
        base.GetPage(page, changeProgress);

        if (index == 9)
        {
            buttonName = "Finish";
        }

        if (index == 10) // finish
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
                AddItemDropSettings(); // add item drop settings and write all unsaved asset changes to disk

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
                    RevertInstallation(ItemDropSetupPage.ClassicAll[8]);    // Page 8
                    break;
                case 2:
                    Next(ItemDropSetupPage.ClassicAll[7], false);           // Page 7
                    break;
                case 3:
                    Next(ItemDropSetupPage.ClassicAll[6]);                  // Page 6
                    break;
                case 4:
                    Next(ItemDropSetupPage.ClassicAll[5]);                  // Page 5
                    break;
                case 5:
                    Next(ItemDropSetupPage.ClassicAll[4]);                  // Page 4
                    break;
                case 6:
                    Next(ItemDropSetupPage.ClassicAll[3]);                  // Page 3
                    break;
                case 7:
                    Next(ItemDropSetupPage.ClassicAll[2]);                  // Page 2
                    break;
                case 8:
                    Next(ItemDropSetupPage.ClassicAll[1]);                  // Page 1
                    break;
                case 9:
                    Next(ItemDropSetupPage.ClassicAll[11]);                 // Page 11 (completed)
                    break;
                default:
                    FirstPage(ItemDropSetupPage.ClassicAll[9], true);       // Page 9
                    break;
            }
        }
        else
        {
            switch (index)
            {
                case 1:
                    Next(ItemDropSetupPage.ClassicAll[2]);                  // Page 2
                    break;
                case 2:
                    Next(ItemDropSetupPage.ClassicAll[3]);                  // Page 3                
                    break;
                case 3:
                    Next(ItemDropSetupPage.ClassicAll[4]);                  // Page 4
                    break;
                case 4:
                    Next(ItemDropSetupPage.ClassicAll[5]);                  // Page 5
                    break;
                case 5:
                    Next(ItemDropSetupPage.ClassicAll[6]);                  // Page 6
                    break;
                case 6:
                    Next(ItemDropSetupPage.ClassicAll[7]);                  // Page 7
                    break;
                case 7:
                    Next(ItemDropSetupPage.ClassicAll[8]);                  // Page 8
                    break;
                case 8:
                    Next(ItemDropSetupPage.ClassicAll[9], false);           // Page 9
                    break;
                case 9:
                    FinishInstallation(ItemDropSetupPage.ClassicAll[10]);   // Page 10 (completed)
                    break;
                default:
                    FirstPage(ItemDropSetupPage.ClassicAll[1]);             // Page 1                    
                    break;
            }
        }
    }
}
#endif