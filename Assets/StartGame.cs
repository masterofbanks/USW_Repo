using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using static GameManager;
using Newtonsoft.Json.Linq;
using TMPro;

public class StartGame : MonoBehaviour
{
    private const string BASE_API_URL = "https://soccer-api-1tkh.onrender.com/";
    public TextMeshProUGUI Loading;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Starting()
    {
        StartCoroutine(GetPlayer("ecbe2839"));
    }

    public IEnumerator GetPlayer(string id)
    {
        Loading.text = "Loading...";
        string newUrl = BASE_API_URL + "Player/" + id;
        UnityWebRequest request = UnityWebRequest.Get(newUrl);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            Player[] players = Newtonsoft.Json.JsonConvert.DeserializeObject<Player[]>(json);
            //PlayerBox.text = DisplayPlayerInfo(players[0]);
            Loading.text = "Finished Loading";
            SceneManager.LoadScene(1);


        }

        else
        {
            Debug.LogError("Failed to fetch player: " + request.error);
        }
    }
}
