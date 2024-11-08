using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Player player1;
    public Player player2;
    // Start is called before the first frame update
   
    public void CalcResult()
    {
        Debug.Log("Player 1 action: " + player1.action);
        Debug.Log("Player 2 action: " + player2.action);
        if (player1.action == player2.action)
        {
            Debug.Log("Both players used the same action! It's a tie!"); 
        }
        else if (player1.action == Player.Action.Reload && player2.action == Player.Action.Shoot)
        {
            Debug.Log("Robocowboy cought you reloading! git gud, Player 2 WON!");
        }
        else if (player2.action == Player.Action.Reload && player1.action == Player.Action.Shoot)
        {
            Debug.Log("Robocowboy is dead! You win!");
        }
        else if (player2.action == Player.Action.Block && player1.action == Player.Action.Shoot)
        {
            Debug.Log("Robocowboy blocked the shot!");
        }
        else if (player1.action == Player.Action.Block && player2.action == Player.Action.Shoot)
        {
            Debug.Log("you blocked robocowboy's shot!");
        }
        else if (player1.action == Player.Action.Bazuka)
        {
            Debug.Log("Player 1 BAZUUKA KILL!");
        }
        else if (player2.action == Player.Action.Bazuka)
        {
            Debug.Log("player 2 BAZUUKA KILL!");
        }
    }
    public void HumanPlayerAct(int actionIndex)
    {
        player1.ChooseAction(actionIndex);
        if(player1.action!=Player.Action.OutOfAmmo)
        {
            player2.ChooseActionAI();
            CalcResult();
        }
    }
}
