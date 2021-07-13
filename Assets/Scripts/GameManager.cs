using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public PlatformManager platformManager;
    public CubeColorChange cubeColor;
    public GameObject RetryScreen;

    public CubeColorChange cubeColorChange;

    private float speed = 8;
    
    private User currentUser;
    private string id;

    DatabaseReference reference;

    private bool isSaved = false;

    public class User
    {
        public string id;
        public string name;
        public string score;

        public User(string id, string name, string score)
        {
            this.id = id;
            this.name = name;
            this.score = score;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        id = SystemInfo.deviceUniqueIdentifier;
        isSaved = false;
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        reference.Child("User").Child(id).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("Firebase Return Error");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                IDictionary dictUser = (IDictionary)snapshot.Value;
                if (dictUser["id"].ToString().Equals(id))
                {
                    currentUser = new User(dictUser["id"].ToString(), dictUser["name"].ToString(), dictUser["score"].ToString());
                }
            }
        });
        RetryScreen.SetActive(false);
        platformManager.spawnSpeed = 0.1f;
        StartCoroutine(increamentSpeed());
    }

    // Update is called once per frame
    void Update()
    {
        if (cubeColor.health > 0)
        {
            
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
        } else
        {
            RetryScreen.SetActive(true);
            print(cubeColorChange.score);
            saveScore();
        }
    }

    IEnumerator increamentSpeed()
    {
        yield return new WaitForSeconds(6);
        speed += 0.3f;
        //platformManager.spawnSpeed -= 0.3f;
        StartCoroutine(increamentSpeed());
    }

    private void saveScore()
    {
        if (!isSaved)
        {
            isSaved = true;
            if (currentUser != null)
            {
                if (cubeColorChange.score > int.Parse(currentUser.score)){
                    currentUser.score = cubeColorChange.score.ToString();
                    print(currentUser.score);
                    saveToDB(currentUser);
                }
            }
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //Application.LoadLevel(Application.loadedLevel);
    }

    public void ExitrtGame()
    {
        SceneManager.LoadScene(2);
        //Application.LoadLevel(Application.loadedLevel);
    }

    private void saveToDB(User user)
    {
        reference.Child("User").Child(id).SetRawJsonValueAsync(JsonUtility.ToJson(currentUser));

    }
}
