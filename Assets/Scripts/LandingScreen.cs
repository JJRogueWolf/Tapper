using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using TMPro;
using Firebase.Database;

public class LandingScreen : MonoBehaviour
{
    private List<User> users = new List<User>();
    private User currentUser;
    string myId;

    public GameObject splashScreen;

    public TMP_InputField nameInputField;
    DatabaseReference reference;

    public TMP_Text rankText;
    public TMP_Text rankTitleText;

    private int rank = 0;

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
        rank = 0;
        splashScreen.SetActive(true);
        rankText.gameObject.SetActive(false);
        rankTitleText.gameObject.SetActive(false);

        reference = FirebaseDatabase.DefaultInstance.RootReference;
        myId = SystemInfo.deviceUniqueIdentifier;
        Debug.Log("My ID " + myId);

        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.SignInAnonymouslyAsync().ContinueWith(task =>
        {
            FirebaseDatabase.DefaultInstance
              .GetReference("User")
              .GetValueAsync().ContinueWith(taskData =>
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
                          rank += 1;
                          if (dictUser["id"].ToString().Equals(myId))
                          {
                              currentUser = new User(dictUser["id"].ToString(), dictUser["name"].ToString(), dictUser["score"].ToString());
                          }
                          print(dictUser["id"] + ", My ID : " + myId);
                      }
                  }

                  splashScreen.SetActive(false);

                  if (currentUser == null)
                  {
                      currentUser = new User(myId, "Player", "0");
                      string nameString = currentUser.name + Random.Range(1, 9999);
                      nameInputField.text = nameString;
                      //nameInputField.text = currentUser.name;
                      Debug.Log("New User");
                  }
                  else
                  {
                      rankText.text = rank.ToString();
                      rankText.gameObject.SetActive(true);
                      rankTitleText.gameObject.SetActive(true);
                      string nameString = currentUser.name;
                      nameInputField.text = nameString;
                      //nameInputField.text = ;
                  }
              });
        });
    }

    public void onStartClicked()
    {
        currentUser.name = nameInputField.text;
        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.SignInAnonymouslyAsync().ContinueWith(task =>
        {
            reference.Child("User").Child(currentUser.id).SetRawJsonValueAsync(JsonUtility.ToJson(currentUser));
            SceneManager.LoadScene(3);
        });
    }
}
