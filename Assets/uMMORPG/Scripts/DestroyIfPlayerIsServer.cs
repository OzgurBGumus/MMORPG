using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DestroyIfPlayerIsServer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(this.gameObject.scene.path != SceneManager.GetAllScenes()[0].path)
        {
            Destroy(this.gameObject);
        }
    }
}
