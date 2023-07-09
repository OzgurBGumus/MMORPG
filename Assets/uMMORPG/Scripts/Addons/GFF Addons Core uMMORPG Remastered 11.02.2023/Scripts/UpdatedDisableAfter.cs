using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class UpdatedDisableAfter : MonoBehaviour
{
    public float time = 2.5f;
    double startTime;

    public void Show()
    {
        startTime = NetworkTime.time;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (NetworkTime.time > startTime + time) gameObject.SetActive(false);
    }
}
