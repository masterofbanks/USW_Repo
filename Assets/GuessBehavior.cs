using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GuessBehavior : BaseGuessBehavior
{

    public override void ShowGuess(GameManager.Player inputPlayer, GameManager.PlayerComparison cmp)
    {
        
        
        ShowGuessHelper(inputPlayer, cmp);

        if (gameManager.getAnswer() == inputPlayer.player_id)
        {
            gameManager.Endgame("Congrats! You got the answer in " + numGuesses() + " attempts!");
        }

        else if (gameManager.num_attempts == 0)
        {
            Debug.Log("No attempts left");
            gameManager.Endgame("The Correct Answer was " + gameManager.ANSWER_NAME);
        }

    }
    

}
