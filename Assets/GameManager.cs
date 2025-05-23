using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
public class GameManager : MonoBehaviour
{
    private const string API_URL = "http://localhost:3000/Player/dc7f8a28";
    private const string BASE_API_URL = "http://localhost:3000/";
    public TextMeshProUGUI playerInfoTextBox;
    public TextMeshProUGUI ClubInfoTextBox;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetPlayer());
        //StartCoroutine(GetClubFromID("a77c513e"));
    }

    IEnumerator GetPlayer()
    {
        UnityWebRequest request = UnityWebRequest.Get(API_URL); 
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text; 
            Player[] players = Newtonsoft.Json.JsonConvert.DeserializeObject<Player[]>(json);
            string currentClubName = "";

            string new_url = BASE_API_URL + "clubs/" + players[0].current_club;
            UnityWebRequest club_request = UnityWebRequest.Get(new_url);
            yield return club_request.SendWebRequest();
            if (club_request.result == UnityWebRequest.Result.Success)
            {
                string club_json = club_request.downloadHandler.text;
                Club[] clubs = Newtonsoft.Json.JsonConvert.DeserializeObject<Club[]>(club_json);
                currentClubName = clubs[0].club_name;
            }
            else
            {
                Debug.LogError("Failed to fetch player: " + request.error);
                currentClubName = "";
            }

            Debug.Log(currentClubName);


            playerInfoTextBox.text = DisplayPlayerInfo(players[0], currentClubName);
        }
        else
        {
            Debug.LogError("Failed to fetch player: " + request.error);
        }
    }

    string DisplayPlayerInfo(Player p, string cn)
    {
        string answer =
            $"Player Info\n\n" +
            $"Name: {p.player_name}\n" +
            $"Nationality: {p.nationality}\n" +
            $"Position: {p.positions}\n" +
            $"Foot: {p.foot}\n" +
            $"Age: {p.age}\n" +
            $"Height: {p.height} cm\n" +
            $"Current Club ID: " + cn + "\n" +
            $"Clubs Played For: {p.clubs_played_for}\n" +
            $"Matches: {p.matches_played}\n" +
            $"Goals: {p.goals}\n" +
            $"Assists: {p.assists}";
        return answer;
    }

    IEnumerator GetClubFromID(string id)
    {
        string new_url = BASE_API_URL + "clubs/" + id;
        UnityWebRequest request = UnityWebRequest.Get(new_url);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            Club[] clubs = Newtonsoft.Json.JsonConvert.DeserializeObject<Club[]>(json);
            ClubInfoTextBox.text =  clubs[0].club_name;
        }
        else
        {
            Debug.LogError("Failed to fetch player: " + request.error);
            ClubInfoTextBox.text = "";
        }
    }




    [System.Serializable]
    public class Player
    {
        public string player_id;
        public string player_name;
        public string positions;
        public string foot;
        public int height;
        public int age;
        public string nationality;
        public string current_club;
        public int goals;
        public int assists;
        public int matches_played;
        public string clubs_played_for;
    }

    [System.Serializable]
    public class Club
    {
        public string club_id;
        public string club_name;
        
    }



}
