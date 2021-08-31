using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class awakScript : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
