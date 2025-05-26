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
    private const string ANSWER_API_URL = "http://localhost:3000/Player/dc7f8a28";
    private const string ANSWER = "dc7f8a28";
    private const string BASE_API_URL = "http://localhost:3000/";
    public  string answer_id;
    public TextMeshProUGUI CMBox;
    public TextMeshProUGUI PlayerBox;
    public TextMeshProUGUI ClubInfoTextBox;
    public USW player_input_actions;
    private InputAction test;


    private Dictionary<string, string> clubMap;
    private bool initializing;

    // Start is called before the first frame update
    void Start()
    {
        
        clubMap = new Dictionary<string, string>();
        StartCoroutine(InitializeDatabase());
        //StartCoroutine(GetClubFromID("a77c513e"));
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
        string submission = BuildJson(id, ANSWER);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(submission);

        UnityWebRequest request = new UnityWebRequest(newUrl, "POST");

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            PlayerComparison cmp = Newtonsoft.Json.JsonConvert.DeserializeObject<PlayerComparison>(json);




            CMBox.text = DisplayComparisonInfo(cmp);

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
            PlayerBox.text = DisplayPlayerInfo(players[0]);
        }

        else
        {
            Debug.LogError("Failed to fetch player: " + request.error);
        }
    }

    string BuildJson(string id1, string id2)
    {
        string answer = $"{{\"id1\":\"{id1}\",\"id2\":\"{id2}\"}}";
        Debug.Log(answer);
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

    public static string DisplayComparisonInfo(PlayerComparison comp)
    {
        return
            $"Nation: {Format(comp.nation_comparison)}\n" +
            $"Position: {Format(comp.positions_comparison)}\n" +
            $"Foot: {Format(comp.foot_comparison)}\n" +
            $"Height: {Format(comp.height_comparison)}\n" +
            $"Age: {Format(comp.age_comparison)}\n" +
            $"Current Club: {Format(comp.cc_comparison)}\n" +
            $"Clubs Played For: {Format(comp.cpf_comparison)}\n" +
            $"Goals: {Format(comp.goals_comparison)}\n" +
            $"Assists: {Format(comp.assists_comparison)}\n" +
            $"Matches Played: {Format(comp.mp_comparison)}";
    }

    private static string Format(string comparison)
    {
        switch (comparison)
        {
            case "equal": return "Match";
            case "partial": return "Partial Match";
            case "greater": return "Higher";
            case "less_than": return "Lower";
            case "unknown": return "Unknown";
            default: return comparison;
        }
    }


    private void Clicker(InputAction.CallbackContext context)
    {
        if (!initializing)
        {
            //StartCoroutine(GetPlayer());
        }

        
    }

    private HashSet<String> DeSerializeString(string str)
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

    private string SetToString(HashSet<String> container)
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
