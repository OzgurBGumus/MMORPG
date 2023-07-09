using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GFFAddons
{
    [CreateAssetMenu(menuName = "ItemUpgrade/ Upgrade Material", order = 999)]
    public class UpgradeMaterialItem : ScriptableItem
    {
        [Header("GFF Upgrade addon")]
        public UpgradeMaterialType upgradeMaterialType;
        public string materialName;
        public string textInfo = "";
        public float chanceOfUpgrade;
        //public int[] effect = new int[PlayerUpgrade.maxUpgrade];

        // caching /////////////////////////////////////////////////////////////////
        // we can only use Resources.Load in the main thread. we can't use it when
        // declaring static variables. so we have to use it as soon as 'dict' is
        // accessed for the first time from the main thread.
        // -> we save the hash so the dynamic item part doesn't have to contain and
        //    sync the whole name over the network
        static Dictionary<int, UpgradeMaterialItem> cache;
        //public static Dictionary<int, UpgradeMaterialItem> upgradeMaterialDict
        //{
        //    get
        //    {
        //        // not loaded yet?
        //        if (cache == null)
        //        {
        //            // get all ScriptableItems in resources
        //            EnchantmentRuneItem[] items = Resources.LoadAll<EnchantmentRuneItem>("");

        //            // check for duplicates, then add to cache
        //            List<string> duplicates = items.ToList().FindDuplicates(item => item.name);
        //            if (duplicates.Count == 0)
        //            {
        //                cache = items.ToDictionary(item => item.name.GetStableHashCode(), item => item);
        //            }
        //            else
        //            {
        //                foreach (string duplicate in duplicates)
        //                    Debug.LogError("Resources folder contains multiple ScriptableItems with the name " + duplicate + ". If you are using subfolders like 'Warrior/Ring' and 'Archer/Ring', then rename them to 'Warrior/(Warrior)Ring' and 'Archer/(Archer)Ring' instead.");
        //            }
        //        }
        //        return cache;
        //    }
        //}
    }
}


