using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPartyLootAuctionResultHUDItemSlot : MonoBehaviour
{
    public Text winner;
    public Image itemImage;
    public float duration = 6f;
    WaitForSeconds lifeTimeInterval;
    IEnumerator courtineLifeTime;


    // Start is called before the first frame update
    void Start()
    {
        courtineLifeTime = DeleteResult();
        lifeTimeInterval = new WaitForSeconds(duration);
        StartCoroutine(courtineLifeTime);
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator DeleteResult()
    {
        yield return lifeTimeInterval;
        Destroy(this.gameObject);
    }
}
