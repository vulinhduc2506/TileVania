using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenePersist : MonoBehaviour
{
    void Awake()
    {
        int numSencePersist = FindObjectsOfType<ScenePersist>().Length;
        if (numSencePersist > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public void ResetSencePersist()
    {
        Destroy(gameObject);
    }
}
