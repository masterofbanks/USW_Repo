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
    private const string BASE_API_URL = "https://soccer-api-1tkh.onrender.com/";

    public List<string> playerNames;
    public Player[] players;
    public GameManager gameManager;
    public bool searching;
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
        if(!searching)
            StartCoroutine(SearchPlayers(inputField.text));
    }

    IEnumerator SearchPlayers(string searchTerm)
    {
        searching = true;
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
                string entry = p.player_name + " | Age: " + p.age.ToString();
                playerNames.Add(entry);
            }


            dropdown.ClearOptions();
            dropdown.AddOptions(playerNames);

        }
        else
        {
            Debug.LogError("Failed to fetch player: " + request.error);
        }

        searching = false;
    }

    public void AcceptPlayerInput()
    {
        
        int pickedEntryIndex = dropdown.value - 1;
        if(pickedEntryIndex >= 0 && gameManager.num_attempts > 0)
            StartCoroutine(gameManager.GetPlayer(players[pickedEntryIndex].player_id));
            gameManager.num_attempts--;

    }
}
