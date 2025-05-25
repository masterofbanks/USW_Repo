using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using static GameManager;
using TMPro;
using UnityEngine.UI;

public class InputBehavior : MonoBehaviour
{
    private TMP_InputField inputField;
    public TMP_Dropdown dropdown;
    private const string BASE_API_URL = "http://localhost:3000/";

    public List<string> playerNames;
    public Player[] players;
    public GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        inputField = GetComponent<TMP_InputField>();
        playerNames = new List<string>();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateSearch()
    {
        StartCoroutine(SearchPlayers(inputField.text));
    }

    IEnumerator SearchPlayers(string searchTerm)
    {
        playerNames.Clear();
        string newRequest = BASE_API_URL + "Player/name/" + searchTerm;
        UnityWebRequest request = UnityWebRequest.Get(newRequest);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            players = Newtonsoft.Json.JsonConvert.DeserializeObject<Player[]>(json);
            playerNames.Add("Click Here to View Options");

            foreach (Player p in players)
            {
                playerNames.Add(p.player_name);
            }


            dropdown.ClearOptions();
            dropdown.AddOptions(playerNames);

        }
        else
        {
            Debug.LogError("Failed to fetch player: " + request.error);
        }
    }

    public void AcceptPlayerInput()
    {
        
        int pickedEntryIndex = dropdown.value - 1;
        if(pickedEntryIndex >= 0)
            StartCoroutine(gameManager.GetPlayer(players[pickedEntryIndex].player_id));
    }
}
