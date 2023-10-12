using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TMPTextOutline : MonoBehaviour
{
    public float width = 0.15f;
    void Update()
    {
        TMP_Text textmeshPro = GetComponent<TMP_Text>();
        textmeshPro.outlineWidth = width;
        textmeshPro.outlineColor = new Color32(0, 0, 0, 255);
    }
}
