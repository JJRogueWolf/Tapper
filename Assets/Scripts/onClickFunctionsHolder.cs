using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class onClickFunctionsHolder : MonoBehaviour
{
    public void goToLeaderboard()
    {
        SceneManager.LoadScene(2);
    }
}
