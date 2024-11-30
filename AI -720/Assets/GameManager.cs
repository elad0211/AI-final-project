using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameNamespace;

public class GameManager : MonoBehaviour
{
    public Player player1;
    public N_Gram player2;
    public AI_BehaviorTree dumbAI;
    public bool AI_VS_AI;
    // Start is called before the first frame update
    int NgramWins;
    int dumbWins;
    public void CalcResult()
    {

        Debug.Log("Player 1 action: " + player1.action);
        Debug.Log("Player 2 action: " + player2.action);
        if (player1.action == player2.action)
        {
            if(player1.action==Action.s)
            {
                player2.Shoot();
                if (AI_VS_AI)
                {
                    dumbAI.Shoot();
                }
            }
            else if (player1.action == Action.r)
            {
                player2.Reload();
                if (AI_VS_AI)
                {
                    dumbAI.Reload();
                }
            }
            Debug.Log("Both players used the same action! It's a tie!"); 
        }
        else if (player1.action == Action.r && player2.action == Action.s)
        {
            NgramWins++;
            Debug.Log("Robocowboy cought you reloading! git gud, Player 2 WON!");
            ResetGame();
        }
        else if (player2.action == Action.r && player1.action == Action.s)
        {
            dumbWins++;
            Debug.Log("Robocowboy is dead! You win!");
            ResetGame();
        }
        else if (player2.action == Action.b && player1.action == Action.s)
        {
            if(AI_VS_AI)
            {
                dumbAI.Shoot();
            }
            Debug.Log("Robocowboy blocked the shot!");
        }
        else if (player1.action == Action.b && player2.action == Action.s)
        {
            if (AI_VS_AI)
            {
                player2.Shoot();
            }
            Debug.Log("you blocked robocowboy's shot!");
        }
        else if (player1.action == Action.z)
        {
            dumbWins++;

            Debug.Log("Player 1 BAZUUKA KILL!");
            ResetGame();

        }
        else if (player2.action == Action.z)
        {
            NgramWins++;

            Debug.Log("player 2 BAZUUKA KILL!");
            ResetGame();

        }
    }
    public void ResetGame()
    {
        player1.bullets = 0;
        player2.bullets = 0;
        dumbAI.bullets = 0;
        Debug.Log("Total game = " + (dumbWins + NgramWins) +
            "Dumb AI / Player won: " +dumbWins
            + "Ngram won: " + NgramWins);
        player2.currentRecord = "";
        for (int i = 0; i < player1.bulletsSprite.Length; i++)
        {
            player1.bulletsSprite[i].color = Color.black;
            player2.bulletsSprite[i].color = Color.black;

        }
    }
    public void HumanPlayerAct(int actionIndex)
    {
        if (actionIndex == 2 && player1.bullets == 0)
        {
            player1.action = Action.OutOfAmmo;
        }
        else if (actionIndex == 4 && player1.bullets < 6)
        {
            player1.action = Action.OutOfAmmo;

        }
        if (player1.action != Action.OutOfAmmo)
        {

            player2.ChooseActionAI();
            player1.ChooseAction(actionIndex);
            switch (actionIndex)
            {
                case 0:
                    player2.LogInput('b');
                    break;
                case 1:
                    player2.LogInput('r');
                    break;
                case 2:
                    player2.LogInput('s');
                    break;
                case 3:
                    player2.LogInput('z');
                    break;
                default:
                    break;
            }

            CalcResult();
        }
    }

    public void AivsAIAct()
    {
        player2.ChooseActionAI();

        player1.action = dumbAI.ChooseActionAI();
        switch (player1.action)
        {
            case Action.s:
                player2.LogInput('s');
                break;
            case Action.r:
                player2.LogInput('r');
                break;
            case Action.b:
                player2.LogInput('b');
                break;
            case Action.z:
                player2.LogInput('z');
                break;
            default:
            case Action.OutOfAmmo:
                break;
        }
        CalcResult();
    }

}
