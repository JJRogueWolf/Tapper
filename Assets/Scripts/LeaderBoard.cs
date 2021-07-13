using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using UnityEngine.SceneManagement;

public class LeaderBoard : MonoBehaviour
{
    public GameObject loadings;
    public GameObject scrollContentHolder;
    public GameObject listDataPrefab;

    private List<User> users;

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
        loadings.SetActive(true);
        users = new List<User>();
        Debug.Log("Started");
        FirebaseDatabase.DefaultInstance
          .GetReference("User")
          .GetValueAsync().ContinueWith(task =>
          {
              Debug.Log("Recieved");
              if (task.IsFaulted)
              {
                  Debug.Log("Firebase Return Error");
              }
              else if (task.IsCompleted)
              {
                  DataSnapshot snapshot = task.Result;
                  foreach (DataSnapshot user in snapshot.Children)
                  {
                      IDictionary dictUser = (IDictionary)user.Value;
                      users.Add(new User(dictUser["id"].ToString(), dictUser["name"].ToString(), dictUser["score"].ToString()));
                  }
                  users = users.OrderByDescending(x => x.score).ToList();
                  Debug.Log(users);
                  setupList();
              }
          });
    }

    private void setupList()
    {
        Debug.Log("Setting List");
        string id = SystemInfo.deviceUniqueIdentifier;
        loadings.SetActive(false);
        int myPosition = -1;
        for(int i = 0; i < users.Count; i++)
        {
            if(users[i].id == id){
                myPosition = i;
            }
        }

        if (users.Count < 9)
        {
            for (int i = 0; i < users.Count; i++)
            {
                createListTile(i);
            }
        }
        else
        {
            for (int i = 0; i < 9; i++)
            {
                createListTile(i);
            }
            if(myPosition < 9)
            {
                createListTile(9);
            }
            else
            {
                createListTile(myPosition);
            }
            
        }
    }

    private void createListTile(int position)
    {
        User user = users[position];
        GameObject list = Instantiate(listDataPrefab) as GameObject;
        list.transform.SetParent(scrollContentHolder.transform);
        list.transform.localScale = new Vector3(1, 1, 1);
        list.transform.localPosition = new Vector3(0, 0, 0);
        list.transform.Find("rank").GetComponent<Text>().text = (position + 1).ToString();
        list.transform.Find("name").GetComponent<Text>().text = user.name;
        list.transform.Find("score").GetComponent<Text>().text = user.score;
    }


    public void loadlandingPage()
    {
        SceneManager.LoadScene(0);
    }

}
