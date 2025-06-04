using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class BaseGuessBehavior : MonoBehaviour
{
    public GameObject[] GuessContainers;
    public GameManager gameManager;
    public int currentIndex;
    public ScrollRect scrollRect;


    // Start is called before the first frame update
    void Start()
    {
        currentIndex = 0;
    }

    public virtual void ShowGuess(GameManager.Player inputPlayer, GameManager.PlayerComparison cmp)
    {
        ShowGuessHelper(inputPlayer, cmp);

        

        

    }

    public void ShowGuessHelper(GameManager.Player inputPlayer, GameManager.PlayerComparison cmp)
    {
        TextMeshProUGUI[] textboxes = GuessContainers[currentIndex].GetComponentsInChildren<TextMeshProUGUI>();
        UnityEngine.UI.Image[] panels = GuessContainers[currentIndex].GetComponentsInChildren<UnityEngine.UI.Image>();
        textboxes[0].text = inputPlayer.player_name;
        panels[0].color = gameManager.getColor(cmp.name_comparison);


        textboxes[1].text = inputPlayer.positions;
        panels[1].color = gameManager.getColor(cmp.positions_comparison);

        textboxes[2].text = inputPlayer.foot;
        panels[2].color = gameManager.getColor(cmp.foot_comparison);

        panels[3].color = gameManager.getColor(cmp.height_comparison);
        textboxes[3].text = compareInts(cmp.height_comparison) + inputPlayer.height.ToString();


        panels[4].color = gameManager.getColor(cmp.age_comparison);
        textboxes[4].text = compareInts(cmp.age_comparison) + inputPlayer.age.ToString();

        textboxes[5].text = inputPlayer.nationality;
        panels[5].color = gameManager.getColor(cmp.nation_comparison);

        if(gameManager.clubMap[inputPlayer.current_club] == "Internazionale")
        {
            textboxes[6].text = "Inter";
        }

        else
        {
            textboxes[6].text = gameManager.clubMap[inputPlayer.current_club];
        }
        panels[6].color = gameManager.getColor(cmp.cc_comparison);

        panels[7].color = gameManager.getColor(cmp.goals_comparison);
        textboxes[7].text = compareInts(cmp.goals_comparison) + inputPlayer.goals.ToString();


        panels[8].color = gameManager.getColor(cmp.assists_comparison);
        textboxes[8].text = compareInts(cmp.assists_comparison) + inputPlayer.assists.ToString();

        panels[9].color = gameManager.getColor(cmp.mp_comparison);
        textboxes[9].text = compareInts(cmp.mp_comparison) + inputPlayer.matches_played.ToString();

        panels[10].color = gameManager.getColor(cmp.league_comparison);
        textboxes[10].text = inputPlayer.league;


        
    }
    public int numGuesses()
    {
        int answer = 0;
        for (int i = 0; i < GuessContainers.Length; i++)
        {
            if (GuessContainers[i].activeSelf)
            {
                answer++;
            }
        }

        return answer;
    }
    public string compareInts(string comparison)
    {
        switch (comparison)
        {
            case "greater": return "↑↑↑↑↑\n";
            case "less_than": return "↓↓↓\n";
            case "unknown": return "?\n";
            default: return "==\n";
        }
    }

    

}
