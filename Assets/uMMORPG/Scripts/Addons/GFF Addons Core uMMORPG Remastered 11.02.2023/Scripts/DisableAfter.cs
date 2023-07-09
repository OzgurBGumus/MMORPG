using System.Collections;
using UnityEngine;

public class DisableAfter : MonoBehaviour
{
    public float time = 0.0f;

    private void OnEnable()
    {
        StartCoroutine(SetActive());
    }

    public IEnumerator SetActive()
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }
}
