using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using UnityEngine.SceneManagement;

public class LeaderBoard : MonoBehaviour
{
    public GameObject loadings;
    public GameObject scrollContentHolder;
    public GameObject listDataPrefab;
    public gameGoogleAdsScript adsScript;

    private List<User> users;
    private string id;

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
        try
        {
            id = SystemInfo.deviceUniqueIdentifier;
            loadings.SetActive(true);
            Debug.Log("Started");
            users = new List<User>();
            Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            auth.SignInAnonymouslyAsync().ContinueWith(task =>
            {
                adsScript.bannerView.Show();
                FirebaseDatabase.DefaultInstance
                  .GetReference("User")
                  .GetValueAsync().ContinueWith(taskData =>
                  {
                      Debug.Log("Recieved");
                      if (task.IsFaulted)
                      {
                          Debug.Log("Firebase Return Error");
                      }
                      else if (task.IsCompleted)
                      {
                          DataSnapshot snapshot = taskData.Result;
                          List<User> currentUser = new List<User>();
                          foreach (DataSnapshot user in snapshot.Children.ToList())
                          {
                              IDictionary dictUser = (IDictionary)user.Value;
                              currentUser.Add(new User(dictUser["id"].ToString(), dictUser["name"].ToString(), dictUser["score"].ToString()));
                          }
                          users = new List<User>();
                          users = currentUser.OrderByDescending(x => int.Parse(x.score)).ToList();
                          Debug.Log(users);
                          setupList();
                      }
                  });
            });
        } catch (UnityException e)
        {
            Debug.Log("" + e.ToString());
        }
    }

    private void setupList()
    {
        FirebaseDatabase.DefaultInstance.GoOffline();
        try
        {
            Debug.Log("Setting List " + id);
            loadings.SetActive(false);
            int myPosition = -1;
            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].id == id)
                {
                    myPosition = i;
                }
            }

            if (users.Count < 9)
            {
                for (int i = 0; i < users.Count; i++)
                {
                    if (users[i] != null)
                    {
                        createListTile(i, myPosition);
                    }
                }
            }
            else
            {
                for (int i = 0; i < 9; i++)
                {
                    if (users[i] != null)
                    {
                        createListTile(i, myPosition);
                    }
                }
                if (myPosition < 9)
                {
                    createListTile(9, myPosition);
                }
                else
                {
                    createListTile(myPosition, myPosition);
                }

            }
        } catch (UnityException e)
        {
            Debug.Log("" + e.ToString());
        }
    }

    private void createListTile(int position, int myPosition)
    {
        try
        {
            User data = users[position];
            GameObject list = Instantiate(listDataPrefab);
            if (position == myPosition)
            {
                list.GetComponent<LeaderboardCardScript>().setSelectedImage();
            }
            list.transform.SetParent(scrollContentHolder.transform);
            list.transform.localScale = new Vector3(1, 1, 1);
            list.transform.localPosition = new Vector3(0, 0, 0);
            list.transform.Find("rank").GetComponent<Text>().text = (position + 1).ToString();
            list.transform.Find("name").GetComponent<Text>().text = data.name;
            list.transform.Find("score").GetComponent<Text>().text = data.score;
        } catch(UnityException e) {
            Debug.Log(" " + e.ToString());
        }
    }

    public void loadlandingPage()
    {
        SceneManager.LoadScene(0);
    }

    private void OnDestroy()
    {
        
    }

}
