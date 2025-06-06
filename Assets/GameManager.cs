﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine.InputSystem;
using System.ComponentModel;
using Newtonsoft.Json.Linq;
using static GameManager;
using static Unity.Burst.Intrinsics.X86.Avx;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    private string ANSWER;
    public string ANSWER_NAME;
    private const string BASE_API_URL = "https://soccer-api-1tkh.onrender.com/";
    public TextMeshProUGUI CMBox;
    public TextMeshProUGUI PlayerBox;
    public TextMeshProUGUI ClubInfoTextBox;
    public USW player_input_actions;
    public GameObject CorrectAnswerGrid;
    private InputAction test;

    public Color correct_color;
    public Color partial_correct_color;
    public Color incorrect_color;
    public Color unknown;

    public int num_attempts;
    public TextMeshProUGUI attempt_box;

    public Dictionary<string, string> clubMap;

    public TextMeshProUGUI congrats;
    public GameObject button;
    public GameObject searching;
    private bool initializing;

    // Start is called before the first frame update
    void Start()
    {
        
        clubMap = new Dictionary<string, string>();
        StartCoroutine(InitializeDatabase());
        button.SetActive(false);
        searching.SetActive(true);
        congrats.enabled = false;
    }

    private void Update()
    {
        attempt_box.text = "Attempts Left: " + num_attempts.ToString();
    }

    private void Awake()
    {
        player_input_actions = new USW();
    }
    IEnumerator InitializeDatabase()
    {
        initializing = true;
        string new_url = BASE_API_URL + "clubs";
        UnityWebRequest club_request = UnityWebRequest.Get(new_url);
        yield return club_request.SendWebRequest();
        Club[] clubs;
        if (club_request.result == UnityWebRequest.Result.Success)
        {
            string club_json = club_request.downloadHandler.text;
            clubs = Newtonsoft.Json.JsonConvert.DeserializeObject<Club[]>(club_json);
            for (int i = 0; i < clubs.Length; i++)
            {
                clubMap.Add(clubs[i].club_id, clubs[i].club_name);
            }
        }
        else
        {
            Debug.LogError("Failed to fetch player: " + club_request.error);
        }

        ClubInfoTextBox.text = "Finished Gathering CLub Data";
        initializing = false;

        string random_url = BASE_API_URL + "random";
        UnityWebRequest random_request = UnityWebRequest.Get(random_url);
        yield return random_request.SendWebRequest();
        if (random_request.result == UnityWebRequest.Result.Success)
        {
            string id_json = random_request.downloadHandler.text;
            ID answer_id = Newtonsoft.Json.JsonConvert.DeserializeObject<ID>(id_json);
            ANSWER = answer_id.player_id;
            StartCoroutine(GetPlayer(ANSWER, CorrectAnswerGrid.GetComponent<BaseGuessBehavior>()));
            
            Debug.Log(id_json);
        }
        else
        {
            Debug.LogError("Failed to fetch random: " + club_request.error);
        }

        string newUrl = BASE_API_URL + "Player/" + ANSWER;
        UnityWebRequest request = UnityWebRequest.Get(newUrl);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            Player[] players = Newtonsoft.Json.JsonConvert.DeserializeObject<Player[]>(json);
            //PlayerBox.text = DisplayPlayerInfo(players[0]);
            ANSWER_NAME = players[0].player_name;

        }

        else
        {
            Debug.LogError("Failed to fetch player: " + request.error);
        }

    }
    public IEnumerator GetPlayer(string id, BaseGuessBehavior gb)
    {
        string newUrl = BASE_API_URL + "compare";
        string submission = BuildJson(ANSWER, id);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(submission);

        UnityWebRequest request = new UnityWebRequest(newUrl, "POST");

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        PlayerComparison cmp = new PlayerComparison();
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            cmp = Newtonsoft.Json.JsonConvert.DeserializeObject<PlayerComparison>(json);





        }
        else
        {
            Debug.LogError("Failed to fetch player: " + request.error);
        }

        newUrl = BASE_API_URL + "Player/" + id;
        request = UnityWebRequest.Get(newUrl);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            Player[] players = Newtonsoft.Json.JsonConvert.DeserializeObject<Player[]>(json);
            //PlayerBox.text = DisplayPlayerInfo(players[0]);
            gb.ShowGuess(players[0], cmp);

        }

        else
        {
            Debug.LogError("Failed to fetch player: " + request.error);
        }
    }

    string BuildJson(string id1, string id2) 
    {
        string answer = $"{{\"id1\":\"{id1}\",\"id2\":\"{id2}\"}}";
        return answer;
    }

    

   
        
    public Color getColor(string comparison)
    {
            switch (comparison)
            {
                case "equal": return correct_color;
                case "partial": return partial_correct_color;
                case "greater": return incorrect_color;
                case "less_than": return incorrect_color;
                case "unknown": return unknown;
                default: return incorrect_color;
            }
        
    }

    private void Clicker(InputAction.CallbackContext context)
    {
        if (!initializing)
        {
            //StartCoroutine(GetPlayer());
        }

        
    }

    public HashSet<String> DeSerializeString(string str)
    {
        HashSet<String> container = new HashSet<String>();
        int start = 0;
        string word = "";
        foreach(char s in str)
        {
            if(s == ' ')
            {
                word = str.Substring(start, 8);
                container.Add(word);
                start = start + 9;
            }
        }
        word = str.Substring(start, 8);
        container.Add(word);


        return container;
    }

    public string SetToString(HashSet<String> container)
    {
        string answer = "";
        foreach(string s in container)
        {
            answer += (clubMap[s] + ", ");
        }

        answer = answer.Substring(0, answer.Length - 2);
        return answer;
    }

    public void Endgame(string release_text)
    {
        button.SetActive(true);
        searching.SetActive(false);
        congrats.text = release_text;
        congrats.enabled = true;
        
        
    }

    private void OnEnable()
    {
        test = player_input_actions.UI.Click;
        test.Enable();
        test.performed += Clicker;
    }

    private void OnDisable()
    {
        test.Disable();
    }

    [System.Serializable]
    public class Player
    {
        public string player_id;
        public string player_name;
        public string positions;
        public string foot;
        public int? height;
        public int? age;
        public string nationality;
        public string current_club;
        public int goals;
        public int assists;
        public int matches_played;
        public string clubs_played_for;
        public string league;


    }

    [System.Serializable]
    public class PlayerComparison
    {
        public string name_comparison;
        public string positions_comparison;
        public string foot_comparison;
        public string height_comparison;
        public string age_comparison;
        public string nation_comparison;
        public string cc_comparison;
        public string goals_comparison;
        public string assists_comparison;
        public string mp_comparison;
        public string cpf_comparison;
        public string league_comparison;
        


    }

    [System.Serializable]
    public class Club
    {
        public string club_id;
        public string club_name;
        
    }

    [System.Serializable]
    public class ID
    {
        public string player_id;

    }

    public string getAnswer()
    {
        return ANSWER;
    }

    public void Replay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }



}
