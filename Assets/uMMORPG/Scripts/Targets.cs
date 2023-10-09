using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[CreateAssetMenu]
public class Targets : ScriptableObject//, IPointerClickHandler
{
    //public static Vector3 positionBandit = new Vector3(10, 2, 5);
    [Serializable]
    public class StringVector3Pair
    {
        public string key;
        public Vector3 value;
    }
    public List<StringVector3Pair> keyValuePairs = new List<StringVector3Pair>();


    // Property to access the dictionary
    public Dictionary<string, Vector3> TargetPositions
    {
        get
        {
            var dictionary = new Dictionary<string, Vector3>();
            foreach (var pair in keyValuePairs)
            {
                dictionary[pair.key] = pair.value;
            }
            return dictionary;
        }
    }
}
