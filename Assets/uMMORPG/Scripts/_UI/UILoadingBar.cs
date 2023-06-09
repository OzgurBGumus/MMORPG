using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILoadingBar : MonoBehaviour
{
    public Image BarFilled;
    private NetworkManagerMMO manager;
    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("NetworkManager").GetComponent<NetworkManagerMMO>();
    }

    // Update is called once per frame
    void Update()
    {
        BarFilled.fillAmount = manager.sceneLoadingProcess;
    }
}
