using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class tutorialLoading : MonoBehaviour
{
    public Scrollbar scrollbar;
    public GameObject reflextScreen;

    // Start is called before the first frame update
    void Start()
    {
        reflextScreen.SetActive(false);
        scrollbar.size = 0;
        StartCoroutine(addProgress());
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator addProgress()
    {
        yield return new WaitForSeconds(Random.Range(0.3f,1f));
        if (scrollbar.size < 0.95)
        {
            float addupProgress = Random.Range(0.05f, 0.2f);
            scrollbar.size += addupProgress;
            StartCoroutine(addProgress());
        }
        else
        {
            reflextScreen.SetActive(true);
            StartCoroutine(startGame());
        }
    }

    IEnumerator startGame()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(1);
    }
}
