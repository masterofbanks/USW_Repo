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
public class GameManager : MonoBehaviour
{
    private const string ANSWER_API_URL = "http://localhost:3000/Player/da8b19e2";
    private const string BASE_API_URL = "http://localhost:3000/";
    public TextMeshProUGUI playerInfoTextBox;
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
    IEnumerator GetPlayer()
    {
        UnityWebRequest request = UnityWebRequest.Get(ANSWER_API_URL); 
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text; 
            Player[] players = Newtonsoft.Json.JsonConvert.DeserializeObject<Player[]>(json);

            


            playerInfoTextBox.text = DisplayPlayerInfo(players[0]);
            
        }
        else
        {
            Debug.LogError("Failed to fetch player: " + request.error);
        }
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

    private void Clicker(InputAction.CallbackContext context)
    {
        if (!initializing)
        {
            StartCoroutine(GetPlayer());
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

    /*IEnumerator GetClubFromID(string id, ref string cCN)
    {
        string new_url = BASE_API_URL + "clubs/" + id;
        UnityWebRequest club_request = UnityWebRequest.Get(new_url);
        yield return club_request.SendWebRequest();
        if (club_request.result == UnityWebRequest.Result.Success)
        {
            string club_json = club_request.downloadHandler.text;
            Club[] clubs = Newtonsoft.Json.JsonConvert.DeserializeObject<Club[]>(club_json);
            cCN = clubs[0].club_name;
        }
        else
        {
            Debug.LogError("Failed to fetch player: " + club_request.error);
            cCN = "";
        }
    }*/


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


        public void MakeSets()
        {

        }
    }

    [System.Serializable]
    public class Club
    {
        public string club_id;
        public string club_name;
        
    }



}
