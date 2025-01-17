using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseUser user;
    private FirebaseFirestore db;

    [SerializeField] Button profileButton, marketButton, leaderboardButton, logoutButton;

    [SerializeField] List<Button> categoryButtons = new List<Button>();

    private void Awake()
    {
        foreach (var button in categoryButtons)
        {
            button.onClick.AddListener(() => LoadQuizPage(button.GetComponentInChildren<TMP_Text>().text));
        }
    }

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;
        db = FirebaseFirestore.DefaultInstance;

        CheckIfUserLoggedIn();

        if(user == null) return;

        profileButton.onClick.AddListener(LoadProfileScene);
        marketButton.onClick.AddListener(LoadMarketScene);
        logoutButton.onClick.AddListener(Logout);
        leaderboardButton.onClick.AddListener(LoadLeaderboardScene);

        
        LoadPlayerPortrait();
    }

    private void LoadLeaderboardScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Leaderboard");
    }

    private void Logout()
    {
        auth.SignOut();
        UnityEngine.SceneManagement.SceneManager.LoadScene("FrontPage");
    }

    private void LoadMarketScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Market");
    }

    public void LoadQuizPage(string category)
    {
        Debug.Log("Loading " + category + " quiz page.");
        PlayerPrefs.SetString("SelectedCategory", category.ToLower());
        UnityEngine.SceneManagement.SceneManager.LoadScene("QuizPage");
    }

    private void CheckIfUserLoggedIn()
    {
        if (user == null)
        {
            Debug.Log("User is not logged in.");
            UnityEngine.SceneManagement.SceneManager.LoadScene("FrontPage");
        }
    }

    private async void LoadPlayerPortrait()
    {
        var allAvatars = Resources.LoadAll<Sprite>("AvatarPack");
        int currentAvatar = 0;

        await db.Collection("users").Document(auth.CurrentUser.UserId).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error loading profile: " + task.Exception);
                return;
            }

            DocumentSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                currentAvatar = snapshot.GetValue<int>("profilePhoto");
            }
            else
            {
                Debug.Log("No profile data found.");
            }
        });


        profileButton.GetComponent<Image>().sprite = allAvatars[currentAvatar];
    }

    public void LoadProfileScene()
    {
        ProfileManager.instance.OpenProfilePopup();
    }

}
