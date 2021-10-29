using System.Collections;
using System.Linq;
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
        FirebaseDatabase.DefaultInstance.GoOnline();

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
                      List<User> currentUsers = new List<User>();
                      foreach (DataSnapshot user in snapshot.Children)
                      {
                          IDictionary dictUser = (IDictionary)user.Value;

                          if (dictUser["id"].ToString().Equals(myId))
                          {
                              currentUser = new User(dictUser["id"].ToString(), dictUser["name"].ToString(), dictUser["score"].ToString());
                          }
                          currentUsers.Add(new User(dictUser["id"].ToString(), dictUser["name"].ToString(), dictUser["score"].ToString()));

                          print(dictUser["id"] + ", My ID : " + myId);
                      }
                      users = currentUsers.OrderByDescending(x => int.Parse(x.score)).ToList();
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
                      for (int i = 0; i < users.Count; i++)
                      {
                          if (users[i].id == currentUser.id)
                          {
                              rank = i + 1;
                          }
                      }
                      rankText.text = rank.ToString();
                      rankText.gameObject.SetActive(true);
                      rankTitleText.gameObject.SetActive(true);
                      string nameString = currentUser.name;
                      nameInputField.text = nameString;
                      //nameInputField.text = ;
                  }
              });
        });

        Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;


    }

    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        UnityEngine.Debug.Log("Received Registration Token: " + token.Token);
    }

    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        UnityEngine.Debug.Log("Received a new message from: " + e.Message.From);
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
