using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManagers : MonoBehaviour
{
    public bool dontDestroyOnLoad;
    // Start is called before the first frame update
    void Start()
    {
        if (!dontDestroyOnLoad)
        {
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
