using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameNamespace;

public class AI_BehaviorTree : Player
{
    int actionChance;
    int aiHealth = 3;
    public GameManager manager;
    public System.Random random = new System.Random();
    

    public override  GameNamespace.Action ChooseActionAI()
    {
        int playerBullets = manager.player1.bullets;

        //Check if Bazooka action is available for AI
        if (bullets == 6)
        {
            action = GameNamespace.Action.z;
            return action;
        }

        //If neither player nor AI have bullets, reload.
        if (playerBullets == 0 && bullets == 0)
        {
            action = GameNamespace.Action.r;
            return action;
        }

        // If AI has ammo
        if (bullets >= 1)
        {
            //If AI has bullets but player does not
            // OR player is about to get bazooka, shoot
            if ((playerBullets == 0 || playerBullets == 5)&&turn>2)
            {
                action = GameNamespace.Action.s;
                return action;

            }
      
            //else random chance action
            else
            {
                int actionChance = random.Next(1, 101);
                if (actionChance <= 50) //40% chance block
                {
                    action = GameNamespace.Action.b;
                }
                else if (actionChance <= 80) //30% chance reload
                {
                    action = GameNamespace.Action.r;
                    return action;
                }
                else //30% chance shoot
                {
                    action = GameNamespace.Action.s;
                    return action;

                }
            }
        }
        //if AI has no ammo, random chance between block & reload
        else
        {
            int actionChance = random.Next(1, 101);

            if (actionChance <= 50) //50% chance reload
            {
                action = GameNamespace.Action.r;
                return action;
            }
            else //50% chance block
            {
                action = GameNamespace.Action.b;
                return GameNamespace.Action.b;
                
            }

        }
        return GameNamespace.Action.b;



    }
    public void Reload()
    {
        bullets++;
        SetBulletVisuals();

    }

    public void Shoot()
    {
        bullets--;
        SetBulletVisuals();
    }
}



