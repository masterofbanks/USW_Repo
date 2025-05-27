using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEditor.PackageManager.Requests;
using System.Linq;
using UnityEngine.InputSystem;
using System.ComponentModel;
using Newtonsoft.Json.Linq;
public class GameManager : MonoBehaviour
{
    private const string ANSWER_API_URL = "http://localhost:3000/Player/4d77b365";
    private const string ANSWER = "4d77b365";
    private const string BASE_API_URL = "http://localhost:3000/";
    public  string answer_id;
    public TextMeshProUGUI CMBox;
    public TextMeshProUGUI PlayerBox;
    public TextMeshProUGUI ClubInfoTextBox;
    public USW player_input_actions;
    private InputAction test;
    public GameObject TopGrid;

    public Color correct_color;
    public Color partial_correct_color;
    public Color incorrect_color;
    public Color unknown;

    public int num_attempts;
    public TextMeshProUGUI attempt_box;

    public Dictionary<string, string> clubMap;
    private bool initializing;

    // Start is called before the first frame update
    void Start()
    {
        
        clubMap = new Dictionary<string, string>();
        StartCoroutine(InitializeDatabase());
        //StartCoroutine(GetClubFromID("a77c513e"));
    }

    private void Update()
    {
        attempt_box.text = "Attempts: " + num_attempts.ToString();
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
        
    }
    public IEnumerator GetPlayer(string id)
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
            TopGrid.GetComponent<GuessBehavior>().ShowGuess(players[0], cmp);

            num_attempts--;
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

    string DisplayPlayerInfo(Player p)
    {
        string foot = string.IsNullOrEmpty(p.foot) ? "Unknown" : p.foot;
        string nationality = string.IsNullOrEmpty(p.nationality) ? "Unknown" : p.nationality;
        string height = p.height.HasValue ? $"{p.height.Value} cm" : "Unknown";
        string age = p.age.HasValue ? $"{p.age.Value}" : "Unknown";

        string answer =
            $"Player Info\n\n" +
            $"Name: {p.player_name}\n" +
            $"Nationality: {nationality}\n" +
            $"Position: {p.positions}\n" +
            $"Foot: {foot}\n" +
            $"Age: {age}\n" +
            $"Height: {height}\n" +
            $"Current Club ID: " + clubMap[p.current_club] + "\n" +
            $"Clubs Played For: " + SetToString( DeSerializeString(p.clubs_played_for)) + "\n" +
            $"Matches: {p.matches_played}\n" +
            $"Goals: {p.goals}\n" +
            $"Assists: {p.assists}";
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
        


    }

    [System.Serializable]
    public class Club
    {
        public string club_id;
        public string club_name;
        
    }



}
