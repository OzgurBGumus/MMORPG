using UnityEngine;

public class LabelCamera : MonoBehaviour
{
    void Awake()
    {
        Vector2 resolution = transform.position;
        resolution.x = Screen.width / 2;
        resolution.y = Screen.height / 2;
        transform.position = resolution;
    }
}
