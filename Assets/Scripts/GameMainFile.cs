using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using UnityEngine.SceneManagement;

public class GameMainFile : MonoBehaviour
{
    public PlatformManager platformManager;
    public CubeColorChange cubeColor;
    public GameObject RetryScreen;

    public CubeColorChange cubeColorChange;

    public float speed = 8;
    
    private User currentUser;
    private string id;

    private AudioSource audioSource;

    DatabaseReference reference;

    private bool isSaved = false;

    [Header("Audio")]
    public AudioClip bgm;
    public AudioClip endAudio;

    private bool start = false;

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
        start = false;
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = bgm;
        audioSource.loop = true;
        audioSource.Play();
        id = SystemInfo.deviceUniqueIdentifier;
        isSaved = false;
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        //reference = FirebaseDatabase.DefaultInstance.GetReference("User");
        auth.SignInAnonymouslyAsync().ContinueWith(task =>
        {
            FirebaseDatabase.DefaultInstance.GetReference("User").GetValueAsync().ContinueWith(taskData =>
            {
                if (task.IsFaulted)
                {
                    Debug.Log("Firebase Return Error");
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = taskData.Result;
                    foreach (DataSnapshot user in snapshot.Children)
                    {
                        IDictionary dictUser = (IDictionary)user.Value;
                        if (dictUser["id"].ToString().Equals(id))
                        {
                            currentUser = new User(dictUser["id"].ToString(), dictUser["name"].ToString(), dictUser["score"].ToString());
                            Debug.Log("Current User : " + currentUser.name);
                            Debug.Log("Current Score : " + currentUser.score);
                        }
                    }
                }
            });
        });
        RetryScreen.SetActive(false);
        platformManager.spawnSpeed = 0.1f;
        StartCoroutine(increamentSpeed());
        StartCoroutine(startGame());
    }

    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            if (cubeColor.health > 0)
            {

                transform.Translate(Vector3.forward * Time.deltaTime * speed);
            }
            else
            {
                if (audioSource.clip != endAudio)
                {
                    audioSource.clip = endAudio;
                    audioSource.loop = false;
                    audioSource.Play();
                }
                RetryScreen.SetActive(true);
                RetryScreen.GetComponent<Animator>().SetTrigger("end_screen");
                print(cubeColorChange.score);
                saveScore();
            }
        }
    }

    IEnumerator startGame()
    {
        yield return new WaitForSeconds(0.6f);
        start = true;
    }

    IEnumerator increamentSpeed()
    {
        yield return new WaitForSeconds(6);
        speed += 2f;
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
        SceneManager.LoadScene(1);
        //Application.LoadLevel(Application.loadedLevel);
    }

    public void ExitrtGame()
    {
        SceneManager.LoadScene(2);
        //Application.LoadLevel(Application.loadedLevel);
    }

    private void saveToDB(User user)
    {
        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.SignInAnonymouslyAsync().ContinueWith(task =>
        {
            reference.Child("User").Child(id).SetRawJsonValueAsync(JsonUtility.ToJson(currentUser));
            Debug.Log("Score Gained : " + user.score);
        });
    }
}
