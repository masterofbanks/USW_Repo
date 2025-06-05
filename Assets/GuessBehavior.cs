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
    public GameObject CorrectGuessAttributes;
    public override void ShowGuess(GameManager.Player inputPlayer, GameManager.PlayerComparison cmp)
    {
        
        
        ShowGuessHelper(inputPlayer, cmp);
        GuessContainers[currentIndex].SetActive(true);
        currentIndex++;
        StartCoroutine(ScrollToBottom());

        if (gameManager.getAnswer() == inputPlayer.player_id)
        {
            if(numGuesses() == 1)
                gameManager.Endgame($"Congrats! You got the answer in " + numGuesses() + " attempt!");
            else
                gameManager.Endgame($"Congrats! You got the answer in " + numGuesses() + " attempts!");
        }

        else if (gameManager.num_attempts == 0)
        {
            Debug.Log("No attempts left");
            gameManager.Endgame("Unfortunate! Here is the Correct Answer:");
            gameManager.CorrectAnswerGrid.SetActive(true); CorrectGuessAttributes.SetActive(true);
        }

    }

    IEnumerator ScrollToBottom()
    {
        yield return new WaitForEndOfFrame();
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
        Canvas.ForceUpdateCanvases();
    }


}
